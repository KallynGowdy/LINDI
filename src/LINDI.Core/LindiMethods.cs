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
        [BindMethod]
        public static IBinding<TInterface> Bind<TInterface>()
        {
            return null;
        }

        /// <summary>
        /// Represents the definition of a dependency on the given binding.
        /// This method returns the default value of the interface type. It is used
        /// as a stub method in expression to signal that the given binding should be used to resolve certain values.
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <param name="binding">The binding that should be used as a dependency for the parameter it is being passed as an argument for.</param>
        /// <returns>default(TInterface)</returns>
        [DependencyMethod]
        public static TInterface Dependency<TInterface>(IBinding<TInterface> binding)
        {
            return default(TInterface);
        }
    }
}
