using System;
using JetBrains.Annotations;
using System.Runtime.CompilerServices;
using Lindi.Core.Bindings;
using Lindi.Core.Linq;

namespace Lindi.Core
{
    /// <summary>
    /// Defines an interface that represents a barebones binding.
    /// </summary>
    public interface IBinding
    {
        /// <summary>
        /// Resolves a value out of the binding.
        /// </summary>
        /// <returns>The value that was resolved.</returns>
        /// <exception cref="BindingResolutionException">
        /// Thrown if binding fails to resolve properly. 
        /// This SHOULD be the only catchable exception that is thrown from all instances of this method.
        /// </exception>
        object Resolve();

        /// <summary>
        /// Gets the <see cref="Type"/> of values that are resolved from this binding.
        /// </summary>
        Type BindingType { get; }
    }

    /// <summary>
    /// Defines an interface that represents a binding from an interface to a specified type.
    /// </summary>
    /// <typeparam name="TInterface">The type that is being bound to another type.</typeparam>
    public interface IBinding<out TInterface> : IBinding
    {
        /// <summary>
        /// Resolves a value out of the binding.
        /// </summary>
        /// <returns>The value that was resolved.</returns>
        /// <exception cref="BindingResolutionException">
        /// Thrown if binding fails to resolve properly. 
        /// This SHOULD be the only catchable exception that is thrown from all instances of this method.
        /// </exception>
        [CanBeNull]
        new TInterface Resolve();
    }

    /// <summary>
    /// Defines an interface that represents a binding who's actual binding resolution is determined by a "child" binding after the object is created.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This interface is used by the methods defined in <see cref="BindingExtensions"/> as a way to "wrap" values so that they
    /// can maintain the binding hierarchy that they are defined in.
    /// </para>
    /// 
    /// <example>
    /// This:
    /// <code>
    /// Bind{T}().GroupBy(type => otherValue).Select(type => new Type());
    /// </code>
    /// 
    /// Translates roughly to:
    /// 
    /// <code>
    /// new ReferenceScopedBinding(new ConstructorBinding(() => new Type()));
    /// </code>
    /// </example>
    /// </remarks>
    /// <typeparam name="TInterface">The type that is being bound to another type.</typeparam>
    public interface IDeferredBinding<TInterface> : IBinding<TInterface>
    {
        /// <summary>
        /// Sets the given binding for this deferred binding.
        /// </summary>
        /// <param name="binding">The binding that should be used for internal resolution.</param>
        void SetBinding([NotNull] IBinding<TInterface> binding);
    }
}
