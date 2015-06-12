using System;
using System.Linq.Expressions;
using System.Reflection;
using LINDI.Core.Bindings;

namespace LINDI.Core.Linq
{
    /// <summary>
    /// Defines extensions for <see cref="IBinding{TInterface}"/> objects that enable LINQ support.
    /// </summary>
    public static class BindingExtensions
    {
        public static IDependencyBinding<TInterface, TDep> Select<TInterface, TDep, TImplementer>(this IDependencyBinding<TInterface, TDep> bindings, Expression<Func<TDep, TImplementer>> expression)
        { 
            return null;
        }

        /// <summary>
        /// Finishes the binding between the given <see cref="IBinding{TInterface}"/> and the type resolved by the given expression.
        /// </summary>
        /// <param name="binding">The binding that should be finished.</param>
        /// <param name="expression">The expression that specifies </param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Invalid Expression. The given expression must be a NewExpression. That is, the calling of a constructor.</exception>
        public static IDependencyBinding<TInterface, TImplementer> Select<TInterface, TImplementer>(this IBinding<TInterface> binding, Expression<Func<TInterface, TImplementer>> expression)
        {
            return null;
        }
        

        public static IBinding<T> Where<T>(this IBinding<T> t, Func<object, bool> e)
        {
            return null;
        }

        public static IBinding<T> GroupBy<T>(this IBinding<T> t, Func<T, bool> e)
        {
            return null;
        }
    }
    

    public interface IDependencyBinding<TInterface, TDep> : IBinding<TInterface>
    {
    }
}