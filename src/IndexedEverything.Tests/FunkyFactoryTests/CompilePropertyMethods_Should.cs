using System;
using System.Linq;
using System.Reflection;

using AutoFixture;

using IndexedEverything.Contracts;

using NUnit.Framework;

namespace IndexedEverything.Tests.FunkyFactoryTests
{
    [TestFixture]
    internal class CompilePropertyMethods_Should
    {
        private readonly FunkyFactory sut;
        private readonly Fixture fixture;
        private string readOnlyProperty;
        private string writeOnlyProperty;

        public string ReadWriteProperty { get; set; }

        public string ReadOnlyProperty
        {
            get => this.readOnlyProperty;
        }

        public string WriteOnlyProperty
        {
            set => this.writeOnlyProperty = value;
        }

        public CompilePropertyMethods_Should()
        {
            this.sut = new FunkyFactory();
            this.fixture = new Fixture();
        }

        [SetUp]
        public void Init()
        {
            this.ReadWriteProperty = null;
            this.readOnlyProperty = null;
            this.WriteOnlyProperty = null;
        }

        [Test]
        public void ThrowArgumentNull_WhenPropertyInfoIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => this.sut.CompilePropertyMethods(null));
        }

        [Test]
        public void ReturnNotNullGetSetPair_WhenPropertyIsReadWrite()
        {
            // Act
            IGetSetPair result = this.sut.CompilePropertyMethods(GetPropertyInfoFor(nameof(this.ReadWriteProperty)));

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Get);
            Assert.NotNull(result.Set);
        }

        [Test]
        public void ReturnWorkingGetterForCorrectProperty_WhenPropertyIsReadWrite()
        {
            // Arrange
            string expected = this.fixture.Create<string>();
            this.ReadWriteProperty = expected;

            // Act
            IGetSetPair result = this.sut.CompilePropertyMethods(GetPropertyInfoFor(nameof(this.ReadWriteProperty)));
            object actual = result.Get(this);

            // Assert
            Assert.AreSame(expected, actual);
        }

        [Test]
        public void ReturnWorkingSetterForCorrectProperty_WhenPropertyIsReadWrite()
        {
            // Arrange
            string expected = this.fixture.Create<string>();

            // Act
            IGetSetPair result = this.sut.CompilePropertyMethods(GetPropertyInfoFor(nameof(this.ReadWriteProperty)));
            result.Set(this, expected);

            // Assert
            Assert.AreSame(expected, this.ReadWriteProperty);
        }

        [Test]
        public void ReturnWorkingPairForCorrectProperty_WhenPropertyIsReadWrite()
        {
            // Arrange
            this.ReadWriteProperty = null;
            string expected = this.fixture.Create<string>();

            // Act
            IGetSetPair result = this.sut.CompilePropertyMethods(GetPropertyInfoFor(nameof(this.ReadWriteProperty)));
            result.Set(this, expected);

            // Assert
            Assert.AreSame(expected, result.Get(this));
        }

        [Test]
        public void ReturnNullSetterWithWorkingGetter_WhenPropertyIsReadonly()
        {
            // Arrange
            string expected = this.fixture.Create<string>();
            this.readOnlyProperty = expected;

            // Act
            IGetSetPair result = this.sut.CompilePropertyMethods(GetPropertyInfoFor(nameof(this.ReadOnlyProperty)));

            // Assert
            Assert.IsNull(result.Set);
            Assert.NotNull(result.Get);
            Assert.AreSame(expected, result.Get(this));
        }

        [Test]
        public void ReturnNullGetterWithWorkingSetter_WhenPropertyIsWriteonly()
        {
            // Arrange
            string expected = this.fixture.Create<string>();

            // Act
            IGetSetPair result = this.sut.CompilePropertyMethods(GetPropertyInfoFor(nameof(this.WriteOnlyProperty)));

            // Assert
            Assert.IsNull(result.Get);
            Assert.NotNull(result.Set);

            result.Set(this, expected);

            Assert.AreSame(expected, this.writeOnlyProperty);
        }

        private static PropertyInfo GetPropertyInfoFor(string name)
        {
            return typeof(CompilePropertyMethods_Should).GetProperties().FirstOrDefault(p => p.Name == name);
        }
    }
}
