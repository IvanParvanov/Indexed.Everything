using System.Reflection;

using Indexed.Everything.Tests.Misc;

using NUnit.Framework;

namespace Indexed.Everything.Tests.IndexedTests
{
    [TestFixture]
    public class Count_Should
    {
        [Test]
        public void Return_CorrectPublicInstancePropertyCount_()
        {
            // Arrange
            TestPerson testPerson = TestHelper.GetTestPerson();
            int expected = typeof(TestPerson).GetProperties(BindingFlags.Public | BindingFlags.Instance).Length;
            var sut = new Indexed(testPerson, true, TestHelper.GetMockedFunkyFactory().Object);

            // Act
            int actual = sut.Count;

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
