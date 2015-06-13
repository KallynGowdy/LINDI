using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Lindi.Core;
using Lindi.Core.Bindings;
using Lindi.Core.Linq;
using Xunit;
using static Lindi.Core.LindiMethods;

namespace Lindi.Tests
{
    /// <summary>
    /// Tests for the <see cref="Lindi.Core.Linq.BindingExtensions.Select()"/> extension methods.
    /// </summary>
    public class LinqSelectTests
    {
        [Fact]
        public void Test_NewExpression_With_Dependencies_Returns_A_LazyBindToConstructor()
        {
            IBinding<ISample> sampleBinding = Bind<ISample>().Select(value => new Sample());

            IBinding<IHasSample> binding = from value in Bind<IHasSample>()
                                           select new HasSample(Dependency(sampleBinding));

            Assert.IsType<LazyConstructorBinding<IHasSample>>(binding);
        }

        [Fact]
        public void Test_MethodCall_With_Dependencies_Returns_A_LazyBindToConstructor()
        {
            IBinding<ISample> sampleBinding = Bind<ISample>().Select(value => new Sample());

            Func<ISample, HasSample> providerFunc = sample => null;

            IBinding<IHasSample> binding = from value in Bind<IHasSample>()
                                           select providerFunc(Dependency(sampleBinding));

            Assert.IsType<LazyConstructorBinding<IHasSample>>(binding);
        }

        [Fact]
        public void Test_MethodCall_With_Dependencies_And_NonDependencies_Returns_A_LazyBindToConstructor()
        {
            IBinding<ISample> sampleBinding = Bind<ISample>().Select(value => new Sample());

            Func<ISample, int, HasSample> providerFunc = (sample, i) => null;

            IBinding<IHasSample> binding = from value in Bind<IHasSample>()
                                           select providerFunc(Dependency(sampleBinding), 1);

            Assert.IsType<LazyConstructorBinding<IHasSample>>(binding);
        }

        [Fact]
        public void Test_Passing_Expression_That_Uses_Value_Throws_ArgumentException()
        {
            IBinding<ISample> sampleBinding = Bind<ISample>().Select(value => new Sample());

            Assert.Throws<ArgumentException>(() =>
            {
                IBinding<IHasSample> binding = from value in Bind<IHasSample>()
                                               select value;
            });
        }

        [Fact]
        public void Test_Select_Throws_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                Bind<ISample>().Select((Expression<Func<ISample, Sample>>)null);
            });
        }
    }
}
