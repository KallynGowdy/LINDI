using Lindi.Tests.Core;

namespace Lindi.Tests.Ioc
{
    public interface INeedMultipleSamples
    {
        ISample[] Samples { get; }
    }

    public class NeedMultipleSamples : INeedMultipleSamples
    {
        public NeedMultipleSamples(ISample[] samples)
        {
            Samples = samples;
        }

        public ISample[] Samples { get; }
    }
}