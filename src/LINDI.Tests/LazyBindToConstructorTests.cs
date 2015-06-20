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

        [Fact]
        public void Test_Resolve_Implementation_Flattens_Multiple_Lazy_Bindings()
        {
            LazyConstructorBinding<IHasSample> lazyBinding = new LazyConstructorBinding<IHasSample>(new IBinding[]
            {
                new LazyConstructorBinding<ISample>(new IBinding[0], bindings => new Sample())
            }, bindings => new HasSample(((LazyConstructorBinding<ISample>)bindings[0]).Resolve()));

            IHasSample sample = lazyBinding.Resolve();

            Assert.IsType<HasSample>(sample);
            Assert.IsType<BindToConstructor<IHasSample>>(lazyBinding.Constructor);
        }

        [Fact]
        public void Test_Resolve_Implementation_Handles_Non_Lazy_Bindings_Correctly()
        {
            IBinding<ISample> binding = new BindToConstructor<ISample>(() => new Sample());
            LazyConstructorBinding<IHasSample> lazyBinding = new LazyConstructorBinding<IHasSample>(
                new IBinding[0],
                bindings => new HasSample(binding.Resolve()));

            IHasSample sample = lazyBinding.Resolve();

            Assert.IsType<HasSample>(sample);
            Assert.IsType<BindToConstructor<IHasSample>>(lazyBinding.Constructor);
        }

        [Fact]
        public void Test_Resolve_Implementation_Handles_Binding_Inside_Initializer()
        {
            IBinding<ISample> binding = new LazyConstructorBinding<ISample>(new IBinding[]
            {
                new LazyConstructorBinding<object>(new IBinding[0], bindings => new Sample())
            }, bindings => new Sample
            {
                Obj = ((ILazyConstructorBinding<object>)bindings[0]).Resolve()
            });

            ISample sample = binding.Resolve();

            Assert.IsType<Sample>(sample);
            Assert.IsType<Sample>(((Sample)sample).Obj);
        }

    }
}
