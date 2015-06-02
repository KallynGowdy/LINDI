using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LINDI.Core;
using LINDI.Core.Bindings;
using LINDI.Core.Linq;
using Xunit;
using static LINDI.Core.LindiMethods;

namespace LINDI.Tests
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
