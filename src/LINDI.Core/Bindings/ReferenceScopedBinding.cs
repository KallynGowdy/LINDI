using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Fasterflect;
using JetBrains.Annotations;

namespace Lindi.Core.Bindings
{
    /// <summary>
    /// Defines a binding that provides scope functionality for reference types, grouped by a given selector function.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This binding provides the "normal" expected functionality for scoping. 
    /// When the given "value" selector produces a new value, a new object is resolved.
    /// When the value selector produces a value that has been seen before, the same object that was returned the first time will be returned.
    /// </para>
    /// <para>
    /// There are a couple of things to note about this approach however. 
    /// First, both the resolved values and the selected values are stored in a table with weak references. 
    /// This means that the values (both the resolved values and the selected values) are still allowed to be garbage collected. 
    /// This allows proper lifecycle management without infecting your code with hooks to see when a request is ending.
    /// So, unless you are somehow resurrecting values from the garbage collector and still needing them to be grouped, you should not have to worry about this fact.
    /// When you are done with a value and there are no active references to it anymore, the garbage collector will be able to pick it up. This binding will not stop that.
    /// 
    /// </para>
    /// <para>
    /// If you need a singleton, use <see cref="ValueScopedBinding{TInterface,TValue}"/>.
    /// </para>
    /// </remarks>
    /// <typeparam name="TInterface">The type of values that this binding resolves.</typeparam>
    /// <typeparam name="TValue">The type of the values that are "selected" and used to group resolved values.</typeparam>
    public class ReferenceScopedBinding<TInterface, TValue> : LazyConstructorBinding<TInterface>, IDeferredBinding<TInterface>
        where TInterface : class
        where TValue : class
    {
        Func<TValue> valueSelector;

        ConditionalWeakTable<TValue, TInterface> valueToImplementationMap = new ConditionalWeakTable<TValue, TInterface>();

        /// <summary>
        /// Creates a new <see cref="ReferenceScopedBinding{TInterface,TValue}"/>.
        /// </summary>
        /// <param name="valueSelector">The function that retrieves the values that the generated values should be scoped by.</param>
        public ReferenceScopedBinding([NotNull] Func<TValue> valueSelector) : base(new IBinding[0], bindings => null)
        {
            if (valueSelector == null) throw new ArgumentNullException(nameof(valueSelector));
            this.valueSelector = valueSelector;
        }

        /// <summary>
        /// Sets the given binding for this deferred binding.
        /// </summary>
        /// <param name="binding">The binding that should be used for internal resolution.</param>
        public void SetBinding(IBinding<TInterface> binding)
        {
            if (binding == null) throw new ArgumentNullException(nameof(binding));

            var value = Expression.Variable(typeof(TValue), "value");
            var implementation = Expression.Variable(typeof(TInterface), "implementation");
            
            var resolve = Expression.Block(
                // var value;
                // var implementation;
                variables: new[] { value, implementation },
                expressions: new Expression[]
                {
                    // value = valueSelector();
                    Expression.Assign(value, Expression.Invoke(Expression.Constant(this.valueSelector))),
                    
                    ExpressionHelpers.TryGetValueFromDictionaryAndAddValueIfItDoesNotExist(
                        this.valueToImplementationMap, 
                        value, 
                        implementation, 
                        Expression.Call(Expression.Constant(binding), typeof(IBinding<TInterface>).Method(nameof(IBinding<TInterface>.Resolve)))),

                    implementation
                });

            this.ConstructionExpression = Expression.Lambda<Func<IBinding[], TInterface>>(resolve, Expression.Parameter(typeof(IBinding[]), "bindings"));
        }
    }
}
