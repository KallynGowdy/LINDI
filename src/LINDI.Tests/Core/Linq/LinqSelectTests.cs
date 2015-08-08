using System;
using System.Linq.Expressions;
using Xunit;
using Lindi.Core;
using Lindi.Core.Bindings;
using Lindi.Core.Linq;

namespace Lindi.Tests.Core.Linq
{
    /// <summary>
    /// Tests for the <see cref="Lindi.Core.Linq.BindingExtensions.Select()"/> extension methods.
    /// </summary>
    public class LinqSelectTests
    {
        [Fact]
        public void Test_NewExpression_With_Dependencies_Returns_A_LazyBindToConstructor()
        {
            IBinding<ISample> sampleBinding = LindiMethods.Bind<ISample>().Select(value => new Sample());

            IBinding<IHasSample> binding = from value in LindiMethods.Bind<IHasSample>()
                                           select new HasSample(LindiMethods.Dependency(sampleBinding));

            Assert.IsType<LazyConstructorBinding<IHasSample>>(binding);
        }

        [Fact]
        public void Test_MethodCall_With_Dependencies_Returns_A_LazyBindToConstructor()
        {
            IBinding<ISample> sampleBinding = LindiMethods.Bind<ISample>().Select(value => new Sample());

            Func<ISample, HasSample> providerFunc = sample => null;

            IBinding<IHasSample> binding = from value in LindiMethods.Bind<IHasSample>()
                                           select providerFunc(LindiMethods.Dependency(sampleBinding));

            Assert.IsType<LazyConstructorBinding<IHasSample>>(binding);
        }

        [Fact]
        public void Test_MethodCall_With_Dependencies_And_NonDependencies_Returns_A_LazyBindToConstructor()
        {
            IBinding<ISample> sampleBinding = LindiMethods.Bind<ISample>().Select(value => new Sample());

            Func<ISample, int, HasSample> providerFunc = (sample, i) => null;

            IBinding<IHasSample> binding = from value in LindiMethods.Bind<IHasSample>()
                                           select providerFunc(LindiMethods.Dependency(sampleBinding), 1);

            Assert.IsType<LazyConstructorBinding<IHasSample>>(binding);
        }

        [Fact]
        public void Test_MethodCall_With_Dependencies_And_NonDependant_Methods_Returns_A_LazyBindToConstructor()
        {
            IBinding<ISample> sampleBinding = LindiMethods.Bind<ISample>().Select(value => new Sample());

            Func<ISample, int, HasSample> providerFunc = (sample, i) => null;

            IBinding<IHasSample> binding = from value in LindiMethods.Bind<IHasSample>()
                                           select providerFunc(LindiMethods.Dependency(sampleBinding), Math.Abs(-9));

            Assert.IsType<LazyConstructorBinding<IHasSample>>(binding);
        }

        [Fact]
        public void Test_Passing_Expression_That_Uses_Value_Throws_ArgumentException()
        {
            IBinding<ISample> sampleBinding = LindiMethods.Bind<ISample>().Select(value => new Sample());

            Assert.Throws<ArgumentException>(() =>
            {
                IBinding<IHasSample> binding = from value in LindiMethods.Bind<IHasSample>()
                                               select value;
            });
        }

        [Fact]
        public void Test_Select_Throws_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                LindiMethods.Bind<ISample>().Select((Expression<Func<ISample, Sample>>)null);
            });
        }

        [Fact]
        public void Test_Null_Dependency_Throws_ArgumentNullException()
        {
            IBinding<IHasSample> hasSampleBinding = null;

            Assert.Throws<ArgumentNullException>(() =>
            {
                IBinding<ISample> sampleBinding = from value in LindiMethods.Bind<ISample>()
                                                  select new Sample
                                                  {
                                                      Obj = LindiMethods.Dependency(hasSampleBinding)
                                                  };
            });
        }

        [Fact]
        public void Test_Binding_Can_Be_Resolved()
        {
            IBinding<ISample> sampleBinding = from value in LindiMethods.Bind<ISample>()
                                              select new Sample();

            ISample sample = sampleBinding.Resolve();

            Assert.NotNull(sample);
            Assert.IsType<Sample>(sample);
        }

        [Fact]
        public void Test_Binding_With_Dependencies_Can_Be_Resolved()
        {
            IBinding<ISample> sampleBinding = from value in LindiMethods.Bind<ISample>()
                                              select new Sample();
            IBinding<IHasSample> hasSampleBinding = from value in LindiMethods.Bind<IHasSample>()
                                                    select new HasSample(LindiMethods.Dependency(sampleBinding));

            IHasSample hasSample = hasSampleBinding.Resolve();

            Assert.NotNull(hasSample);
            Assert.IsType<HasSample>(hasSample);
            Assert.NotNull(hasSample.Sample);
            Assert.IsType<Sample>(hasSample.Sample);
        }

        [Fact]
        public void Test_Binding_With_Dependencies_Using_Extension_Method_Can_Be_Resolved()
        {
            IBinding<ISample> sampleBinding = from value in LindiMethods.Bind<ISample>()
                                              select new Sample();
            IBinding<IHasSample> hasSampleBinding = from value in LindiMethods.Bind<IHasSample>()
                                                    select new HasSample(sampleBinding.AsDependency());

            IHasSample hasSample = hasSampleBinding.Resolve();

            Assert.NotNull(hasSample);
            Assert.IsType<HasSample>(hasSample);
            Assert.NotNull(hasSample.Sample);
            Assert.IsType<Sample>(hasSample.Sample);
        }

        [Fact]
        public void Test_MethodCall_With_Dependencies_And_NonDependant_Methods_Can_Be_Resolved()
        {
            IBinding<ISample> sampleBinding = LindiMethods.Bind<ISample>().Select(value => new Sample());

            Func<ISample, int, HasSample> providerFunc = (sample, i) => new HasSample(sample);

            IBinding<IHasSample> binding = from value in LindiMethods.Bind<IHasSample>()
                                           select providerFunc(LindiMethods.Dependency(sampleBinding), Math.Abs(-9));

            IHasSample hasSample = binding.Resolve();

            Assert.NotNull(hasSample);
            Assert.IsType<HasSample>(hasSample);
            Assert.NotNull(hasSample.Sample);
            Assert.IsType<Sample>(hasSample.Sample);
        }
    }
}
