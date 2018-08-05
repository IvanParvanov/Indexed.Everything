using System;
using System.Collections.Generic;

using AutoFixture;

using Indexed.Everything.Contracts;
using Indexed.Everything.Tests.Misc;

using Moq;

using NUnit.Framework;

namespace Indexed.Everything.Tests.IndexedTests
{
    [TestFixture]
    internal class Indexer_Should
    {
        private readonly Fixture fixture = new Fixture();

        [Test]
        public void Return_CorrectValue()
        {
            // Arrange
            TestPerson person = this.fixture.Create<TestPerson>();
            Mock<IFunkyFactory> funkyFactory = TestHelper.GetMockedFunkyFactory();

            Indexed sut = new Indexed(person, true, funkyFactory.Object);

            // Act
            KeyValuePair<string, object> actualAge = sut[0];
            KeyValuePair<string, object> actualName = sut[1];

            // Assert
            Assert.AreEqual(person.Age, actualAge.Value);
            Assert.AreEqual(nameof(person.Age), actualAge.Key);

            Assert.AreSame(person.Name, actualName.Value);
            Assert.AreEqual(nameof(person.Name), actualName.Key);
        }

        [TestCase(int.MinValue)]
        [TestCase(int.MaxValue)]
        public void Return_EmptyKeyValuePair_WhenThrowOnMissingIsFalseAndIndexOutOfRange(int outOfRangeIndex)
        {
            // Arrange
            Mock<IFunkyFactory> funkyFactory = TestHelper.GetMockedFunkyFactory();
            KeyValuePair<string, object> actual = new KeyValuePair<string, object>();

            Indexed sut = new Indexed(new TestPerson(), false, funkyFactory.Object);

            // Act & Assert
            Assert.DoesNotThrow(() => actual = sut[outOfRangeIndex]);
            Assert.AreSame(actual.Value, null);
            Assert.AreSame(actual.Key, null);
        }

        [TestCase(int.MinValue)]
        [TestCase(int.MaxValue)]
        public void Throw_ArgumentOutOfRangeException_WhenThrowOnMissingIsTrueAndIndexOutOfRange(int outOfRangeIndex)
        {
            // Arrange
            Mock<IFunkyFactory> funkyFactory = TestHelper.GetMockedFunkyFactory();

            Indexed sut = new Indexed(new TestPerson(), true, funkyFactory.Object);

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                                                       {
                                                           var actual = sut[outOfRangeIndex];
                                                       });
        }
    }
}
