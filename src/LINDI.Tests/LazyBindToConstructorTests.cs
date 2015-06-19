using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lindi.Core;
using Lindi.Core.Bindings;
using Xunit;

namespace Lindi.Tests
{
    /// <summary>
    /// Tests for <see cref="LazyConstructorBinding{TInterface}"/>.
    /// </summary>
    public class LazyBindToConstructorTests
    {

        [Fact]
        public void Test_Resolve_Implementation_Builds_Expression()
        {
            var lazyBinding = new LazyConstructorBinding<ISample>(new IBinding[0], bindings => new Sample());

            Assert.False(lazyBinding.IsBuilt);

            ISample sample = lazyBinding.Resolve();

            Assert.IsType<Sample>(sample);
            Assert.True(lazyBinding.IsBuilt);
        }

    }
}
