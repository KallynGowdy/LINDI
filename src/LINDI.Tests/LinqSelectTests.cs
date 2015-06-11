using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using LINDI.Core;
using LINDI.Core.Bindings;
using LINDI.Core.Linq;
using Xunit;
using static LINDI.Core.LindiMethods;

namespace LINDI.Tests
{
    /// <summary>
    /// Tests for the <see cref="LINDI.Core.Linq.BindingExtensions.Select()"/> extension methods.
    /// </summary>
    public class LinqSelectTests
    {
        [Fact]
        public void Test_Simple_NewExpression_Returns_Directly_Built_BindToConstructor_Binding()
        {
            IBinding<ISample> binding = Bind<ISample>().Select(type => new Sample());

            Assert.IsType<BindToConstructor<ISample, Sample>>(binding);
        }

        [Fact]
        public void Test_NewExpression_With_Dependencies_Returns_A_LazyBindToConstructor()
        {
            IBinding<ISample> sampleBinding = Bind<ISample>().Select(value => new Sample());


            IBinding<IHasSample> binding = from value in Bind<IHasSample>()
                                           let dependencies = new { dep1 = DependencyUsing(sampleBinding) }
                                           select new HasSample(dependencies.dep1);

        }
    }
}
