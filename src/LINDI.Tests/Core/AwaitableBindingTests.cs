using System;
using Lindi.Core;
using Lindi.Core.Linq;
using Xunit;

namespace Lindi.Tests.Core
{
    public class AwaitableBindingTests
    {

        [Fact]
        public async void Test_Binding_Is_Awaitable()
        {
            var binding = from value in LindiMethods.Bind<ISample>()
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
