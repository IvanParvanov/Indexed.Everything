using System;

using AutoFixture;

using Indexed.Everything.Contracts;
using Indexed.Everything.Tests.Misc;

using Moq;

using NUnit.Framework;

namespace Indexed.Everything.Tests.IndexedTests
{
    [TestFixture]
    internal class Get_Should
    {
        private readonly Fixture fixture = new Fixture();

        [Test]
        public void Return_CorrectValueFromFunc_WhenIndexer()
        {
            // Arrange
            TestPerson instance = this.fixture.Create<TestPerson>();
            Mock<IFunkyFactory> factoryMock = TestHelper.GetMockedFunkyFactory();
            Indexed sut = new Indexed(instance, true, factoryMock.Object);

            // Act & Assert
            Assert.AreSame(instance.Name, sut[nameof(TestPerson.Name)]);
            Assert.AreEqual(instance.Age, sut[nameof(TestPerson.Age)]);
        }

        [Test]
        public void Return_CorrectValueFromFunc_WhenMethod()
        {
            // Arrange
            TestPerson instance = this.fixture.Create<TestPerson>();
            Mock<IFunkyFactory> factoryMock = TestHelper.GetMockedFunkyFactory();
            Indexed sut = new Indexed(instance, true, factoryMock.Object);

            // Act & Assert
            Assert.AreSame(instance.Name, sut.Get(nameof(TestPerson.Name)));
            Assert.AreEqual(instance.Age, sut.Get(nameof(TestPerson.Age)));
        }

        [Test]
        public void Return_CorrectValueFromFunc_WhenGenericMethod()
        {
            // Arrange
            TestPerson instance = this.fixture.Create<TestPerson>();
            Mock<IFunkyFactory> factoryMock = TestHelper.GetMockedFunkyFactory();
            Indexed sut = new Indexed(instance, true, factoryMock.Object);

            // Act & Assert
            Assert.AreSame(instance.Name, sut.Get<string>(nameof(TestPerson.Name)));
            Assert.AreEqual(instance.Age, sut.Get<int>(nameof(TestPerson.Age)));
        }

        [Test]
        public void Throw_MissingMethodException_WhenShouldThrowOnMissingIsTrueAndMethodNotFound()
        {
            // Arrange
            TestPerson instance = this.fixture.Create<TestPerson>();
            Mock<IFunkyFactory> factoryMock = TestHelper.GetMockedFunkyFactory();
            Indexed sut = new Indexed(instance, true, factoryMock.Object);

            // Act & Assert
            MissingMethodException ex = Assert.Throws<MissingMethodException>(() =>
                                                                              {
                                                                                  object a = sut[TestConst.InvalidPropertyName];
                                                                              });

            StringAssert.Contains(TestConst.InvalidPropertyName, ex.Message);
        }

        [Test]
        public void Throw_MissingMethodException_WhenShouldThrowOnMissingIsTrueAndMethodNotFound_WhenMethod()
        {
            // Arrange
            TestPerson instance = this.fixture.Create<TestPerson>();
            Mock<IFunkyFactory> factoryMock = TestHelper.GetMockedFunkyFactory();
            Indexed sut = new Indexed(instance, true, factoryMock.Object);

            // Act & Assert
            MissingMethodException ex = Assert.Throws<MissingMethodException>(() => sut.Get(TestConst.InvalidPropertyName));
            StringAssert.Contains(TestConst.InvalidPropertyName, ex.Message);
        }

        [Test]
        public void Throw_MissingMethodException_WhenShouldThrowOnMissingIsTrueAndMethodNotFound_WhenGenericMethod()
        {
            // Arrange
            TestPerson instance = this.fixture.Create<TestPerson>();
            Mock<IFunkyFactory> factoryMock = TestHelper.GetMockedFunkyFactory();
            Indexed sut = new Indexed(instance, true, factoryMock.Object);

            // Act & Assert
            MissingMethodException ex = Assert.Throws<MissingMethodException>(() => sut.Get<string>(TestConst.InvalidPropertyName));
            StringAssert.Contains(TestConst.InvalidPropertyName, ex.Message);
        }

        [Test]
        public void ReturnNull_WhenThrowOnMissingIsTrueAndMethodNotFound_WhenGenericMethod()
        {
            // Arrange
            TestPerson instance = this.fixture.Create<TestPerson>();
            Mock<IFunkyFactory> factoryMock = TestHelper.GetMockedFunkyFactory();
            string actual = string.Empty;
            Indexed sut = new Indexed(instance, false, factoryMock.Object);

            // Act & Assert
            Assert.DoesNotThrow(() =>
                                {
                                    actual = sut.Get<string>(TestConst.InvalidPropertyName);
                                });

            Assert.AreEqual(null, actual);
        }

        [Test]
        public void ReturnDefaultValue_WhenShouldThrowOnMissingIsTrueAndMethodNotFound_WhenGenericMethod()
        {
            // Arrange
            TestPerson instance = this.fixture.Create<TestPerson>();
            Mock<IFunkyFactory> factoryMock = TestHelper.GetMockedFunkyFactory();
            int actual = -1;
            Indexed sut = new Indexed(instance, false, factoryMock.Object);

            // Act & Assert
            Assert.DoesNotThrow(() =>
                                {
                                    actual = sut.Get<int>(TestConst.InvalidPropertyName);
                                });

            Assert.AreEqual(0, actual);
        }
    }
}
