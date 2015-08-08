using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lindi.Core.Bindings;
using Xunit;
using static Lindi.Core.LindiMethods;

namespace Lindi.Tests.Core.Bindings
{
    /// <summary>
    /// Tests for <see cref="ReferenceScopedBinding{TInterface,TValue}"/>.
    /// </summary>
    public class ReferenceScopedBindingTests
    {
        [Fact]
        public void Test_Constructor_Throws_When_Given_Null_Value_Selector()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                ReferenceScopedBinding<object, object> binding = new ReferenceScopedBinding<object, object>(null);
            });
        }

        [Fact]
        public void Test_SetBinding_Throws_When_Given_Null_Binding()
        {
            ReferenceScopedBinding<IHasSample, ISample> binding = new ReferenceScopedBinding<IHasSample, ISample>(() => null);
            Assert.Throws<ArgumentNullException>(() =>
            {
                binding.SetBinding(null);
            });
        }

    }
}
