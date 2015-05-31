using System;
using System.Linq.Expressions;

namespace LINDI.Tests
{
    public static class Extensions
    {
        public static IBinding<T> Select<T1,T>(this IBinding<T1> t, Expression<Func<T1, T>>  e)
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