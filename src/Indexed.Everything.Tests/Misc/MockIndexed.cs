using Indexed.Everything.Contracts;

using NUnit.Framework;

namespace Indexed.Everything.Tests.Misc
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
