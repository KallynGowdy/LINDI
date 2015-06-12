using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LINDI.Core.Linq;

namespace LINDI.Core
{
    public static class LindiMethods
    {
        /// <summary>
        /// Returns a null binding for the given interface type.
        /// The reason why this returns null is because it is a type-hack. It allows the Select, Where, Group By, etc. extension methods
        /// to operate on it without actually containing any logic.
        /// </summary>
        /// <typeparam name="TInterface">The type that should be bound to a type that will be determined by the future operations applied to it.</typeparam>
        /// <returns>Null</returns>
        public static IBinding<TInterface> Bind<TInterface>()
        {
            // TODO
            return null;
        }

        public static TInterface DependencyUsing<TInterface>(IBinding<TInterface> binding)
        {
            return default(TInterface);
        }

        public static TInterface UsesDependency<TInterface>(IBinding<TInterface> binding)
        {
            return default(TInterface);
        }
        
    }
}
