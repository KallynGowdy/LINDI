using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using JetBrains.Annotations;

namespace Lindi.Core.Bindings
{
    /// <summary>
    /// Defines a binding that contains the ability to lazily build a <see cref="ConstructorBinding{TInterface}"/>
    /// binding based on the given dependencies and expression.
    /// </summary>
    /// <typeparam name="TInterface">The type of values that this binding can produce.</typeparam>
    public interface ILazyConstructorBinding<TInterface> : IBinding<TInterface>
    {
        /// <summary>
        /// Gets the list of bindings that represent the dependencies of this binding.
        /// </summary>
        [NotNull]
        IEnumerable<IBinding> Dependencies { get; }

        /// <summary>
        /// Gets the expression that was passed to this binding to define the constructor.
        /// </summary>
        [NotNull]
        Expression<Func<IBinding[], TInterface>> ConstructionExpression { get; }

        /// <summary>
        /// Gets whether the constructor represented by this binding has been built.
        /// </summary>
        bool IsBuilt { get; }

        /// <summary>
        /// Gets the internal binding that is built by this lazy binding.
        /// </summary>
        [NotNull]
        IConstructorBinding<TInterface> Constructor { get; }
    }
}