﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Lindi.Core.Bindings;

namespace Lindi.Core.Linq
{
    /// <summary>
    /// Defines a class that contains extension methods for composing <see cref="IInjectValuesInto{TInjectedInto}"/> objects.
    /// </summary>
    public static class InjectExtensions
    {
        /// <summary>
        /// Finishes the injections for the given binding, using the injections defined in the given expression.
        /// </summary>
        /// <typeparam name="TInjectedInto">The type that the values are being injected into.</typeparam>
        /// <typeparam name="TInstance">The type of the instance that is being used as the mental vessel for injecting values into a <typeparamref name="TInjectedInto"/> value.</typeparam>
        /// <param name="binding">The type of the injection that should be finished</param>
        /// <param name="injectExpression">The expression that specifies how the values should injected.</param>
        /// <returns>Returns a completed <see cref="IInjectValuesInto{TInjectedInto}"/> object that can be used to inject values into objects.</returns>
        /// <remarks>
        /// 
        /// </remarks>
        public static IInjectValuesInto<TInjectedInto> Select<TInjectedInto, TInstance>(this IInjectValuesInto<TInjectedInto> binding, Expression<Func<TInjectedInto, TInstance>> injectExpression)
            where TInstance : TInjectedInto
        {
            return null;
        }

        public static IInjectValuesInto<TInjectedInto> Select<TInjectedInto>(this IInjectValuesInto<TInjectedInto> binding, Action<TInjectedInto> injectFunc)
        {
            return null;
        }

        /// <summary>
        /// Builds on the given injection by creating an injection that can inject values into a object specified by the given expression.
        /// </summary>
        /// <typeparam name="TInterface">The type of values that are being injected.</typeparam>
        /// <typeparam name="TInjectedInto">The type of objects that the values should be injected into.</typeparam>
        /// <param name="binding">The injection that should be used to resolve new values.</param>
        /// <param name="expression">An expression that identifies the type of objects that the injection applies to.</param>
        /// <returns>Returns a new <see cref="IInjectValuesInto{TInjectedInto}"/> object that has yet to be "completed."</returns>
        public static IInjectValuesInto<TInjectedInto> Where<TInterface, TInjectedInto>(this IBinding<TInterface> binding, Expression<Func<TInterface, TInjectedInto>> expression)
        {
            return new InjectValuesInto<TInjectedInto>();
        }
    }
}
