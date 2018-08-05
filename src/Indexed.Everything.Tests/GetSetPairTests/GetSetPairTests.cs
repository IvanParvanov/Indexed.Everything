using System;
using System.Reflection;

using Indexed.Everything.Tests.Misc;

using NUnit.Framework;

// ReSharper disable ObjectCreationAsStatement
namespace Indexed.Everything.Tests.GetSetPairTests
{
    [TestFixture]
    internal class GetSetPairTests
    {
        private static readonly Func<object, object> Func = o1 => null;

        private static readonly Action<object, object> Action = (o1, o2) =>
                                                                {
                                                                };

        public string Property { get; }

        [Test]
        public void Ctor_NullPropertyInfo_ShouldThrow()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new GetSetPair(null, Action, null));
        }

        [Test]
        public void Ctor_NullGet_ShouldNotThrow()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => new GetSetPair(null, Action));
        }

        [Test]
        public void Ctor_NullSet_ShouldNotThrow()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => new GetSetPair(Func, null));
        }

        [Test]
        public void GetGet_ShouldGetSameValue_Whenever()
        {
            // Act
            GetSetPair sut = new GetSetPair(Func, null);

            // Assert
            Assert.AreSame(Func, sut.Get);
        }

        [Test]
        public void GetSet_ShouldGetSameValue_Whenever()
        {
            // Act
            GetSetPair sut = new GetSetPair(null, Action);

            // Assert
            Assert.AreSame(Action, sut.Set);
        }

        [Test]
        public void Equals_ShouldReturnTrue_WhenSameInstance()
        {
            // Arrange
            GetSetPair sut = new GetSetPair(null, null);

            // Act
            bool actual = sut.Equals(sut);

            // Assert
            Assert.IsTrue(actual);
        }

        [Test]
        public void Equals_ShouldReturnTrue_WhenEqual()
        {
            // Arrange
            PropertyInfo prop = this.GetType().GetProperty(nameof(this.Property));

            GetSetPair sut = new GetSetPair(Func, Action, prop);
            GetSetPair sut1 = new GetSetPair(Func, Action, prop);

            // Act
            bool actual = sut.Equals(sut1);

            // Assert
            Assert.IsTrue(actual);
        }

        [Test]
        public void Equals_ShouldReturnFalse_WhenNotEqual()
        {
            // Arrange
            PropertyInfo prop = this.GetType().GetProperty(nameof(this.Property));
            GetSetPair sut = new GetSetPair(Func, Action, prop);

            PropertyInfo prop1 = typeof(TestPerson).GetProperty(nameof(TestPerson.Name));
            GetSetPair sut1 = new GetSetPair(Func, Action, prop1);

            // Act
            bool actual = sut.Equals(sut1);

            // Assert
            Assert.IsFalse(actual);
        }

        [Test]
        public void Equals_ShouldReturnFalse_WhenNullIsPassed()
        {
            // Arrange
            GetSetPair sut = new GetSetPair(null, null);

            // Act
            bool actual = sut.Equals(null);

            // Assert
            Assert.IsFalse(actual);
        }
    }
}
