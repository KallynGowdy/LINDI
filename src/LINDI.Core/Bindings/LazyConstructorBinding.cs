﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Fasterflect;
using JetBrains.Annotations;
using Lindi.Core.Attributes;

namespace Lindi.Core.Bindings
{
    /// <summary>
    /// Defines a binding that contains the ability to lazily build a <see cref="ConstructorBinding{TInterface}"/>
    /// binding based on the given dependencies and expression.
    /// </summary>
    /// <typeparam name="TInterface">The type of values that this binding can produce.</typeparam>
    public class LazyConstructorBinding<TInterface> : BaseBinding<TInterface>, ILazyConstructorBinding<TInterface>
    {
        private const string MustBeSetBeforeUseFormat = "{0} must be set before trying to build the final expression. Did you forget to call Select() at the end of setting up a binding?";

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
        public IConstructorBinding<TInterface> Constructor => constructor.Value;

        private readonly Lazy<IConstructorBinding<TInterface>> constructor;

        /// <summary>
        /// Creates a new <see cref="LazyConstructorBinding{TInterface}"/> that doesn't have any values.
        /// </summary>
        protected LazyConstructorBinding()
        {
            constructor = new Lazy<IConstructorBinding<TInterface>>(BuildConstructor);
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

        /// <summary>
        /// Creates a new <see cref="IConstructorBinding{TInterface}"/> from the expression and dependencies contained in this object.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        protected virtual IConstructorBinding<TInterface> BuildConstructor()
        {
            if (Dependencies == null) throw new InvalidOperationException(string.Format(MustBeSetBeforeUseFormat, nameof(Dependencies)));
            if (ConstructionExpression == null) throw new InvalidOperationException(string.Format(MustBeSetBeforeUseFormat, nameof(ConstructionExpression)));
            // Look through each dependency and inline it if possible
            var inliner = new DependencyInliner(Dependencies.ToArray());
            return new ConstructorBinding<TInterface>(inliner.InlineDependencies(ConstructionExpression.Body).Compile());
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

            /// <summary>
            /// Inlines all of the calls to methods decorated with the <see cref="DependencyMethodAttribute"/> that are contained in the given expression.
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public Expression<Func<TInterface>> InlineDependencies(Expression expression)
            {
                return Expression.Lambda<Func<TInterface>>(InlineDependenciesImpl(expression));
            }

            private Expression InlineDependenciesImpl(Expression expression)
            {
                return Visit(expression);
            }

            /// <summary>
            /// Visits the children of the <see cref="T:System.Linq.Expressions.MethodCallExpression"/>.
            /// </summary>
            /// <returns>
            /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
            /// </returns>
            /// <param name="node">The expression to visit.</param>
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
