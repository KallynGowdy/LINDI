using JetBrains.Annotations;
using Lindi.Core.Bindings;

namespace Lindi.Core
{
    /// <summary>
    /// Defines an interface that wraps a binding and is able to inject values into a given object of a given type.
    /// </summary>
    /// <typeparam name="TInjectedInto">The type of objects that the values are being injected into.</typeparam>
    public interface IInjectValuesInto<in TInjectedInto>
    {
        /// <summary>
        /// Injects the configured values into the given object.
        /// </summary>
        /// <param name="injectedInto">The object that the values should be injected into.</param>
        /// <exception cref="BindingResolutionException">Thrown if the values were not able to be injected into the given object.</exception>
        void Inject([NotNull] TInjectedInto injectedInto);
    }

    /// <summary>
    /// Defines an interface that wraps a binding and is able to inject values into a given object of a given type.
    /// </summary>
    /// <typeparam name="TInterface">The type of values that are being injected into the given objects.</typeparam>
    /// <typeparam name="TInjectedInto">The type of object that the values will be injected into.</typeparam>
    public interface IInjectValuesInto<out TInterface, in TInjectedInto> : IInjectValuesInto<TInjectedInto>
    {
        /// <summary>
        /// Gets the binding that resolves the values.
        /// </summary>
        [NotNull]
        IBinding<TInterface> ValueResolver { get; }
    }
}