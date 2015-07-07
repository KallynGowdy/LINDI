using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lindi.Core;
using Lindi.Core.Linq;
using Xunit;
using static Lindi.Core.LindiMethods;

namespace Lindi.Tests
{
    /// <summary>
    /// Tests for <see cref="BindingExtensions.Where(IBinding{T},Func{object,bool}(object))"/>
    /// </summary>
    public class LinqWhereTests
    {
        [Fact]
        public void Test_Where_Is_Able_To_Inject_Values_Into_Type()
        {
            Sample s = new Sample();
            IBinding<ISample> sampleBinding = from value in Bind<ISample>()
                                              select s;

            IInjectValuesInto<INeedSample> binding = from value in Inject()
                                                     where InjectedInto(value) as INeedSample
                                                     select value.Sample = sampleBinding.Resolve();

            NeedSample needSample = new NeedSample();

            binding.Inject(needSample);

            Assert.Same(s, needSample.Sample);
        }

        [Fact]
        public void Test_Multiple_Values_Are_Able_To_Be_Injected()
        {
            Sample s = new Sample();
            const int i = 10;

            IBinding<ISample> sampleBinding = from value in Bind<ISample>()
                                              select s;

            IBinding<int> intBinding = from value in Bind<int>()
                                       select i;

            IInjectValuesInto<INeedSampleAndInt> injector = Inject()
                                                            .Where(value => InjectedInto(value) as INeedSampleAndInt)
                                                            .Select(value =>
                                                            {
                                                                value.Sample = sampleBinding.Resolve();
                                                                value.Int = intBinding.Resolve();
                                                            });

            NeedSample needsSample = new NeedSample();

            injector.Inject(needsSample);
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
