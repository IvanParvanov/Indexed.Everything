using Indexed.Everything.Tests.Misc;

using NUnit.Framework;

namespace Indexed.Everything.Tests.IndexedTests
{
    [TestFixture]
    public class ExplicitCastOperator_Should
    {
        [Test]
        public void Cast_From_Indexed()
        {
            // Arrange
            TestPerson expected = TestHelper.GetTestPerson();
            var sut = new Indexed<TestPerson>(expected, true, TestHelper.GetMockedFunkyFactory().Object);

            // Act
            var actual = (TestPerson)sut;

            // Assert
            Assert.AreSame(expected, actual);
        }
    }
}
