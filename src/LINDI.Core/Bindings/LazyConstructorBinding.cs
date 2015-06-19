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
        public IEnumerable<IBinding> Dependencies { get; }

        /// <summary>
        /// Gets the expression that was passed to this binding to define the constructor.
        /// </summary>
        public Expression<Func<IBinding[], TInterface>> ConstructionExpression { get; }
        
        /// <summary>
        /// Gets whether the constructor represented by this binding has been built.
        /// </summary>
        public bool IsBuilt => constructor.IsValueCreated;

        /// <summary>
        /// Gets the internal binding that is built by this lazy binding.
        /// </summary>
        public IBindToConstructor<TInterface> Constructor => constructor.Value;

        private readonly Lazy<IBindToConstructor<TInterface>> constructor;

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
        public LazyConstructorBinding([NotNull] IEnumerable<IBinding> dependencies, [NotNull] Expression<Func<IBinding[], TInterface>> constructionExpression)
        {
            if (dependencies == null) throw new ArgumentNullException(nameof(dependencies));
            if (constructionExpression == null) throw new ArgumentNullException(nameof(constructionExpression));
            Dependencies = dependencies;
            ConstructionExpression = constructionExpression;
            constructor = new Lazy<IBindToConstructor<TInterface>>(BuildConstructor);
        }

        private IBindToConstructor<TInterface> BuildConstructor()
        {
            // Look through each dependency and inline it if possible
            var inliner = new DependencyInliner(Dependencies.ToArray());
            return new BindToConstructor<TInterface>(inliner.InlineDependencies(ConstructionExpression.Body).Compile());
        }
        
        private class DependencyInliner : ExpressionVisitor
        {
            IBinding[] dependencies;

            public DependencyInliner(IBinding[] dependencies)
            {
                this.dependencies = dependencies;
            }

            public Expression<Func<TInterface>> InlineDependencies(Expression expression)
            {
                return Expression.Lambda<Func<TInterface>>(Visit(expression));
            }

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                if (node.Method.Name == nameof(IBinding<TInterface>.Resolve) && typeof(IBinding).IsAssignableFrom(node.Method.DeclaringType))
                {
                    IndexExpression indexExpression = (IndexExpression) node.Object;
                    Type bindingType = indexExpression.Type;

                    if (bindingType.IsGenericType &&
                        bindingType.GetGenericTypeDefinition() == typeof (LazyConstructorBinding<>))
                    {
                        var dep = dependencies[indexExpression.Arguments[0].GetValueFromParameter<int>()];
                        var dependencyResolver = new DependencyInliner((IBinding[])dep.GetPropertyValue(nameof(LazyConstructorBinding<TInterface>.Dependencies)));

                        return dependencyResolver.InlineDependencies(
                            (Expression)(dep.GetPropertyValue(
                                nameof(LazyConstructorBinding<TInterface>.ConstructionExpression))
                                    .GetPropertyValue(nameof(Expression<Func<TInterface>>.Body))
                                    )
                                 ).Body;
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
