﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
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
        /// <param name="expression">The expression that specifies </param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Invalid Expression. The given expression must be a NewExpression. That is, the calling of a constructor.</exception>
        public static IBinding<TInterface> Select<TInterface, TImplementer>(this IBinding<TInterface> binding, Expression<Func<TInterface, TImplementer>> expression)
            where TImplementer : TInterface
        {
            NewExpression constructorExpression = expression.Body as NewExpression;

            if (constructorExpression != null)
            {
                // Check for dependencies in the constructor expression
                IBinding[] dependencies = constructorExpression.GetDependencies();

                if (dependencies.Length > 0)
                {
                    return new LazyConstructorBinding<TInterface>(dependencies, constructorExpression.ReplaceArguments<TInterface>(dependencies));
                }
                else
                {
                    return new BindToConstructor<TInterface, TImplementer>(Expression.Lambda<Func<TImplementer>>(constructorExpression).Compile());
                }
            }
            else
            {
                throw new ArgumentException("Invalid Expression. The given expression must be a NewExpression. That is, the calling of a constructor.", nameof(expression));
            }
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