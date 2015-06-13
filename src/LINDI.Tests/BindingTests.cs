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
    /// Tests verifying the binding behavior for types.
    /// </summary>
    public class BindingTests
    {
        [Fact]
        public void TestBindTypeToDefaultConstructorUsingLinqReturnsProperBindingType()
        {
            IBinding<ISample> binding = from type in Bind<ISample>()
                                        select new Sample();
            Assert.IsAssignableFrom<IBindToConstructor<ISample, Sample>>(binding);
        }

        [Fact]
        public void TestBindTypeToConstructorWithExtraBindingsUsingLinqReturnsProperBindingType()
        {
            IBinding<ISample> binding = from type in Bind<ISample>()
                                        select new Sample(default(object));
            Assert.IsAssignableFrom<IBindToConstructor<ISample, Sample>>(binding);
        }

        [Fact]
        public void TestBindTypeToConstructorWithGivenDataUsingLinqReturnsProperBindingType()
        {
            IBinding<ISample> binding = from type in Bind<ISample>()
                                        select new Sample("Hello, World");
            Assert.IsAssignableFrom<IBindToConstructor<ISample, Sample>>(binding);
        }
    }
}
