namespace Lindi.Tests.Core
{
    public class HasSample : IHasSample
    {
        public HasSample(ISample sample)
        {
            Sample = sample;
        }

        public ISample Sample { get; }
    }
}