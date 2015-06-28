using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Fasterflect;
using JetBrains.Annotations;

namespace Lindi.Core.Bindings
{
    /// <summary>
    /// Defines a binding that contains the ability to lazily build a <see cref="BindToConstructor{TInterface,TImplementer}"/>
    /// binding based on the given dependencies and expression.
    /// </summary>
    /// <typeparam name="TInterface">The type of values that this binding can produce.</typeparam>
    public class LazyConstructorBinding<TInterface> : BaseBinding<TInterface>, ILazyConstructorBinding<TInterface>
    {
        /// <summary>
        /// Gets the list of bindings that represent the dependencies of this binding.
        /// </summary>
        public IEnumerable<IBinding> Dependencies { get; protected set; }

        /// <summary>
        /// Gets the expression that was passed to this binding to define the constructor.
        /// </summary>
        public Expression<Func<IBinding[], TInterface>> ConstructionExpression { get; protected set; }

        /// <summary>
        /// Gets whether the constructor represented by this binding has been built.
        /// </summary>
        public bool IsBuilt => constructor.IsValueCreated;

        /// <summary>
        /// Gets the internal binding that is built by this lazy binding.
        /// </summary>
        public IBindToConstructor<TInterface> Constructor => constructor.Value;

        private readonly Lazy<IBindToConstructor<TInterface>> constructor;

        protected LazyConstructorBinding()
        {
            constructor = new Lazy<IBindToConstructor<TInterface>>(BuildConstructor);
        }

        /// <summary>
        /// Creates a new <see cref="LazyConstructorBinding{TInterface}"/> from the given dependencies and construction epression.
        /// </summary>
        /// <param name="dependencies">The array of bindings that represent dependencies that the constructor needs.</param>
        /// <param name="constructionExpression">
        /// An expression that specifies the constructor that should be called and also identifies the paring
        /// between a binding and constructor parameter.
        /// </param>
        /// <example>
        /// 
        /// <code>
        /// new LazyConstructorBinding&lt;TInterface&gt;(
        ///     new IBinding[] { dependentBinding1, dependentBinding2 },
        ///     bindings =&gt; new TImplementer(bindings[0].Resolve(), bindings[1].Resolve())
        /// );
        /// </code>
        /// </example>
        public LazyConstructorBinding([NotNull] IEnumerable<IBinding> dependencies, [NotNull] Expression<Func<IBinding[], TInterface>> constructionExpression) : this()
        {
            if (dependencies == null) throw new ArgumentNullException(nameof(dependencies));
            if (constructionExpression == null) throw new ArgumentNullException(nameof(constructionExpression));
            Dependencies = dependencies;
            ConstructionExpression = constructionExpression;
        }

        protected virtual IBindToConstructor<TInterface> BuildConstructor()
        {
            if (Dependencies == null) throw new InvalidOperationException($"{nameof(Dependencies)} must be set before trying to build the final expression.");
            if (ConstructionExpression == null) throw new InvalidOperationException($"{nameof(ConstructionExpression)} must be set before trying to build the final expression.");
            // Look through each dependency and inline it if possible
            var inliner = new DependencyInliner(Dependencies.ToArray());
            return new BindToConstructor<TInterface>(inliner.InlineDependencies(ConstructionExpression.Body).Compile());
        }

        /// <summary>
        /// Defines a <see cref="ExpressionVisitor"/> that provides functionality to inline a given expression
        /// that contains references to a list of <see cref="IBinding"/> dependencies granted that each dependency is a 
        /// lazy dependency.
        /// </summary>
        /// <example>
        /// Given a <see cref="LazyConstructorBinding{TInterface}"/> like this:
        /// 
        /// <code>
        /// var b =  new LazyConstructorBinding{TInterface}(new IBinding[]
        /// {
        ///     new LazyConstructorBinding{TDep}(new IBinding[0], bindings => new Dep())
        /// }, bindings => new Value(((LazyConstructorBinding{TDep})bindings[0]).Resolve()));
        /// </code>
        /// 
        /// And used like this:
        /// 
        /// <code>
        /// var inliner = new DependencyInliner(b.Dependencies);
        /// Expression{Func{TInterface}} finalExpression = inliner.InlineDependencies(b.ConstructionExpression);
        /// </code>
        /// 
        /// The inliner will produce an expression like this:
        /// 
        /// <code>
        /// new Value(new Dep())
        /// </code>
        /// </example>
        public class DependencyInliner : ExpressionVisitor
        {
            IBinding[] dependencies;

            public DependencyInliner(IBinding[] dependencies)
            {
                this.dependencies = dependencies;
            }

            public Expression<Func<TInterface>> InlineDependencies(Expression expression)
            {
                return Expression.Lambda<Func<TInterface>>(InlineDependenciesImpl(expression));
            }

            private Expression InlineDependenciesImpl(Expression expression)
            {
                return Visit(expression);
            }

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                if (node.Method.Name == nameof(IBinding<TInterface>.Resolve) && typeof(IBinding).IsAssignableFrom(node.Method.DeclaringType))
                {
                    ConstantExpression indexConstant = ((node.Object as UnaryExpression)?.Operand as BinaryExpression)?.Right as ConstantExpression;
                    if (indexConstant != null)
                    {
                        int index = (int)indexConstant.Value;
                        Type bindingType = dependencies[index].GetType();

                        if (bindingType.IsGenericType)
                        {
                            Type genericTypeDefinition = bindingType.GetGenericTypeDefinition();
                            Type lazyConstructorType = typeof(ILazyConstructorBinding<>);
                            if (genericTypeDefinition.Implements(lazyConstructorType) ||
                                genericTypeDefinition == lazyConstructorType)
                            {
                                var dep = dependencies[index];
                                var dependencyResolver = new DependencyInliner((IBinding[]) dep.GetPropertyValue(nameof(LazyConstructorBinding<TInterface>.Dependencies)));

                                return dependencyResolver.InlineDependenciesImpl(
                                    (Expression) (dep.GetPropertyValue(
                                        nameof(LazyConstructorBinding<TInterface>.ConstructionExpression))
                                                     .GetPropertyValue(nameof(Expression<Func<TInterface>>.Body))
                                                 )
                                    );
                            }
                        }
                    }
                }
                return base.VisitMethodCall(node);
            }
        }

        protected override TInterface ResolveImplementation()
        {
            return Constructor.Resolve();
        }
    }
}
