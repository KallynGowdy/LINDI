using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LINDI.Core.Bindings
{
    /// <summary>
    /// Defines an interface that represents a binding from a given interface type to a given implementer type.
    /// </summary>
    /// <typeparam name="TInterface">The type of the interface that is being bound.</typeparam>
    /// <typeparam name="TImplementer">The type that is being bound to the interface as an implementer.</typeparam>
    public interface IBindToConstructor<TInterface, TImplementer>
        where TImplementer : TInterface
    {

    }
}
