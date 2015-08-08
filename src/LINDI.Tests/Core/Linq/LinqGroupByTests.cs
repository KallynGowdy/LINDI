using System;
using Lindi.Core;
using Lindi.Core.Bindings;
using Lindi.Core.Linq;
using Xunit;
using static Lindi.Core.LindiMethods;
using Thread = System.Threading.Thread;

namespace Lindi.Tests.Core.Linq
{
    /// <summary>
    /// Tests for <see cref="BindingExtensions.GroupBy{TInterface,TValue}"/>.
    /// </summary>
    public class LinqGroupByTests
    {
        [Fact]
        public void Test_Grouped_Binding_Creates_New_Scoped_Binding_For_Given_Reference_Value()
        {
            object val = new object();
            IBinding<ISample> binding = from type in Bind<ISample>()
                                        group type by val into scope
                                        select new Sample();

            Assert.NotNull(binding);
            Assert.IsType<ReferenceScopedBinding<ISample, object>>(binding);
        }

        [Fact]
        public void Test_Grouped_Binding_Can_Be_Resolved()
        {
            object val = new object();
            Sample expectedValue = new Sample();
            IBinding<ISample> binding = from type in Bind<ISample>()
                                        group type by val into scope
                                        select expectedValue;

            ISample value = binding.Resolve();

            Assert.Same(expectedValue, value);
        }

        [Fact]
        public void Test_Grouped_Binding_Reuses_Value()
        {
            object val = new object();
            IBinding<ISample> binding = from type in Bind<ISample>()
                                        group type by val into scope
                                        select new Sample();

            ISample value = binding.Resolve();

            ISample secondValue = binding.Resolve();

            Assert.Same(value, secondValue);
        }

        [Fact]
        public void Test_GroupBy_Throws_ArgumentNullException_WhenGivenNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                IBinding<ISample> binding = Bind<ISample>().GroupBy<ISample, object>(null).Select(scope => new Sample());
            });
        }

        [Fact]
        public void Test_Grouped_Binding_Produces_New_Value_When_Context_Value_Changes()
        {
            object val = new object();

            IBinding<ISample> binding = from type in Bind<ISample>()
                                        group type by val into scope
                                        select new Sample();

            ISample first = binding.Resolve();

            val = new object();

            ISample second = binding.Resolve();

            Assert.NotSame(first, second);
        }

        [Fact]
        public void Test_Grouped_Binding_Allows_Value_To_Be_Garbage_Collected()
        {
            WeakReference reference = null;

            // Action symbolizes a request/response cycle like what you would see in a 
            // ASP.Net Application
            new Action(() =>
            {
                object val = new object(); // Use value, this could be anything (HttpContext, Thread, etc.)

                IBinding<ISample> binding = from type in Bind<ISample>()
                                            group type by val into scope
                                            select new Sample();

                ISample sample = binding.Resolve(); // Break down the created binding and make sure the value is allocated

                reference = new WeakReference(val, true); // Obtain a weak reference to the value
            })();

            GC.Collect();
            GC.WaitForPendingFinalizers();

            Assert.Null(reference.Target);
        }

        [Fact]
        public void Test_Grouping_By_True_Creates_New_ValueScopedBinding()
        {
            IBinding<ISample> binding = from type in Bind<ISample>()
                                        group type by true into scope
                                        select new Sample();

            Assert.NotNull(binding);
            Assert.IsType<ValueScopedBinding<ISample, bool>>(binding);
        }

        [Fact]
        public void Test_ValueScopedBinding_Can_Resolve_Values()
        {
            IBinding<ISample> binding = from type in Bind<ISample>()
                                        group type by true into scope
                                        select new Sample();

            ISample value = binding.Resolve();

            Assert.NotNull(value);
        }

        [Fact]
        public void Test_ValueScopedBinding_Produces_Same_Value_For_Same_Selected_Value()
        {
            IBinding<ISample> binding = from type in Bind<ISample>()
                                        group type by true into scope
                                        select new Sample();

            ISample value = binding.Resolve();

            ISample other = binding.Resolve();

            Assert.Same(value, other);
        }

        [Fact]
        public void Test_ValueScopedBinding_Produces_Different_Value_For_Different_Selected_Value()
        {
            bool b = true;
            Func<bool> selector = () => (b = !b);
            IBinding<ISample> binding = from type in Bind<ISample>()
                                        group type by selector() into scope
                                        select new Sample();

            ISample value = binding.Resolve();
            ISample other = binding.Resolve();

            Assert.NotSame(value, other);
            Assert.Same(value, binding.Resolve());
        }

        [Fact]
        public void Test_Group_By_Singleton_Produces_Same_Value_Every_Time()
        {
            IBinding<ISample> binding = from type in Bind<ISample>()
                                        group type by Singleton() into scope
                                        select new Sample();

            ISample value = binding.Resolve();

            ISample other = binding.Resolve();

            Assert.Same(value, other);
        }

        [Fact]
        public void Test_Group_By_Thread_Produces_Same_Value_Inside_Same_Thread()
        {
            IBinding<ISample> binding = from type in Bind<ISample>()
                                        group type by Thread() into scope
                                        select new Sample();
            ISample value = null;
            ISample otherValue = null;
            Thread t = new Thread(() =>
            {
                value = binding.Resolve();
                otherValue = binding.Resolve();
            });

            t.Start();
            t.Join();

            Assert.NotNull(value);
            Assert.NotNull(otherValue);
            Assert.Same(value, otherValue);
        }

        [Fact]
        public void Test_Group_By_Thread_Produces_Different_Value_On_Different_Threads()
        {
            IBinding<ISample> binding = from type in Bind<ISample>()
                                        group type by Thread() into scope
                                        select new Sample();
            ISample value = null;
            ISample otherValue = null;
            Thread t = new Thread(() =>
            {
                value = binding.Resolve();
            });
            Thread t1 = new Thread(() =>
            {
                otherValue = binding.Resolve();
            });

            t.Start();
            t1.Start();
            t.Join();
            t1.Join();

            Assert.NotNull(value);
            Assert.NotNull(otherValue);
            Assert.NotSame(value, otherValue);
        }
    }
}
