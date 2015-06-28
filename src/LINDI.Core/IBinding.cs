using JetBrains.Annotations;
using Lindi.Core.Bindings;

namespace Lindi.Core
{
    /// <summary>
    /// Defines an interface that represents a barebones binding.
    /// </summary>
    public interface IBinding
    {
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
        TInterface Resolve();
    }

    /// <summary>
    /// Defines an interface that represents a binding that can contain and delegate resolution to other bindings.
    /// </summary>
    /// <typeparam name="TInterface"></typeparam>
    public interface IFilterBinding<TInterface> : IBinding<TInterface>
    {
        /// <summary>
        /// Adds the given binding to this binding.
        /// </summary>
        /// <param name="binding"></param>
        void SetBinding([NotNull] IBinding<TInterface> binding);
    }
}
