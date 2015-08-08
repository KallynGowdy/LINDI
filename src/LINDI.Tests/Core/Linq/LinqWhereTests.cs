using Lindi.Core;
using Lindi.Core.Bindings;
using Lindi.Core.Linq;
using Xunit;

namespace Lindi.Tests.Core.Linq
{
    /// <summary>
    /// Tests for <see cref="InjectExtensions.Where{TInterface,TInjectedInto}"/>.
    /// </summary>
    public class LinqWhereTests
    {
        [Fact]
        public void Test_Where_Without_Select_Returns_Incomplete_Injection()
        {
            Sample s = new Sample();

            IInjectValuesInto<INeedSample> binding = LindiMethods.Inject()
                                                        .Where(v => LindiMethods.InjectedInto(v) as INeedSample);
            NeedSample needSample = new NeedSample();

            Assert.Throws<BindingResolutionException>(() =>
            {
                binding.Inject(needSample);
            });
        }

        [Fact]
        public void Test_Where_Is_Able_To_Inject_Values_Into_Type()
        {
            Sample s = new Sample();
            IBinding<ISample> sampleBinding = from value in LindiMethods.Bind<ISample>()
                                              select s;

            IInjectValuesInto<INeedSample> binding = LindiMethods.Inject()
                                                        .Where(v => LindiMethods.InjectedInto(v) as INeedSample)
                                                        .Select(value =>
                                                        {
                                                            value.Sample = sampleBinding.Resolve();
                                                        });

            NeedSample needSample = new NeedSample();

            binding.Inject(needSample);

            Assert.Same(s, needSample.Sample);
        }

        [Fact]
        public void Test_Multiple_Values_Are_Able_To_Be_Injected()
        {
            Sample s = new Sample();
            const int i = 10;

            IBinding<ISample> sampleBinding = from value in LindiMethods.Bind<ISample>()
                                              select s;

            IBinding<int> intBinding = from value in LindiMethods.Bind<int>()
                                       select i;

            IInjectValuesInto<INeedSampleAndInt> injector = LindiMethods.Inject()
                                                            .Where(value => LindiMethods.InjectedInto(value) as INeedSampleAndInt)
                                                            .Select(value =>
                                                            {
                                                                value.Sample = sampleBinding.Resolve();
                                                                value.Int = intBinding.Resolve();
                                                            });

            NeedSample needsSample = new NeedSample();

            injector.Inject(needsSample);

            Assert.Same(s, needsSample.Sample);
            Assert.Equal(i, needsSample.Int);
        }

        [Fact]
        public void Test_IsInjectedInto_Creates_Same_Injection_As_InjectedInto()
        {
            Sample s = new Sample();
            const int i = 10;

            IBinding<ISample> sampleBinding = from value in LindiMethods.Bind<ISample>()
                                              select s;

            IBinding<int> intBinding = from value in LindiMethods.Bind<int>()
                                       select i;

            IInjectValuesInto<INeedSampleAndInt> injector = LindiMethods.Inject()
                                                            .Where(value => LindiMethods.InjectedInto(value) as INeedSampleAndInt)
                                                            .Select(value =>
                                                            {
                                                                value.Sample = sampleBinding.Resolve();
                                                                value.Int = intBinding.Resolve();
                                                            });

            IInjectValuesInto<INeedSampleAndInt> otherInjector = LindiMethods.Inject()
                        .Where(value => LindiMethods.IsInjectedInto<INeedSampleAndInt>())
                        .Select(value =>
                        {
                            value.Sample = sampleBinding.Resolve();
                            value.Int = intBinding.Resolve();
                        });

            Assert.Same(injector.GetType(), otherInjector.GetType());
        }
    }

    public interface INeedSample
    {
        ISample Sample { get; set; }
    }

    public interface INeedSampleAndInt : INeedSample
    {
        int Int { get; set; }
    }
    public class NeedSample : INeedSampleAndInt
    {
        public ISample Sample { get; set; }
        public int Int { get; set; }
    }
}
