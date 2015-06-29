using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Fasterflect;
using JetBrains.Annotations;

namespace Lindi.Core.Bindings
{
    /// <summary>
    /// Defines a binding that is able to scope generated values by other "selected" values. (which happen to be value types)
    /// </summary>
    /// <remarks>
    /// <para>
    /// This binding scopes bound values by other values which are value types. That means that there is no reference to them, and therefore no tangable garbage collection
    /// to signal "boundaries" between new bound values for different selected values. 
    /// </para>
    /// <para>
    /// Unlike the <see cref="ReferenceScopedBinding{TInterface,TValue}"/>, the <see cref="ValueScopedBinding{TInterface,TValue}"/> stores values in a dictionary that contains
    /// hard references to both the resolved values and their respective selectors. This essentially means that every value you request from a binding of this type is automatically a
    /// singleton, "scoped" by whatever value was produced by the value selector.
    /// </para>
    /// </remarks>
    /// <typeparam name="TInterface">The type of objects that this binding is able to produce.</typeparam>
    /// <typeparam name="TValue">The type of values that generated objects should be scoped by.</typeparam>
    public class ValueScopedBinding<TInterface, TValue> : LazyConstructorBinding<TInterface>, IDeferredBinding<TInterface>
        where TInterface : class
        where TValue : struct
    {
        Func<TValue> valueSelector;
        Dictionary<TValue, TInterface> valueToImplementationMap = new Dictionary<TValue, TInterface>();

        public ValueScopedBinding([NotNull] Func<TValue> valueSelector) : base(new IBinding[0], bindings => null)
        {
            if (valueSelector == null) throw new ArgumentNullException(nameof(valueSelector));
            this.valueSelector = valueSelector;
        }

        public void SetBinding(IBinding<TInterface> binding)
        {
            if (binding == null) throw new ArgumentNullException(nameof(binding));

            var value = Expression.Variable(typeof(TValue), "value");
            var implementation = Expression.Variable(typeof(TInterface), "implementation");

            var resolve = Expression.Block(
                variables: new[] {value, implementation},
                expressions: new Expression[]
                {
                    Expression.Assign(value, Expression.Invoke(Expression.Constant(this.valueSelector))),

                    ExpressionHelpers.TryGetValueFromDictionaryAndAddValueIfItDoesNotExist(
                        valueToImplementationMap, 
                        value, 
                        implementation,
                        Expression.Call(Expression.Constant(binding), typeof(IBinding<TInterface>).Method(nameof(IBinding<TInterface>.Resolve)))),

                    implementation
                }
            );

            this.ConstructionExpression = Expression.Lambda<Func<IBinding[], TInterface>>(resolve, Expression.Parameter(typeof(IBinding[]), "bindings"));
        }
    }
}
