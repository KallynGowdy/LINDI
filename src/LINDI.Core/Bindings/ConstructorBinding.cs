using System;
using JetBrains.Annotations;

namespace Lindi.Core.Bindings
{
    /// <summary>
    /// Defines a class that provides the default implementation of <see cref="IConstructorBinding{TInterface}"/>.
    /// </summary>
    /// <typeparam name="TInterface"></typeparam>
    public class ConstructorBinding<TInterface> : BaseBinding<TInterface>, IConstructorBinding<TInterface>
    {
        /// <summary>
        /// Resolves the value for the binding.
        /// If you are an implementer, implement this method how you would implement the <see cref="BaseBinding{TInterface}.Resolve"/> method.
        /// </summary>
        /// <returns>Returns the value that was resolved.</returns>
        protected override TInterface ResolveImplementation()
        {
            return Constructor();
        }

        /// <summary>
        /// Creates a new <see cref="ConstructorBinding{TInterface}"/> object that
        /// resolves values via calling the given constructor object.
        /// </summary>
        /// <param name="constructor">The function that resolves an instance of <typeparamref name="TInterface"/>.</param>
        public ConstructorBinding([NotNull] Func<TInterface> constructor)
        {
            if (constructor == null) throw new ArgumentNullException(nameof(constructor));
            Constructor = constructor;
        }

        /// <summary>
        /// Gets the function that is used as a constructor for new values.
        /// </summary>
        public Func<TInterface> Constructor { get; }
    }
}