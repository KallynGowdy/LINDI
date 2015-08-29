using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lindi.Core.Bindings
{
    /// <summary>
    /// Defines a <see cref="IBinding{TInterface}"/> that provides base functionality like wrapping all resolution exceptions in
    /// <see cref="BindingResolutionException"/>.
    /// </summary>
    public abstract class BaseBinding<TInterface> : IBinding<TInterface>
    {
        /// <summary>
        /// Resolves a value out of the binding.
        /// </summary>
        /// <returns>The value that was resolved.</returns>
        /// <exception cref="BindingResolutionException">
        /// Thrown if binding fails to resolve properly. 
        /// This SHOULD be the only catchable exception that is thrown from all instances of this method.
        /// </exception>
        public TInterface Resolve()
        {
            try
            {
                return ResolveImplementation();
            }
            catch (Exception e)
            {
                throw new BindingResolutionException(GetType(), e);
            }
        }

        /// <summary>
        /// Gets the <see cref="Type"/> of values that are resolved from this binding.
        /// </summary>
        public Type BindingType => typeof(TInterface);

        /// <summary>
        /// Resolves the value for the binding.
        /// If you are an implementer, implement this method how you would implement the <see cref="Resolve()"/> method.
        /// </summary>
        /// <returns>Returns the value that was resolved.</returns>
        protected abstract TInterface ResolveImplementation();

        /// <summary>
        /// Resolves a value out of the binding.
        /// </summary>
        /// <returns>The value that was resolved.</returns>
        /// <exception cref="BindingResolutionException">
        /// Thrown if binding fails to resolve properly. 
        /// This SHOULD be the only catchable exception that is thrown from all instances of this method.
        /// </exception>
        object IBinding.Resolve()
        {
            return Resolve();
        }
    }
}
