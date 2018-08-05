using AutoFixture;

using IndexedEverything.Contracts;
using IndexedEverything.Tests.Misc;

using Moq;

using NUnit.Framework;

namespace IndexedEverything.Tests.IndexedTests
{
    [TestFixture]
    internal class TryGetValue_Should
    {
        private readonly Fixture fixture = new Fixture();

        [Test]
        public void Return_FalseAndGetNull_WhenKeyDoesNotExist()
        {
            // Arrange
            object result;
            Mock<IFunkyFactory> factory = TestHelper.GetMockedFunkyFactory();
            TestPerson person = this.fixture.Create<TestPerson>();
            Indexed sut = new Indexed(person, true, factory.Object);

            // Act
            bool returnValue = sut.TryGetValue(TestConst.InvalidPropertyName, out result);

            // Assert
            Assert.IsNull(result);
            Assert.IsFalse(returnValue);
        }

        [Test]
        public void Return_TrueAndGetCorrectValue_WhenKeyDoesExist()
        {
            // Arrange
            object result;
            TestPerson person = this.fixture.Create<TestPerson>();
            Mock<IFunkyFactory> factory = TestHelper.GetMockedFunkyFactory();
            Indexed sut = new Indexed(person, true, factory.Object);

            // Act & Assert
            bool returnValue = sut.TryGetValue(nameof(TestPerson.Name), out result);

            Assert.IsTrue(returnValue);

            Assert.NotNull(result);
            string actualName = result as string;
            Assert.AreSame(person.Name, actualName);

            sut.TryGetValue(nameof(TestPerson.Age), out result);

            Assert.NotNull(result);
            int actualAge = (int)result;
            Assert.AreEqual(person.Age, actualAge);
        }
    }
}
