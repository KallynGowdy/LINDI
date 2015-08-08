using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lindi.Core.Bindings;
using Lindi.Core.Linq;
using Lindi.Tests.Core.Linq;
using Xunit;
using static Lindi.Core.LindiMethods;

namespace Lindi.Tests.Core.Bindings
{
    /// <summary>
    /// Tests for <see cref="InjectValuesInto{TInjectedInto}"/>.
    /// </summary>
    public class InjectValuesIntoTests
    {
        [Fact]
        public void Test_Inject_Wraps_Thrown_Exceptions_In_BindingResolutionException()
        {
            var injection = Inject()
                .Where(value => InjectedInto(value) as INeedSample)
                .Select(value =>
                {
                    value.Sample = null;
                    throw new Exception();
                });

            Assert.Throws<BindingResolutionException>(() =>
            {
                INeedSample needSample = new NeedSample();
                injection.Inject(needSample);
            });
        }

        [Fact]
        public void Test_Inject_Does_Not_ReWrap_BindingResolutionException()
        {
            BindingResolutionException ex = new BindingResolutionException(typeof(INeedSample), new Exception());
            bool caught = false;
            var injection = Inject()
                .Where(value => InjectedInto(value) as INeedSample)
                .Select(value =>
                {
                    value.Sample = null;
                    throw ex;
                });

            try
            {
                INeedSample needSample = new NeedSample();
                injection.Inject(needSample);
            }
            catch (BindingResolutionException e)
            {
                Assert.Same(ex, e);
                caught = true;
            }

            Assert.True(caught);
        }
    }
}
