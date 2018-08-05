using IndexedEverything.Tests.Misc;

using NUnit.Framework;

namespace IndexedEverything.Tests.IndexedTests
{
    [TestFixture]
    internal class ContainsKey_Should
    {
        [TestCase(nameof(TestPerson.Name))]
        [TestCase(nameof(TestPerson.Age))]
        public void Return_True_WhenContainsKey(string key)
        {
            // Arrange
            Indexed sut = new Indexed(new TestPerson(), true, TestHelper.GetMockedFunkyFactory().Object);

            // Act
            bool result = sut.ContainsKey(key);

            // Assert
            Assert.IsTrue(result);
        }

        [TestCase(TestConst.InvalidPropertyName)]
        public void Return_False_WhenDoesNotContainKey(string key)
        {
            // Arrange
            Indexed sut = new Indexed(new TestPerson(), true, TestHelper.GetMockedFunkyFactory().Object);

            // Act
            bool result = sut.ContainsKey(key);

            // Assert
            Assert.IsFalse(result);
        }
    }
}
