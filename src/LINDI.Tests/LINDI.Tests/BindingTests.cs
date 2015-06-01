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
            Assert.IsType<IBindToConstructor<ISample, Sample>>(binding);
        }

    }
}
