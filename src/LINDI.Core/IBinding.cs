using JetBrains.Annotations;
using LINDI.Core.Bindings;

namespace LINDI.Core
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
}
