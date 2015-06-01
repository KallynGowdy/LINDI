using System;
using System.Linq.Expressions;

namespace LINDI.Core.Linq
{
    /// <summary>
    /// Defines extensions for <see cref="IBinding{TInterface}"/> objects that enable LINQ support.
    /// </summary>
    public static class BindingExtensions
    {
        /// <summary>
        /// Finishes the binding between the given <see cref="IBinding{TInterface}"/> and the type resolved by the given expression.
        /// </summary>
        /// <param name="binding">The binding that should be finished.</param>
        /// <param name="expression">The expression that specifies </param>
        /// <returns></returns>
        public static IBinding<TInterface> Select<TInterface, TImplementer>(this IBinding<TInterface> binding, Expression<Func<TInterface, TImplementer>>  expression)
            where TImplementer : TInterface
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
}