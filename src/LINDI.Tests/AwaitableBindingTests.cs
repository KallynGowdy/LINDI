using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lindi.Core;
using Lindi.Core.Linq;
using Xunit;
using Xunit.Abstractions;
using static Lindi.Core.LindiMethods;

namespace Lindi.Tests
{
    public class AwaitableBindingTests
    {

        [Fact]
        public async void Test_Binding_Is_Awaitable()
        {
            var binding = from value in Bind<ISample>()
                          select new Sample();

            var v = await binding;

            Assert.NotNull(v);
            Assert.IsType<Sample>(v);
        }

        [Fact]
        public async void Test_Get_Awaiter_On_Null_Binding_Throws_ArgumentNullException()
        {
            IBinding<ISample> binding = null;

            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                var v = await binding;
            });
        }
    }
}
