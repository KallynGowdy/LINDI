﻿using System;
using JetBrains.Annotations;

namespace LINDI.Core.Bindings
{
    /// <summary>
    /// Defines a class that provides the default implementation of <see cref="IBindToConstructor{TInterface,TImplementer}"/>.
    /// </summary>
    /// <typeparam name="TInterface"></typeparam>
    /// <typeparam name="TImplementer"></typeparam>
    public class BindToConstructor<TInterface, TImplementer> : BaseBinding<TInterface>, IBindToConstructor<TInterface, TImplementer>
        where TImplementer : TInterface
    {
        protected override TInterface ResolveImplementation()
        {
            return Constructor();
        }

        /// <summary>
        /// Creates a new <see cref="BindToConstructor{TInterface,TImplementer}"/> object that
        /// resolves values via calling the given constructor object.
        /// </summary>
        /// <param name="constructor">The function that resolves an instance of <typeparamref name="TImplementer"/>.</param>
        public BindToConstructor([NotNull] Func<TImplementer> constructor)
        {
            if (constructor == null) throw new ArgumentNullException(nameof(constructor));
            Constructor = constructor;
        }

        public Func<TImplementer> Constructor { get; }
    }
}