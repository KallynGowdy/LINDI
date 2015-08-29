using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lindi.Core;
using Lindi.Core.Bindings;

namespace Lindi.Ioc
{
    /// <summary>
    /// Defines a collection of <see cref="IBinding"/> objects.
    /// </summary>
    public interface IBindingCollection : IEnumerable<IBinding>
    {
        /// <summary>
        /// Gets the number of types that are resolvable by one or more bindings in this collection.
        /// </summary>
        int TypeCount { get; }

        /// <summary>
        /// Gets the total number of <see cref="IBinding"/> objects that are stored in this collection.
        /// </summary>
        int BindingCount { get; }

        /// <summary>
        /// Adds the given binding to the collection.
        /// </summary>
        /// <param name="binding">The binding that should be added.</param>
        void Add([NotNull] IBinding binding);

        /// <summary>
        /// Adds the given list of bindings to the collection.
        /// </summary>
        /// <param name="bindings">The bindings that should be added to the collection.</param>
        void AddRange([NotNull] IEnumerable<IBinding> bindings);

        /// <summary>
        /// Removes the given binding from the collection.
        /// </summary>
        /// <param name="binding">The binding that should be removed from the collection.</param>
        bool Remove([CanBeNull] IBinding binding);

        /// <summary>
        /// Resolves a object of the given type from this collection.
        /// </summary>
        /// <param name="type">The type of object that should be resolved.</param>
        /// <exception cref="BindingResolutionException">Thrown if a binding of the given type could not be found or if resolution of the binding failed.</exception>
        /// <returns>Returns the resolved value.</returns>
        [CanBeNull]
        object Resolve([NotNull] Type type);

        /// <summary>
        /// Resolves a object of the given type from this collection.
        /// </summary>
        /// <typeparam name="T">The type of object that should be resolved.</typeparam>
        /// <exception cref="BindingResolutionException">Thrown if a binding of the given type could not be found or if resolution of the binding failed.</exception>
        /// <returns>Returns the resolved value.</returns>
        [CanBeNull]
        T Resolve<T>();

        /// <summary>
        /// Resolves objects for all of the bindings for the given type.
        /// </summary>
        /// <param name="type">The type of objects that should be resolved.</param>
        /// <exception cref="BindingResolutionException">Thrown if a binding of the given type could not be found or if resolution of one of the bindings failed.</exception>
        /// <returns>Returns the resolve values.</returns>
        [CanBeNull]
        object[] ResolveAll([NotNull] Type type);

        /// <summary>
        /// Resolves objects for all of the bindings for the given type.
        /// </summary>
        /// <typeparam name="T">The type of objects that should be resolved.</typeparam>
        /// <exception cref="BindingResolutionException">Thrown if a binding of the given type could not be found or if resolution of one of the bindings failed.</exception>
        /// <returns>Returns the resolve values.</returns>
        [CanBeNull]
        T[] ResolveAll<T>();
    }
}
