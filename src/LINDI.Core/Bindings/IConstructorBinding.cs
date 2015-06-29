using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Lindi.Core.Bindings
{
    /// <summary>
    /// Defines an interface that represents a binding from a given interface type to a given implementer type.
    /// </summary>
    /// <typeparam name="TInterface">The type of the interface that is being bound.</typeparam>
    public interface IConstructorBinding<out TInterface> : IBinding<TInterface>
    {
        /// <summary>
        /// Gets the function that is used as a constructor for new values.
        /// </summary>
        [NotNull]
        Func<TInterface> Constructor { get; }
    }
}
