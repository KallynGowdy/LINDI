using System;

namespace Lindi.Core.Bindings
{
    /// <summary>
    /// Defines a class that provides a basic implementation of <see cref="IInjectValuesInto{TInjectedInto}"/>.
    /// </summary>
    /// <typeparam name="TInjectedInto">The type that the values should be injected into.</typeparam>
    public class InjectValuesInto<TInjectedInto> : IInjectValuesInto<TInjectedInto>
    {
        private Action<TInjectedInto> injectionFunction;

        /// <summary>
        /// Gets or sets the function that is used to inject values into a given object.
        /// Only can be set once.
        /// </summary>
        protected internal virtual Action<TInjectedInto> InjectionFunction
        {
            get { return injectionFunction; }
            set
            {
                if (injectionFunction == null)
                {
                    injectionFunction = value;
                }
            }
        }

        /// <summary>
        /// Injects the configured values into the given object.
        /// </summary>
        /// <param name="injectedInto">The object that the values should be injected into.</param>
        /// <exception cref="BindingResolutionException">Thrown if the values were not able to be injected into the given object.</exception>
        public void Inject(TInjectedInto injectedInto)
        {
            if (InjectionFunction == null)
            {
                throw new BindingResolutionException($"This injection binding must be finished before it can be used. That means {nameof(InjectionFunction)} must be set. Usually, it is set by calling Select() when configuring the injection.", GetType(), null);
            }
            try
            {
                InjectionFunction(injectedInto);
            }
            catch (Exception e)
            {
                if (e is BindingResolutionException)
                {
                    throw;
                }
                else
                {
                    throw new BindingResolutionException(this.GetType(), e);
                }
            }
        }
    }
}