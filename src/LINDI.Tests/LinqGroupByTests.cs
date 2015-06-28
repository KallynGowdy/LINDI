using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lindi.Core;
using Lindi.Core.Bindings;
using Lindi.Core.Linq;
using Xunit;
using static Lindi.Core.LindiMethods;

namespace Lindi.Tests
{
    /// <summary>
    /// Tests for <see cref="BindingExtensions.GroupBy"/>.
    /// </summary>
    public class LinqGroupByTests
    {
        public static object Value { get; } = new object();
        public static object OtherValue { get; }

        [Fact]
        public void Test_Grouped_Binding_Creates_New_Scoped_Binding_For_Given_Value()
        {
            IBinding<ISample> binding = from type in Bind<ISample>()
                                        group type by Value into scope
                                        select new Sample();

            Assert.NotNull(binding);
            Assert.IsType<ValueScopedBinding<ISample, object>>(binding);
        }

        [Fact]
        public void Test_Grouped_Binding_Can_Be_Resolved()
        {
            Sample expectedValue = new Sample();
            IBinding<ISample> binding = from type in Bind<ISample>()
                                        group type by Value into scope
                                        select expectedValue;

            ISample value = binding.Resolve();

            Assert.Same(expectedValue, value);
        }

        [Fact]
        public void Test_Grouped_Binding_Reuses_Value()
        {
            IBinding<ISample> binding = from type in Bind<ISample>()
                                        group type by Value into scope
                                        select new Sample();

            ISample value = binding.Resolve();

            ISample secondValue = binding.Resolve();

            Assert.Same(value, secondValue);
        }

        [Fact]
        public void Test_GroupBy_Throws_ArgumentNullException_WhenGivenNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                IBinding<ISample> binding = Bind<ISample>().GroupBy<ISample, object>(null).Select(scope => new Sample());
            });
        }
    }
}
