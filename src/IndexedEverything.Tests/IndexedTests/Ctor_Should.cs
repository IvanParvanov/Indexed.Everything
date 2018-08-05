﻿using System;
using System.Collections.Generic;

using AutoFixture;

using IndexedEverything.Contracts;
using IndexedEverything.Tests.Misc;

using Moq;

using NUnit.Framework;

namespace IndexedEverything.Tests.IndexedTests
{
    [TestFixture]
    internal class Ctor_Should
    {
        private readonly Fixture fixture = new Fixture();

        [Test]
        public void Throw_ArgumentNullException_WhenInstanceIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new Indexed(null, false));
        }

        [Test]
        public void Throw_ArgumentNullException_WhenInstanceIsNull_()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new Indexed<TestPerson>(null));
        }

        [Test]
        public void DoesNotThrow_ArgumentNullException_WhenInstanceIsNotNull()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => new Indexed<object>(new object()));
        }

        [Test]
        public void Throw_ArgumentException_WhenInstanceIsIndexed()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new Indexed(new Indexed(1, false), false));
        }

        [Test]
        public void Call_FactoryBuildGetSetFuncsMethodWithCorrectParamsOnce_WhenInherited()
        {
            // Arrange
            Mock<IFunkyFactory> factoryMock = TestHelper.GetMockedFunkyFactory();
            factoryMock.Setup(f => f.GetPropertyAccessorFuncs(It.IsAny<Type>()))
                       .Returns(new Dictionary<string, IGetSetPair>());

            // Act
            MockIndexed sut = new MockIndexed(factoryMock.Object);

            // Assert
            factoryMock.Verify(x => x.GetPropertyAccessorFuncs(It.Is<Type>(t => t.FullName == typeof(MockIndexed).FullName)), Times.Once);
        }

        [Test]
        public void Call_FactoryBuildGetSetFuncsMethodWithCorrectparamsOnce()
        {
            // Arrange
            TestPerson instance = this.fixture.Create<TestPerson>();
            Mock<IFunkyFactory> factoryMock = TestHelper.GetMockedFunkyFactory();

            // Act
            Indexed sut = new Indexed(instance, true, factoryMock.Object);

            // Assert
            factoryMock.Verify(x => x.GetPropertyAccessorFuncs(It.Is<Type>(t => t.FullName == typeof(TestPerson).FullName)), Times.Once);
        }

        [Test]
        public void ToString_ShouldAddAllPropertiesToOutput()
        {
            // Arrange
            TestPerson instance = this.fixture.Create<TestPerson>();
            Mock<IFunkyFactory> factoryMock = TestHelper.GetMockedFunkyFactory();
            Indexed sut = new Indexed(instance, true, factoryMock.Object);

            // Act
            string actual = sut.ToString();

            // Assert
            StringAssert.Contains(nameof(TestPerson.Name), actual);
            StringAssert.Contains(instance.Name, actual);
            StringAssert.Contains(nameof(TestPerson.Age), actual);
            StringAssert.Contains(instance.Age.ToString(), actual);
        }
    }
}
