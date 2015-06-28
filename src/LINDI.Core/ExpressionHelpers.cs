using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Fasterflect;
using JetBrains.Annotations;
using Lindi.Core.Attributes;
using Lindi.Core.Bindings;

namespace Lindi.Core
{
    /// <summary>
    /// Defines a static class that contains helpers for <see cref="Expression"/> objects.
    /// </summary>
    public static class ExpressionHelpers
    {
        /// <summary>
        /// Defines a class that is used to build expressions that can be used with the <see cref="LazyConstructorBinding{TInterface}"/>.
        /// </summary>
        /// <remarks>
        /// This class is not thread safe.
        /// </remarks>
        private class LazyConstructorExpressionBuilder : ExpressionVisitor
        {
            private readonly List<IBinding> bindingDependencies = new List<IBinding>();
            private readonly ParameterExpression parameter = Expression.Parameter(typeof(IBinding[]), "b");

            /// <summary>
            /// Gets the list of bindings that are called in methods annotated with the <see cref="DependencyMethodAttribute"/>
            /// in the given expression.
            /// </summary>
            /// <param name="expression"></param>
            /// <param name="dependencies"></param>
            /// <returns></returns>
            public Expression<Func<IBinding[], TInterface>> FindAndReplaceDependencies<TInterface>(Expression expression, out IBinding[] dependencies)
            {
                bindingDependencies.Clear();
                Expression<Func<IBinding[], TInterface>> expr = Expression.Lambda<Func<IBinding[], TInterface>>(Visit(expression), parameter);
                dependencies = bindingDependencies.ToArray();
                return expr;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                throw new ArgumentException($"(ParamName: {node.Name}) Parameters from the given expression are not allowed. To pass in a value, make sure it is from an external source and is therefore captured as a constant expression", nameof(node));
            }

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                if (IsDependencyMethod(node))
                {
                    IBinding binding = node.Arguments.First().GetValueFromParameter<IBinding>();
                    if (binding == null) throw new ArgumentNullException(nameof(binding), "Null binding dependencies are not allowed, if you want to represent a null value, just pass in `null`, not `Dependency(null)`");
                    bindingDependencies.Add(binding);

                    Type bindingType = binding.GetType().GetInterfaces().First(@interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IBinding<>));

                    // b[i]
                    BinaryExpression index = Expression.ArrayIndex(parameter, Expression.Constant(bindingDependencies.Count - 1));

                    // (IBinding<T>)b[i]
                    UnaryExpression cast = Expression.Convert(index, bindingType);

                    // ((IBinding<T>)b[i]).Resolve()
                    return Expression.Call(cast, bindingType.GetMethod("Resolve"));
                }
                return base.VisitMethodCall(node);
            }

            private static bool IsDependencyMethod(MethodCallExpression node)
            {
                return node.Method.GetCustomAttributes(typeof(DependencyMethodAttribute), false).Any();
            }
        }

        /// <summary>
        /// Replaces the arguments of the given <see cref="NewExpression"/> with the defined replacements.
        /// </summary>
        /// <param name="expression">The expression that the values should be replaced in.</param>
        /// <remarks>
        /// <para>
        /// In LINDI's case, "lazy" expressions are essentially pre-processed versions
        /// of expressions that have been given to the LINDI expression engine.
        /// These pre-processed versions replace the stubbed methods with calls to their binders and generally
        /// prepare for being processed further.
        /// </para>
        /// <para>
        /// Because a <see cref="LazyConstructorBinding{TInterface}"/> takes a list
        /// of dependent <see cref="IBinding"/> objects and a function that utilizes those bindings,
        /// we need to prepare it so that the stub methods are not actually called, but that the binding
        /// is referenced directly.
        /// </para>
        /// <para>
        /// This method is thread-safe given that <paramref name="expression"/> and <paramref name="dependentBindings"/> are not used across multiple threads.
        /// </para>
        /// </remarks>
        public static Expression<Func<IBinding[], TInterface>> BuildLazyBindingExpression<TInterface>(this Expression expression, out IBinding[] dependentBindings)
        {
            LazyConstructorExpressionBuilder visitor = new LazyConstructorExpressionBuilder();
            return visitor.FindAndReplaceDependencies<TInterface>(expression, out dependentBindings);
        }

        /// <summary>
        /// Gets the value that the given expression resolves to.
        /// </summary>
        /// <typeparam name="T">The expected type of the returned value.</typeparam>
        /// <param name="expression">The expression that should be reduced to a value.</param>
        /// <returns>Returns the value that was resolved from the expression.</returns>
        public static T GetValueFromParameter<T>(this Expression expression)
        {
            var conversion = Expression.Convert(expression, typeof(T));
            return Expression.Lambda<Func<T>>(conversion).Compile()();
        }

        /// <summary>
        /// Creates a new expression that represents a 'lock' expression, using the given <paramref name="lockObj"/> and <paramref name="body"/>.
        /// </summary>
        /// <param name="lockObj">An expression that resolves to a value that should be used for locking.</param>
        /// <param name="body">The body that represents the body of the lock expression.</param>
        /// <remarks>
        /// This method generates a 'lock' statement, using the method detailed at http://stackoverflow.com/a/6029829/1832856.
        /// The returned expression uses <see cref="Monitor.Enter(object,ref bool)"/> and <see cref="Monitor.Exit"/> to manage the lock state
        /// for the given <paramref name="lockObj"/>.
        /// </remarks>
        /// <returns>Returns an expression that represents the lock statement.</returns>
        public static Expression Lock([NotNull] Expression lockObj, [NotNull] Expression body)
        {
            if (lockObj == null) throw new ArgumentNullException(nameof(lockObj));
            if (body == null) throw new ArgumentNullException(nameof(body));

            ParameterExpression lockWasTaken = Expression.Variable(typeof(bool), "lockWasTaken");
            ParameterExpression temp = Expression.Variable(lockObj.Type, "temp");

            return Expression.Block(
                // bool lockWasTaken;
                // var temp = lockObj;
                variables: new[] { lockWasTaken, temp },
                expressions: new Expression[] {
                    Expression.Assign(temp, lockObj),
                    // try {
                    Expression.TryFinally(
                        Expression.Block(
                            // Monitor.Enter(temp, ref lockWasTaken);
                            Expression.Call(typeof(Monitor).Method("Enter", new[] { typeof(object), typeof(bool).MakeByRefType() }, Flags.Static | Flags.Public), temp, lockWasTaken),
                            // body
                            body
                        ),
                        // } finally {
                        Expression.Block(
                            // if(lockWasTaken) Monitor.Exit(lockObj);
                            Expression.IfThen(lockWasTaken, Expression.Call(((Action<object>)Monitor.Exit).Method, temp))
                        )
                    // }
                    )
                }
            );
        }
    }
}
