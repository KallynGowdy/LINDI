using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LINDI.Core
{
    /// <summary>
    /// Defines a static class that contains helpers for <see cref="Expression"/> objects.
    /// </summary>
    public static class ExpressionHelpers
    {
        /// <summary>
        /// Replaces the arguments of the given <see cref="NewExpression"/> with the defined replacements.
        /// </summary>
        /// <param name="expression">The expression that the values should be replaced in.</param>
        /// <param name="dependencies">The list of bindings that should replace the dependencies.</param>
        /// <returns></returns>
        public static Expression<Func<IBinding[], TInterface>>  ReplaceArguments<TInterface>(this NewExpression expression, IBinding[] dependencies)
        {
            ParameterExpression bindings = Expression.Parameter(typeof(IBinding[]), "b");

            List<Expression> parameters = new List<Expression>(expression.Arguments.Count);
            int i = 0;
            foreach (var argument in expression.Arguments)
            {
                IBinding b;
                if (argument.IsDependencyExpression(out b))
                {
                    Type bindingType = b.GetType().GetInterfaces().First(@interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IBinding<>));

                    // b[i]
                    BinaryExpression index = Expression.ArrayIndex(bindings, Expression.Constant(i++));

                    // (IBinding<T>)b[i]
                    UnaryExpression cast = Expression.Convert(index, bindingType);

                    // ((IBinding<T>)b[i]).Resolve()
                    MethodCallExpression call = Expression.Call(cast, bindingType.GetMethod("Resolve"));

                    parameters.Add(call);
                }
                else
                {
                    parameters.Add(argument);
                }
            }

            // new TImplementer(params)
            var newConstructor = Expression.New(expression.Constructor, parameters);

            // (TInterface)new TImplementer(params)
            var constructionCast = Expression.Convert(newConstructor, typeof(TInterface));

            // b => (TInterface)new TImplementer(params)
            return Expression.Lambda<Func<IBinding[], TInterface>>(constructionCast, bindings);
        }

        /// <summary>
        /// Gets the list of dependencies that are defined in the given constructor.
        /// </summary>
        /// <param name="constructorExpression">The expression that the dependencies are defined in.</param>
        /// <returns>Returns the list of bindings that are in the given constructor expression.</returns>
        public static IBinding[] GetDependencies(this NewExpression constructorExpression)
        {
            List<IBinding> bindings = new List<IBinding>();
            foreach (Expression argument in constructorExpression.Arguments)
            {
                IBinding b;
                if (argument.IsDependencyExpression(out b))
                    bindings.Add(b);
            }
            return bindings.ToArray();
        }

        /// <summary>
        /// Determines if the given expression represents the definition of a binding as a dependency.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static bool IsDependencyExpression(this Expression expression, out IBinding dependency)
        {
            if (expression.NodeType == ExpressionType.Call)
            {
                MethodCallExpression methodCallExpression = (MethodCallExpression)expression;
                MethodInfo method = methodCallExpression.Method;
                DependencyMethodAttribute attribute = method.GetCustomAttribute<DependencyMethodAttribute>();

                if (attribute != null)
                {
                    dependency = methodCallExpression.Arguments.First().GetValueFromParameter<IBinding>();
                    return true;
                }
            }
            dependency = null;
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static T GetValueFromParameter<T>(this Expression expression)
        {
            var conversion = Expression.Convert(expression, typeof(T));
            return Expression.Lambda<Func<T>>(conversion).Compile()();
        }
    }
}
