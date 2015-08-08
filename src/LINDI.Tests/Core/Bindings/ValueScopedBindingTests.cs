using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lindi.Core.Bindings;
using Xunit;

namespace Lindi.Tests.Core.Bindings
{
    /// <summary>
    /// Tests for <see cref="ValueScopedBinding{TInterface,TValue}"/>.
    /// </summary>
    public class ValueScopedBindingTests
    {
        [Fact]
        public void Test_Constructor_Throws_When_Given_Null_Value_Selector()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                ValueScopedBinding<object, bool> binding = new ValueScopedBinding<object, bool>(null);
            });
        }

        [Fact]
        public void Test_SetBinding_Throws_When_Given_Null_Binding()
        {
            ValueScopedBinding<IHasSample, bool> binding = new ValueScopedBinding<IHasSample, bool>(() => true);
            Assert.Throws<ArgumentNullException>(() =>
            {
                binding.SetBinding(null);
            });
        }
    }
}
