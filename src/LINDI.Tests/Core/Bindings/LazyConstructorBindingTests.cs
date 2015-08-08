using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.InteropServices.ComTypes;
using Lindi.Core;
using Lindi.Core.Bindings;
using Xunit;

namespace Lindi.Tests.Core.Bindings
{
    /// <summary>
    /// Tests for <see cref="LazyConstructorBinding{TInterface}"/>.
    /// </summary>
    public class LazyConstructorBindingTests
    {
        [Fact]
        public void Test_Constructor_Throws_When_Given_Null_Dependencies()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                LazyConstructorBinding<object> binding = new LazyConstructorBinding<object>(null, bindings => new object());
            });
        }

        [Fact]
        public void Test_Constructor_Throws_When_Given_Null_Construction_Expression()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                LazyConstructorBinding<object> binding = new LazyConstructorBinding<object>(new IBinding[0], null);
            });
        }

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
            Assert.IsType<ConstructorBinding<IHasSample>>(lazyBinding.Constructor);
        }

        [Fact]
        public void Test_Resolve_Implementation_Handles_Non_Lazy_Bindings_Correctly()
        {
            IBinding<ISample> binding = new ConstructorBinding<ISample>(() => new Sample());
            LazyConstructorBinding<IHasSample> lazyBinding = new LazyConstructorBinding<IHasSample>(
                new IBinding[0],
                bindings => new HasSample(binding.Resolve()));

            IHasSample sample = lazyBinding.Resolve();

            Assert.IsType<HasSample>(sample);
            Assert.IsType<ConstructorBinding<IHasSample>>(lazyBinding.Constructor);
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

        [Fact]
        public void Test_Resolve_Throws_InvalidOperationException_When_Dependencies_Have_Not_Been_Set()
        {
            DerivedLazyConstructorBinding binding = new DerivedLazyConstructorBinding();
            binding.ConstructionExpression = bindings => new object();
            BindingResolutionException ex = Assert.Throws<BindingResolutionException>(() =>
            {
                object obj = binding.Resolve();
            });

            Assert.NotNull(ex.InnerException);
            Assert.IsType<InvalidOperationException>(ex.InnerException);
        }

        [Fact]
        public void Test_Resolve_Throws_InvalidOperationException_When_ConstructionExpression_Has_Not_Been_Set()
        {
            DerivedLazyConstructorBinding binding = new DerivedLazyConstructorBinding();
            binding.Dependencies = new IBinding[0];
            BindingResolutionException ex = Assert.Throws<BindingResolutionException>(() =>
            {
                object obj = binding.Resolve();
            });
            
            Assert.NotNull(ex.InnerException);
            Assert.IsType<InvalidOperationException>(ex.InnerException);
        }

        class DerivedLazyConstructorBinding : LazyConstructorBinding<object>
        {
            public new IEnumerable<IBinding> Dependencies
            {
                get
                {
                    return base.Dependencies;
                }
                set
                {
                    base.Dependencies = value;
                }
            }

            public new Expression<Func<IBinding[], object>> ConstructionExpression
            {
                get
                {
                    return base.ConstructionExpression;
                }
                set
                {
                    base.ConstructionExpression = value;
                }
            }

            public DerivedLazyConstructorBinding() : base()
            {

            }
        }
    }
}
