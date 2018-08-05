using IndexedEverything.Contracts;

using NUnit.Framework;

namespace IndexedEverything.Tests.Misc
{
    // ლ(ಠ益ಠ)ლ
    internal class MockIndexed : Indexed
    {
        public MockIndexed(IFunkyFactory funkyFactory = null)
            : base(true, funkyFactory)
        {
        }
    }
}
