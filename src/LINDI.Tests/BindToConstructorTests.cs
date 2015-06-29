using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lindi.Core.Bindings;
using Xunit;

namespace Lindi.Tests
{
    /// <summary>
    /// Tests for <see cref="ConstructorBinding{TInterface}"/>.
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
            ConstructorBinding<ISample> constructor = new ConstructorBinding<ISample>(constructorFunc);

            ISample obj = constructor.Resolve();

            Assert.Same(value, obj);
        }
    }
}
