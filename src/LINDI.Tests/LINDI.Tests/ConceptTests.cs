using System;
using Xunit;

namespace LINDI.Tests
{
    public class ConceptTests
    {
        public IBinding<T> Bind<T>()
        {
            return null;
        }

        public void TestBuildBindingThroughLinq()
        {
            // Bind ISample to Sample as a default
            IBinding<Sample> binding = from type in Bind<ISample>()
                          select type as Sample;

            // Bind ISample to Sample with factory
            IBinding<Sample> b5 = from type in Bind<ISample>()
                     select new Sample();

            // Bind ISample to Sample with factory needing more injections
            IBinding<Sample> b6 = from type in Bind<ISample>()
                     select new Sample(default(object));

            ISample sample = b6.Resolve();

            // Conditional binding
            // Bind ISample to OtherSample only if the given function is true
            IBinding<OtherSample> b1 = from type in Bind<ISample>()
                     where Environment.Is64BitOperatingSystem // Only bind if we are running on a 64 bit operating system
                     select type as OtherSample;
            // Bind ISample to Sample if it is being injected into IHasSample
            IBinding<Sample> b4 = from type in Bind<ISample>()
                     where type is IHasSample
                     select type as Sample;

            // TODO: Add case for merging multiple bindings for the same interface into a single binding that
            //       allows ordering for priority (which to check first)

            // Container logic
            // Whether to reuse existing types or not
            // This example specifies a singleton
            IBinding<Sample> b2 = Bind<ISample>().GroupBy(t => true).Select(t => t as Sample);
            IBinding<Sample> b3 = from type in Bind<ISample>()
                     group type by true into t
                     select t as Sample;

            sample = b3.Resolve();
            var s = b3.Resolve();
            Assert.Same(sample, s);
        }

    }
}