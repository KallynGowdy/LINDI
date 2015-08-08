using System;
using Lindi.Core.Bindings;
using Xunit;

namespace Lindi.Tests.Core.Bindings
{
    /// <summary>
    /// Tests for <see cref="BindingResolutionException"/>.
    /// </summary>
    public class BindingResolutionExceptionTests
    {
        [Fact]
        public void Test_Constructor_Throws_ArgumentNullExeption_When_Given_Null_BindingType()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var exception = new BindingResolutionException(null, null);
                var exception1 = new BindingResolutionException("Message", null, null);
            });
        }

        [Fact]
        public void Test_Create_Exception_With_BindingType()
        {
            var exception = new BindingResolutionException(typeof(ISample), null);
            var otherException = new BindingResolutionException("Message", typeof(ISample), null);

            Assert.Equal(typeof(ISample), exception.BindingType);
            Assert.Equal(typeof(ISample), otherException.BindingType);
        }
    }
}
