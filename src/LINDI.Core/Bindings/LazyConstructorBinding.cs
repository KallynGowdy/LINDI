using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Lindi.Core.Bindings
{
    /// <summary>
    /// Defines a binding that contains the ability to lazily build a <see cref="BindToConstructor{TInterface,TImplementer}"/>
    /// binding based on the given dependencies and expression.
    /// </summary>
    /// <typeparam name="TInterface">The type of values that this binding can produce.</typeparam>
    public class LazyConstructorBinding<TInterface> : BaseBinding<TInterface>
    {
        /// <summary>
        /// Creates a new <see cref="LazyConstructorBinding{TInterface}"/> from the given dependencies and construction epression.
        /// </summary>
        /// <param name="dependencies">The array of bindings that represent dependencies that the constructor needs.</param>
        /// <param name="constructionExpression">
        /// An expression that specifies the constructor that should be called and also identifies the paring
        /// between a binding and constructor parameter.
        /// </param>
        public LazyConstructorBinding([NotNull] IEnumerable<IBinding> dependencies, [NotNull] Expression<Func<IBinding[], TInterface>> constructionExpression)
        {
            if (dependencies == null) throw new ArgumentNullException(nameof(dependencies));
            if (constructionExpression == null) throw new ArgumentNullException(nameof(constructionExpression));
        }

        protected override TInterface ResolveImplementation()
        {
            throw new NotImplementedException();
        }
    }
}
