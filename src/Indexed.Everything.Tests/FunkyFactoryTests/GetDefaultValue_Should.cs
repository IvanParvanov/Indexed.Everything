using System;
using System.Numerics;

using NUnit.Framework;

namespace Indexed.Everything.Tests.FunkyFactoryTests
{
    [TestFixture]
    internal class GetDefaultValue_Should
    {
        private interface IMockedInterface { }
        private enum MockedEnum { }
        private struct MockedStruct  { }
        private delegate void MockedDelegate();

        private static readonly Type[] TypesToBeTested =
        {
            typeof(int),
            typeof(bool),
            typeof(char),
            typeof(byte),
            typeof(sbyte),
            typeof(short),
            typeof(ushort),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),
            typeof(IntPtr),
            typeof(UIntPtr),
            typeof(decimal),
            typeof(double),
            typeof(float),
            typeof(string),
            typeof(GetDefaultValue_Should),
            typeof(IMockedInterface),
            typeof(MockedDelegate),
            typeof(MockedEnum),
            typeof(MockedStruct),
            typeof(BigInteger)
        };

        private readonly FunkyFactory sut = new FunkyFactory();

        [TestCaseSource(nameof(TypesToBeTested))]
        public void Create_DefaultValue(Type type)
        {
            // Arrange
            object expected = type.IsValueType  ? Activator.CreateInstance(type) : null;

            // Act
            object actual = this.sut.GetDefaultValue(type);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Throw_ArgumentNullException_WhenArgumentIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => this.sut.GetDefaultValue(null));
        }
    }
}
