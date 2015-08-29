using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lindi.Core;
using Lindi.Core.Bindings;
using Xunit;

namespace Lindi.Tests.Core.Bindings
{
    class MockBaseBinding<TInterafce> : BaseBinding<TInterafce>
    {
        protected override TInterafce ResolveImplementation()
        {
            throw new Exception();
        }
    }

    /// <summary>
    /// Tests for <see cref="BaseBinding{TInterface}"/>.
    /// </summary>
    public class BaseBindingTests
    {
        [Fact]
        public void Test_BaseBinding_Wraps_Exception_In_BindingResolutionException()
        {
            MockBaseBinding<ISample> binding = new MockBaseBinding<ISample>();

            Assert.Throws<BindingResolutionException>(() =>
            {
                var sample = binding.Resolve();
            });
        }

        [Fact]
        public void Test_Non_Generic_Resolve_Calls_Generic_Resolve()
        {
            IBinding binding = new MockBaseBinding<ISample>();

            Assert.Throws<BindingResolutionException>(() =>
            {
                var sample = binding.Resolve();
            });
        }

        [Fact]
        public void Test_BindingType_Returns_Generic_Interface_Type_Of_Binding()
        {
            IBinding binding = new MockBaseBinding<ISample>();

            Assert.Same(typeof(ISample), binding.BindingType);
        }
    }
}
