using System;

using AutoFixture;

using Indexed.Everything.Contracts;
using Indexed.Everything.Tests.Misc;

using Moq;

using NUnit.Framework;

namespace Indexed.Everything.Tests.IndexedTests
{
    [TestFixture]
    internal class Set_Should
    {
        private readonly Fixture fixture = new Fixture();

        [Test]
        public void Set_CorrectValueFromFunc_WhenIndexer()
        {
            // Arrange
            TestPerson instance = new TestPerson();
            string expectedName = this.fixture.Create<string>();
            int expectedAge = this.fixture.Create<int>();
            Mock<IFunkyFactory> factoryMock = TestHelper.GetMockedFunkyFactory();
            Indexed sut = new Indexed(instance, true, factoryMock.Object);

            // Act
            sut[nameof(TestPerson.Name)] = expectedName;
            sut[nameof(TestPerson.Age)] = expectedAge;

            // Assert
            Assert.AreSame(expectedName, instance.Name);
            Assert.AreEqual(expectedAge, instance.Age);
        }

        [Test]
        public void Set_CorrectValueFromFunc_WhenGenericMethod()
        {
            // Arrange
            TestPerson instance = new TestPerson();
            string expectedName = this.fixture.Create<string>();
            int expectedAge = this.fixture.Create<int>();
            Mock<IFunkyFactory> factoryMock = TestHelper.GetMockedFunkyFactory();
            Indexed sut = new Indexed(instance, true, factoryMock.Object);

            // Act
            sut.Set<string>(nameof(TestPerson.Name), expectedName);
            sut.Set<int>(nameof(TestPerson.Age), expectedAge);

            // Assert
            Assert.AreSame(expectedName, instance.Name);
            Assert.AreEqual(expectedAge, instance.Age);
        }

        [Test]
        public void Throw_MissingMethodException_WhenPropertyNotFound()
        {
            // Arrange
            TestPerson instance = new TestPerson();
            string expectedName = this.fixture.Create<string>();
            int expectedAge = this.fixture.Create<int>();
            Mock<IFunkyFactory> factoryMock = TestHelper.GetMockedFunkyFactory();
            Indexed sut = new Indexed(instance, true, factoryMock.Object);

            // Act & Assert
            Assert.Throws<MissingMethodException>(() => sut.Set<string>(TestConst.InvalidPropertyName, expectedName));
            Assert.Throws<MissingMethodException>(() => sut.Set<int>(TestConst.InvalidPropertyName, expectedAge));
        }

        [Test]
        public void NotThrow_MissingMethodException_WhenPropertyNotFoundAndThrowOnMissingIsFalse()
        {
            // Arrange
            TestPerson instance = new TestPerson();
            string expectedName = this.fixture.Create<string>();
            int expectedAge = this.fixture.Create<int>();
            Mock<IFunkyFactory> factoryMock = TestHelper.GetMockedFunkyFactory();
            Indexed sut = new Indexed(instance, false, factoryMock.Object);

            // Act & Assert
            Assert.DoesNotThrow(() => sut.Set<string>(TestConst.InvalidPropertyName, expectedName));
            Assert.DoesNotThrow(() => sut.Set<int>(TestConst.InvalidPropertyName, expectedAge));
        }

        [Test]
        public void NotThrow_WhenPropertyIsValueTypeAndNullIsPassed()
        {
            // Arrange
            TestPerson instance = new TestPerson();
            int expectedAge = this.fixture.Create<int>();
            Mock<IFunkyFactory> factoryMock = TestHelper.GetMockedFunkyFactory();
            factoryMock.Setup(x => x.GetDefaultValue(It.IsAny<Type>()))
                       .Returns(expectedAge);
            Indexed sut = new Indexed(instance, false, factoryMock.Object);

            // Act & Assert
            Assert.DoesNotThrow(() => sut.Set(nameof(TestPerson.Age), (object)null));
            Assert.AreEqual(expectedAge, instance.Age);
        }

        [Test]
        public void Call_GetDefaultValueMethod_OnceWithCorrectParams_WhenPropertyIsValueTypeAndNullIsPassed()
        {
            // Arrange
            TestPerson instance = new TestPerson();
            Mock<IFunkyFactory> factoryMock = TestHelper.GetMockedFunkyFactory();
            IIndexed sut = new Indexed(instance, false, factoryMock.Object);
            factoryMock.Setup(x => x.GetDefaultValue(It.Is<Type>(t => t == typeof(int))))
                       .Returns(0);

            // Act
            sut.Set(nameof(TestPerson.Age), (object)null);

            // Assert
            factoryMock.Verify(x => x.GetDefaultValue(It.Is<Type>(t => t == typeof(int))), Times.Once);
        }
    }
}
