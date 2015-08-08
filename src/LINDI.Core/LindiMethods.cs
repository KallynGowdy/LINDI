using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lindi.Core.Attributes;
using Lindi.Core.Linq;

namespace Lindi.Core
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
        /// Returns a null binding for the given interface type.
        /// Different syntax that is mostly used for setting up injections.
        /// </summary>
        /// <typeparam name="TInterface">The type that should be bound to a type that will be determined by the future operations applied to it.</typeparam>
        /// <returns>Null</returns>
        [BindMethod]
        public static IBinding<object> Inject()
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
        /// <exception cref="ArgumentNullException"><paramref name="binding"/> is <see langword="null" />.</exception>
        [DependencyMethod]
        [ExcludeFromCodeCoverage]
        public static TInterface AsDependency<TInterface>([NotNull] this IBinding<TInterface> binding)
        {
            if (binding == null) throw new ArgumentNullException(nameof(binding));
            return default(TInterface);
        }

        /// <summary>
        /// Represents the definition of a dependency on the given binding.
        /// This method returns the default value of the interface type. It is used
        /// as a stub method in expression to signal that the given binding should be used to resolve certain values.
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <param name="binding">The binding that should be used as a dependency for the parameter it is being passed as an argument for.</param>
        /// <returns>default(TInterface)</returns>
        /// <exception cref="ArgumentNullException"><paramref name="binding"/> is <see langword="null" />.</exception>
        [DependencyMethod]
        [ExcludeFromCodeCoverage]
        public static TInterface Dependency<TInterface>([NotNull] IBinding<TInterface> binding)
        {
            if (binding == null) throw new ArgumentNullException(nameof(binding));
            return default(TInterface);
        }

        [ExcludeFromCodeCoverage]
        public static object InjectedInto<TInterface>(TInterface value)
        {
            return null;
        }

        [ExcludeFromCodeCoverage]
        public static TInjectedInto IsInjectedInto<TInjectedInto>()
        {
            return default(TInjectedInto);
        }

        /// <summary>
        /// Returns the current thread. Acts as a "shortcut" method for referencing the current thread for a scope.
        /// </summary>
        /// <returns><value>System.Threading.Thread.CurrentThread</value></returns>
        public static Thread Thread()
        {
            return System.Threading.Thread.CurrentThread;
        }

        /// <summary>
        /// Returns true. Acts as a "shortcut" method for putting a value into a singleton scope.
        /// </summary>
        /// <returns><value>true</value></returns>
        public static bool Singleton()
        {
            return true;
        }
    }
}
