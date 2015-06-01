using System;

namespace LINDI.Core.Bindings
{
    /// <summary>
    /// Defines a class that provides the default implementation of <see cref="IBindToConstructor{TInterface,TImplementer}"/>.
    /// </summary>
    /// <typeparam name="TInterface"></typeparam>
    /// <typeparam name="TImplementer"></typeparam>
    public class BindToConstructor<TInterface, TImplementer> : IBindToConstructor<TInterface, TImplementer>
        where TImplementer : TInterface
    {
        public TInterface Resolve()
        {
            return Constructor();
        }

        public BindToConstructor(Func<TImplementer> constructor)
        {
            if (constructor == null) throw new ArgumentNullException(nameof(constructor));
            Constructor = constructor;
        }

        public Func<TImplementer> Constructor { get; }
    }
}