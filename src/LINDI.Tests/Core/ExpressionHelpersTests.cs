using System;
using System.Linq.Expressions;
using Lindi.Core;
using Xunit;

namespace Lindi.Tests.Core
{
    /// <summary>
    /// Tests for <see cref="ExpressionHelpers"/> methods.
    /// </summary>
    public class ExpressionHelpersTests
    {
        [Fact]
        public void Test_Lock_Throws_When_Given_Null_LockObj_Expression()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                ExpressionHelpers.Lock(null, Expression.Parameter(typeof(int)));
            });
        }

        [Fact]
        public void Test_Lock_Throws_When_Given_Null_Body_Expression()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                ExpressionHelpers.Lock(Expression.Parameter(typeof(int)), null);
            });
        }
    }
}
