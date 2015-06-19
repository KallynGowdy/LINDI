using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;
using Lindi.Core.Bindings;

namespace Lindi.Core.Linq
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
        /// <param name="expression">
        /// The expression that specifies how the value is retrieved.
        /// This is generally a <see cref="NewExpression"/>, however anything that can be translated into a <see cref="Func{TImplementer}"/> can be used.
        /// </param>
        /// <example>
        /// Declare a binding to a constructor-like function:
        /// 
        /// <code>
        /// IBinding<TInterface> finalBinding = b.Select(type => new TImplementer());
        /// IBinding<TInterface> finalBinding = from type in b select new TImplementer();
        /// </code>
        /// </example>
        /// <returns>
        /// Returns a new <see cref="IBinding{TInterface}"/> that is able to resolve a value that implements the specified type when <see cref="IBinding{TInterface}.Resolve"/> is called.
        /// </returns>
        /// <exception cref="ArgumentException">Invalid Expression. The given expression must be a NewExpression. That is, the calling of a constructor.</exception>
        public static IBinding<TInterface> Select<TInterface, TImplementer>(this IBinding<TInterface> binding, [NotNull] Expression<Func<TInterface, TImplementer>> expression)
            where TImplementer : TInterface
        {
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            Expression constructorExpression = expression.Body;

            IBinding[] dependencies;
            Expression<Func<IBinding[], TInterface>> lazyExpression = constructorExpression.BuildLazyBindingExpression<TInterface>(out dependencies);

            return new LazyConstructorBinding<TInterface>(dependencies, lazyExpression);
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