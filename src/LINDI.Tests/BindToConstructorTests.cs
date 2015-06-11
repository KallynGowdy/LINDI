using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LINDI.Core.Bindings;
using Xunit;

namespace LINDI.Tests
{
    /// <summary>
    /// Tests for <see cref="BindToConstructor{TInterface,TImplementer}"/>.
    /// </summary>
    public class BindToConstructorTests
    {
        [Fact]
        public void Test_Resolve_Uses_Given_Constructor_Function()
        {
            Sample value = new Sample();
            Func<Sample> constructorFunc = () =>
            {
                return value;
            };
            BindToConstructor<ISample, Sample> constructor = new BindToConstructor<ISample, Sample>(constructorFunc);

            ISample obj = constructor.Resolve();

            Assert.Same(value, obj);
        }
    }
}
