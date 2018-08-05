using System;
using System.Collections.Generic;
using System.Linq;

using AutoFixture;

using IndexedEverything.Contracts;
using IndexedEverything.Tests.Misc;

using NUnit.Framework;

namespace IndexedEverything.Tests.FunkyFactoryTests
{
    [TestFixture]
    internal class BuildGetSetFuncs_Should
    {
        private readonly FunkyFactory sut = new FunkyFactory();
        private readonly Fixture fixture = new Fixture();

        [Test]
        public void ThrowArgumentNullException_WhenTypeIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => this.sut.GetPropertyAccessorFuncs(null));
        }

        [Test]
        public void ReturnGetSetPairs_ForEachPublicInstanceProperty()
        {
            // Arrange
            IReadOnlyDictionary<string, IGetSetPair> expected = TestHelper.PersonPropertyAccesors;

            // Act
            IReadOnlyDictionary<string, IGetSetPair> result = this.sut.GetPropertyAccessorFuncs(typeof(TestPerson));

            // Assert
            CollectionAssert.AreEquivalent(expected.Keys, result.Keys);
            CollectionAssert.AreEquivalent(expected.Values, result.Values);
        }

        [Test]
        public void ReturnWorkingGetters_ForEachPublicInstanceProperty()
        {
            // Arrange
            TestPerson instance = this.fixture.Create<TestPerson>();
            IReadOnlyDictionary<string, IGetSetPair> expected = TestHelper.PersonPropertyAccesors;

            // Act
            IReadOnlyDictionary<string, IGetSetPair> result = this.sut.GetPropertyAccessorFuncs(typeof(TestPerson));

            // Assert
            foreach (string key in expected.Keys)
            {
                IGetSetPair actualFuncs = result[key];
                IGetSetPair expectedFuncs = expected[key];

                Assert.AreEqual(expectedFuncs.Get(instance), actualFuncs.Get(instance));
            }
        }

        [Test]
        public void ReturnWorkingSetters_ForEachPublicInstanceProperty()
        {
            // Arrange
            var i = 0;
            object[] expectedValues = { this.fixture.Create<int>(), this.fixture.Create<string>() };
            TestPerson instance = new TestPerson();
            IReadOnlyDictionary<string, IGetSetPair> expected = TestHelper.PersonPropertyAccesors
                                                                          .OrderBy(x => x.Key)
                                                                          .ToDictionary(x => x.Key, x => x.Value);

            // Act
            IReadOnlyDictionary<string, IGetSetPair> result = this.sut.GetPropertyAccessorFuncs(typeof(TestPerson))
                                                                  .OrderBy(x => x.Key)
                                                                  .ToDictionary(x => x.Key, x => x.Value);

            // Assert
            Assert.That(() => result.Keys.Count() == expectedValues.Length);
            foreach (string key in result.Keys)
            {
                IGetSetPair actualFuncs = result[key];
                actualFuncs.Set(instance, expectedValues[i]);
                object actualValue = expected[key].Get(instance);

                Assert.AreEqual(expectedValues[i], actualValue);
                i++;
            }
        }
    }
}
