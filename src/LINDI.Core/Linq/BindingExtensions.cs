using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net.Http.Headers;
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
        private static IBinding<TInterface> Wrap<TInterface>(IBinding<TInterface> wrapper, IBinding<TInterface> wrapped)
        {
            var superBinding = wrapper as IFilterBinding<TInterface>;
            if (superBinding != null)
            {
                superBinding.SetBinding(wrapped);
                return wrapper;
            }
            return wrapped;
        } 

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

            LazyConstructorBinding<TInterface> b = new LazyConstructorBinding<TInterface>(dependencies, lazyExpression);
            return Wrap(binding, b);
        }

        public static IBinding<T> Where<T>(this IBinding<T> t, Func<object, bool> e)
        {
            return null;
        }

        /// <summary>
        /// Builds on the given binding by grouping newly created values by the values returned by the given <paramref name="valueSelector"/>.
        /// The <paramref name="valueSelector"/> is used to point to an ambient value that can change depending on the calling context.
        /// </summary>
        /// <param name="binding">The binding that the new binding should be wrapped inside.</param>
        /// <param name="valueSelector">A function that retrieves the ambient value that the resolved values should be grouped by.</param>
        /// <returns>A binding that is able to resolve the same value when the <paramref name="valueSelector"/> returns a same value.</returns>
        public static IBinding<TInterface> GroupBy<TInterface, TValue>(this IBinding<TInterface> binding, [NotNull] Func<TInterface, TValue> valueSelector)
            where TInterface : class
            where TValue : class
        {
            if (valueSelector == null) throw new ArgumentNullException(nameof(valueSelector));
            return Wrap(binding, new ValueScopedBinding<TInterface, TValue>(() => valueSelector(null)));
        }
    }
}