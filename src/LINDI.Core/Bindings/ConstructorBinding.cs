using System;
using JetBrains.Annotations;

namespace Lindi.Core.Bindings
{
    /// <summary>
    /// Defines a class that provides the default implementation of <see cref="IConstructorBinding{TInterface}"/>.
    /// </summary>
    /// <typeparam name="TInterface"></typeparam>
    /// <typeparam name="TImplementer"></typeparam>
    public class ConstructorBinding<TInterface> : BaseBinding<TInterface>, IConstructorBinding<TInterface>
    {
        protected override TInterface ResolveImplementation()
        {
            return Constructor();
        }

        /// <summary>
        /// Creates a new <see cref="ConstructorBinding{TInterface}"/> object that
        /// resolves values via calling the given constructor object.
        /// </summary>
        /// <param name="constructor">The function that resolves an instance of <typeparamref name="TImplementer"/>.</param>
        public ConstructorBinding([NotNull] Func<TInterface> constructor)
        {
            if (constructor == null) throw new ArgumentNullException(nameof(constructor));
            Constructor = constructor;
        }

        public Func<TInterface> Constructor { get; }
    }
}