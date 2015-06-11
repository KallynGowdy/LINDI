using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LINDI.Core.Bindings
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
        /// <returns>The value that should be resolved.</returns>
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

        protected abstract TInterface ResolveImplementation();
    }
}
