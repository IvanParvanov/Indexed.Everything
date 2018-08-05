using System.Collections;
using System.Collections.Generic;
using System.Linq;

using AutoFixture;

using Indexed.Everything.Contracts;
using Indexed.Everything.Tests.Misc;

using Moq;

using NUnit.Framework;

namespace Indexed.Everything.Tests.IndexedTests
{
    [TestFixture]
    internal class GetEnumerator_Should
    {
        private readonly Fixture fixture = new Fixture();

        [Test]
        public void Return_NotNullEnumerator()
        {
            // Arrange
            TestPerson person = this.fixture.Create<TestPerson>();
            Mock<IFunkyFactory> funkyFactory = TestHelper.GetMockedFunkyFactory();

            Indexed sut = new Indexed(person, true, funkyFactory.Object);

            // Act
            IEnumerator<KeyValuePair<string, object>> result = sut.GetEnumerator();

            // Assert
            Assert.NotNull(result);
        }

        [Test]
        public void Return_WorkingEnumerator()
        {
            // Arrange
            TestPerson person = this.fixture.Create<TestPerson>();
            string expectedName = person.Name;
            int expectedAge = person.Age;
            Mock<IFunkyFactory> funkyFactory = TestHelper.GetMockedFunkyFactory();

            Indexed sut = new Indexed(person, true, funkyFactory.Object);

            // Act
            Dictionary<string, object> actual = sut.ToDictionary(n => n.Key, n => n.Value);

            // Assert
            Assert.IsTrue(actual.ContainsKey(nameof(person.Name)));
            Assert.AreEqual(expectedName, actual[nameof(person.Name)]);

            Assert.IsTrue(actual.ContainsKey(nameof(person.Age)));
            Assert.AreEqual(expectedAge, actual[nameof(person.Age)]);
        }

        [Test]
        public void Return_WorkingEnumerator_WhenImplicit()
        {
            // Arrange
            TestPerson person = this.fixture.Create<TestPerson>();
            Mock<IFunkyFactory> funkyFactory = TestHelper.GetMockedFunkyFactory();

            Indexed sut = new Indexed(person, true, funkyFactory.Object);
            Indexed expected = sut;

            // Act
            IEnumerator result = ((IEnumerable)sut).GetEnumerator();

            // Assert
            Assert.NotNull(result);

            IEnumerable<KeyValuePair<string, object>> actual = result.ToEnumerable<KeyValuePair<string, object>>();
            CollectionAssert.AreEquivalent(expected, actual);
        }
    }
}
