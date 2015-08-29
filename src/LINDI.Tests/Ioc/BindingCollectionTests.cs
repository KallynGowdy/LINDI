using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lindi.Core;
using Lindi.Core.Bindings;
using Lindi.Core.Linq;
using Lindi.Ioc;
using Lindi.Tests.Core;
using Xunit;
using static Lindi.Core.LindiMethods;
using System.Collections;

namespace Lindi.Tests.Ioc
{
    /// <summary>
    /// Tests for <see cref="BindingCollection"/>.
    /// </summary>
    public class BindingCollectionTests
    {
        [Fact]
        public void Test_Create_Empty_Collection()
        {
            IBindingCollection collection = new BindingCollection();
            Assert.Equal(0, collection.TypeCount);
            Assert.Equal(0, collection.BindingCount);
        }

        [Fact]
        public void Test_Create_Collection_With_Array()
        {
            IBinding<ISample> binding = from b in Bind<ISample>()
                                        select new Sample();
            IBinding<IHasSample> otherBinding = from b in Bind<IHasSample>()
                                                select new HasSample(Dependency(binding));

            IBindingCollection collection = new BindingCollection(new IBinding[] { binding, otherBinding });

            Assert.Equal(2, collection.TypeCount);
            Assert.Equal(2, collection.BindingCount);
        }

        [Fact]
        public void Test_Create_Collection_With_Null_Array()
        {
            IBindingCollection collection = new BindingCollection(null);
            Assert.Equal(0, collection.TypeCount);
            Assert.Equal(0, collection.BindingCount);
        }

        [Fact]
        public void Test_TypeCount_Returns_The_Number_Of_Distinct_Types_Stored_In_The_Collection()
        {
            IBinding<ISample> binding = from b in Bind<ISample>()
                                        select new Sample();
            IBinding<ISample> otherSampleBinding = Bind<ISample>().Select(b => new Sample(new object()));
            IBinding<IHasSample> otherBinding = from b in Bind<IHasSample>()
                                                select new HasSample(Dependency(binding));

            IBindingCollection collection = new BindingCollection(new IBinding[] { binding, otherBinding, otherSampleBinding });
            Assert.Equal(2, collection.TypeCount);
        }

        [Fact]
        public void Test_BindingCount_Returns_The_Number_Of_Bindings_Stored_In_The_Collection()
        {
            IBinding<ISample> binding = from b in Bind<ISample>()
                                        select new Sample();
            IBinding<ISample> otherSampleBinding = Bind<ISample>().Select(b => new Sample(new object()));
            IBinding<IHasSample> otherBinding = from b in Bind<IHasSample>()
                                                select new HasSample(Dependency(binding));

            IBindingCollection collection = new BindingCollection(new IBinding[] { binding, otherBinding, otherSampleBinding });
            Assert.Equal(3, collection.BindingCount);
        }

        [Fact]
        public void Test_Resolve_Non_Generic_Binding_From_Collection()
        {
            ISample sample = new Sample();
            IBinding<ISample> binding = from b in Bind<ISample>()
                                        select sample;

            IBindingCollection collection = new BindingCollection(new IBinding[] { binding });

            object resolved = collection.Resolve(typeof(ISample));

            Assert.Same(sample, resolved);
        }

        [Fact]
        public void Test_Resolve_Generic_Binding_From_Collection()
        {
            ISample sample = new Sample();
            IBinding<ISample> binding = from b in Bind<ISample>()
                                        select sample;

            IBindingCollection collection = new BindingCollection(new IBinding[] { binding });

            ISample resolved = collection.Resolve<ISample>();

            Assert.Same(sample, resolved);
        }

        [Fact]
        public void Test_Resolve_Multiple_Non_Generic_Bindings_From_Collection()
        {
            Sample firstSample = new Sample();
            Sample secondSample = new Sample(new object());
            IBinding<ISample> binding = from b in Bind<ISample>()
                                        select firstSample;
            IBinding<ISample> otherSampleBinding = Bind<ISample>().Select(b => secondSample);
            IBinding<IHasSample> otherBinding = from b in Bind<IHasSample>()
                                                select new HasSample(Dependency(binding));

            IBindingCollection collection = new BindingCollection(new IBinding[] { binding, otherBinding, otherSampleBinding });

            object[] resolved = collection.ResolveAll(typeof(ISample));

            // The bindings should come back in the same order that they were added in
            Assert.Collection(resolved,
                s1 => Assert.Same(firstSample, s1),
                s2 => Assert.Same(secondSample, s2));
        }

        [Fact]
        public void Test_Resolve_Multiple_Generic_Bindings_From_Collection()
        {
            Sample firstSample = new Sample();
            Sample secondSample = new Sample(new object());
            IBinding<ISample> binding = from b in Bind<ISample>()
                                        select firstSample;
            IBinding<ISample> otherSampleBinding = Bind<ISample>().Select(b => secondSample);
            IBinding<IHasSample> otherBinding = from b in Bind<IHasSample>()
                                                select new HasSample(Dependency(binding));

            IBindingCollection collection = new BindingCollection(new IBinding[] { binding, otherBinding, otherSampleBinding });

            ISample[] resolved = collection.ResolveAll<ISample>();

            // The bindings should come back in the same order that they were added in
            Assert.Collection(resolved,
                s1 => Assert.Same(firstSample, s1),
                s2 => Assert.Same(secondSample, s2));
        }

        [Fact]
        public void Test_Resolve_Non_Generic_Missing_Type_From_Collection_Throws_BindingResolutionException()
        {
            IBindingCollection collection = new BindingCollection();
            Assert.Throws<BindingResolutionException>(() =>
            {
                object resolved = collection.Resolve(typeof(ISample));
            });
        }

        [Fact]
        public void Test_Resolve_Generic_Missing_Type_From_Collection_Throws_BindingResolutionException()
        {
            IBindingCollection collection = new BindingCollection();
            Assert.Throws<BindingResolutionException>(() =>
            {
                ISample resolved = collection.Resolve<ISample>();
            });
        }

        [Fact]
        public void Test_Resolve_Multiple_Non_Generic_Missing_Type_Bindings_From_Collection_Throws_BindingResolutionException()
        {
            IBindingCollection collection = new BindingCollection();
            Assert.Throws<BindingResolutionException>(() =>
            {
                object[] resolved = collection.ResolveAll(typeof(ISample));
            });
        }

        [Fact]
        public void Test_Resolve_Multiple_Generic_Missing_Type_Bindings_From_Collection_Throws_BindingResolutionException()
        {
            IBindingCollection collection = new BindingCollection();
            Assert.Throws<BindingResolutionException>(() =>
            {
                ISample[] resolved = collection.ResolveAll<ISample>();
            });
        }

        [Fact]
        public void Test_Add_Binding_To_Collection()
        {
            IBinding<ISample> binding = from b in Bind<ISample>()
                                        select new Sample();
            IBindingCollection collection = new BindingCollection();

            collection.Add(binding);

            Assert.Collection(collection,
                b => Assert.Same(binding, b));
        }

        [Fact]
        public void Test_Add_Binding_To_Collection_Throws_ArgumentNullException()
        {
            IBindingCollection collection = new BindingCollection();

            Assert.Throws<ArgumentNullException>(() =>
            {
                collection.Add(null);
            });
        }

        [Fact]
        public void Test_Add_Multiple_Bindings_To_Collection()
        {
            IBinding<ISample> binding = from b in Bind<ISample>()
                                        select new Sample();
            IBinding<ISample> otherSampleBinding = Bind<ISample>().Select(b => new Sample(new object()));
            IBinding<IHasSample> otherBinding = from b in Bind<IHasSample>()
                                                select new HasSample(Dependency(binding));

            IBindingCollection collection = new BindingCollection();

            collection.AddRange(new IBinding[] { binding, otherBinding, otherSampleBinding });
            
            Assert.Equal(3, collection.BindingCount);
        }

        [Fact]
        public void Test_Add_Multiple_Bindings_To_Collection_Throws_ArgumentNullException()
        {
            IBindingCollection collection = new BindingCollection();

            Assert.Throws<ArgumentNullException>(() =>
            {
                collection.AddRange(null);
            });
        }

        [Fact]
        public void Test_Remove_Binding_From_Collection()
        {
            IBinding<ISample> binding = from b in Bind<ISample>()
                                        select new Sample();
            IBinding<ISample> otherSampleBinding = Bind<ISample>().Select(b => new Sample(new object()));
            IBinding<IHasSample> otherBinding = from b in Bind<IHasSample>()
                                                select new HasSample(Dependency(binding));

            IBindingCollection collection = new BindingCollection(new IBinding[] { binding, otherBinding, otherSampleBinding });

            Assert.True(collection.Remove(binding));
            Assert.Equal(2, collection.BindingCount);
        }

        [Fact]
        public void Test_Remove_Binding_From_Collection_Allows_Null()
        {
            IBindingCollection collection = new BindingCollection();

            Assert.False(collection.Remove(null));
        }

        [Fact]
        public void Test_Create_Binding_That_Requires_Container_To_Resolve_Multiple_Values()
        {
            Sample firstSample = new Sample();
            Sample secondSample = new Sample(new object());
            IBinding<ISample> binding = from b in Bind<ISample>()
                                        select firstSample;
            IBinding<ISample> otherBinding = from b in Bind<ISample>()
                                             select secondSample;

            IBindingCollection collection = new BindingCollection(new IBinding[] { binding, otherBinding });

            IBinding<INeedMultipleSamples> testedBinding = from b in Bind<INeedMultipleSamples>()
                                                           select new NeedMultipleSamples(collection.ResolveAll<ISample>());

            INeedMultipleSamples resolved = testedBinding.Resolve();

            Assert.NotNull(resolved);
            Assert.Collection(resolved.Samples,
                s1 => Assert.Same(firstSample, s1),
                s2 => Assert.Same(secondSample, s2));
        }

        [Fact]
        public void Test_Enumerate_Bindings_In_Collection()
        {
            Sample firstSample = new Sample();
            Sample secondSample = new Sample(new object());
            IBinding<ISample> binding = from b in Bind<ISample>()
                                        select firstSample;
            IBinding<ISample> otherBinding = from b in Bind<ISample>()
                                             select secondSample;

            IBindingCollection collection = new BindingCollection(new IBinding[] { binding, otherBinding });

            Assert.Collection(collection,
                b => Assert.Same(binding, b),
                b => Assert.Same(otherBinding, b));
        }

        [Fact]
        public void Test_Enumerate_Bindings_In_Collection_With_Non_Generic_Enumerator()
        {
            Sample firstSample = new Sample();
            Sample secondSample = new Sample(new object());
            IBinding<ISample> binding = from b in Bind<ISample>()
                                        select firstSample;
            IBinding<ISample> otherBinding = from b in Bind<ISample>()
                                             select secondSample;

            IBindingCollection collection = new BindingCollection(new IBinding[] { binding, otherBinding });
            IEnumerable enumerable = collection;
            var enumerator = enumerable.GetEnumerator();

            int count = 0;
            while (enumerator.MoveNext())
            {
                count++;
            }

            Assert.Equal(2, count);
        }

        [Fact]
        public void Test_Implicit_Cast_From_Array_Of_Bindings_Creates_New_Binding_Collection()
        {
            IBinding<ISample> binding = from b in Bind<ISample>()
                                        select new Sample();
            IBinding<ISample> otherBinding = from b in Bind<ISample>()
                                             select new Sample(new object());

            BindingCollection collection = new IBinding[] { binding, otherBinding };

            Assert.Equal(1, collection.TypeCount);
            Assert.Equal(2, collection.BindingCount);
        }

        [Fact]
        public void Test_Implicit_Cast_From_List_Of_Bindings_Creates_New_Binding_Collection()
        {
            IBinding<ISample> binding = from b in Bind<ISample>()
                                        select new Sample();
            IBinding<ISample> otherBinding = from b in Bind<ISample>()
                                             select new Sample(new object());

            BindingCollection collection = new List<IBinding> { binding, otherBinding };

            Assert.Equal(1, collection.TypeCount);
            Assert.Equal(2, collection.BindingCount);
        }

        [Fact]
        public void Test_ToCollection_Extension_Method_Creates_New_Binding_Collection()
        {
            IBinding<ISample> binding = from b in Bind<ISample>()
                                        select new Sample();
            IBinding<ISample> otherBinding = from b in Bind<ISample>()
                                             select new Sample(new object());
            IEnumerable<IBinding> bindings = new List<IBinding> { binding, otherBinding };

            IBindingCollection collection = bindings.ToCollection();

            Assert.Equal(1, collection.TypeCount);
            Assert.Equal(2, collection.BindingCount);
        }
    }
}
