using System.Linq;
using System.Reflection;

using Indexed.Everything.Tests.Misc;

using NUnit.Framework;

namespace Indexed.Everything.Tests.IndexedTests
{
    [TestFixture]
    public class Values_Should
    {
        [Test]
        public void Return_CorrectValues()
        {
            // Arrange
            TestPerson testPerson = TestHelper.GetTestPerson();
            var expected = typeof(TestPerson).GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(p => p.GetValue(testPerson)).ToList();
            var sut = new Indexed(testPerson, true, TestHelper.GetMockedFunkyFactory().Object);

            // Act
            var actual = sut.Values;

            // Assert
            CollectionAssert.AreEquivalent(expected, actual);
        }
    }
}
