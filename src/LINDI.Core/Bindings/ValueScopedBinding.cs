using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using JetBrains.Annotations;

namespace Lindi.Core.Bindings
{
    /// <summary>
    /// Defines a super binding that provides scope functionality by a given selector function.
    /// </summary>
    public class ValueScopedBinding<TInterface, TValue> : LazyConstructorBinding<TInterface>, IFilterBinding<TInterface>
        where TInterface : class
        where TValue : class
    {
        Func<TValue> valueSelector;

        ConditionalWeakTable<TValue, TInterface> valueToImplementationMap = new ConditionalWeakTable<TValue, TInterface>();

        public ValueScopedBinding([NotNull] Func<TValue> valueSelector) : base(new IBinding[0], bindings => null)
        {
            if (valueSelector == null) throw new ArgumentNullException(nameof(valueSelector));
            this.valueSelector = valueSelector;
        }

        public void SetBinding(IBinding<TInterface> binding)
        {
            if (binding == null) throw new ArgumentNullException(nameof(binding));

            Expression<Func<TInterface>> b = () => binding.Resolve();

            var value = Expression.Variable(typeof(TValue), "value");
            var implementation = Expression.Variable(typeof(TInterface), "implementation");

            ConstantExpression valueToImplementationMap = Expression.Constant(this.valueToImplementationMap);

            // valueToImplementationMap.TryGetValue(value, out implementation)
            var tryGetValueFromMap = Expression.Call(
                valueToImplementationMap,
                typeof(ConditionalWeakTable<TValue, TInterface>).GetMethod(nameof(ConditionalWeakTable<TValue, TInterface>.TryGetValue)),
                value, implementation
            );

            // if(!tryGetValueFromMap)
            UnaryExpression notTryGetValueFromMap = Expression.Not(tryGetValueFromMap);

            var resolve = Expression.Block(
                // var value;
                // var implementation;
                variables: new[] { value, implementation },
                expressions: new Expression[]
                {
                    // value = valueSelector();
                    Expression.Assign(value, Expression.Invoke(Expression.Constant(this.valueSelector))),

                    // if(notTryGetValueFromMap)
                    Expression.IfThen(notTryGetValueFromMap, 
                        // lock(valueToImplementationMap)
                        ExpressionHelpers.Lock(valueToImplementationMap, 
                            Expression.Block(
                                // if(notTryGetValueFromMap)
                                Expression.IfThen(notTryGetValueFromMap, 
                                    Expression.Block(
                                        // implementation = binding.Resolve();
                                        Expression.Assign(implementation, Expression.Call(Expression.Constant(binding), ((Func<TInterface>)binding.Resolve).Method)),

                                        // valueToImplementationMap.Add(value, implementation);
                                        Expression.Call(valueToImplementationMap, ((Action<TValue, TInterface>)this.valueToImplementationMap.Add).Method, value, implementation)
                                    )
                                )
                            )
                        )
                    ),
                    implementation
                });


            this.ConstructionExpression = Expression.Lambda<Func<IBinding[], TInterface>>(resolve, Expression.Parameter(typeof(IBinding[]), "binding"));
        }
    }
}
