using System;
using Lindi.Core;
using Lindi.Core.Linq;
using Xunit;
using static Lindi.Core.LindiMethods;

namespace Lindi.Tests
{
    public class ConceptTests
    {
        public void TestBuildBindingThroughLinq()
        {
            // Bind ISample to Sample with specified constructor
            IBinding<ISample> b5 = from type in Bind<ISample>()
                     select new Sample();

            // Bind ISample to Sample with specified constructor needing more injections
            IBinding<ISample> b6 = from type in Bind<ISample>()
                     select new Sample(default(object));

            //// Conditional binding
            //// Bind ISample to OtherSample only if the given function is true
            //IBinding<ISample> b1 = from type in Bind<ISample>()
            //         where Environment.Is64BitOperatingSystem // Only bind if we are running on a 64 bit operating system
            //         select type as OtherSample;

            //// Bind ISample to Sample if it is being injected into IHasSample
            //IBinding<ISample> b4 = from type in Bind<ISample>()
            //         where type is IHasSample
            //         select type as Sample;

            // TODO: Add case for merging multiple bindings for the same interface into a single binding that
            //       allows ordering for priority (which to check first)

            // Scope logic
            // Whether to reuse existing types or not
            // This example specifies a singleton
            //IBinding<ISample> b2 = Bind<ISample>().GroupBy(t => true).Select(t => t as Sample);
            //IBinding<ISample> b3 = from type in Bind<ISample>()
            //         group type by true into t
            //         select t as Sample;
            
        }

    }
}