using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Indexed.Everything.Contracts;

using Moq;

namespace Indexed.Everything.Tests.Misc
{
    internal static class TestHelper
    {
        static TestHelper()
        {
            PropertyInfo[] props = typeof(TestPerson).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            PersonPropertyAccesors = new Dictionary<string, IGetSetPair>()
                                     {
                                         [nameof(TestPerson.Age)] = new GetSetPair(o => ((TestPerson)o).Age,
                                                                                   (o, v) => ((TestPerson)o).Age = (int)v,
                                                                                   props.First(p => p.Name == nameof(TestPerson.Age))),

                                         [nameof(TestPerson.Name)] = new GetSetPair(o => ((TestPerson)o).Name,
                                                                                    (o, v) => ((TestPerson)o).Name = (string)v,
                                                                                    props.First(p => p.Name == nameof(TestPerson.Name))),
                                     };
        }

        public static IReadOnlyDictionary<string, IGetSetPair> PersonPropertyAccesors { get; }

        public static Mock<IFunkyFactory> GetMockedFunkyFactory()
        {
            Mock<IFunkyFactory> factoryMock = new Mock<IFunkyFactory>();
            factoryMock.Setup(x => x.GetPropertyAccessorFuncs(It.Is<Type>(t => t == typeof(TestPerson))))
                       .Returns(() => PersonPropertyAccesors);

            return factoryMock;
        }

        public static IEnumerable<T> ToEnumerable<T>(this IEnumerator enumerator)
        {
            while (enumerator.MoveNext())
            {
                yield return (T)enumerator.Current;
            }
        }
    }
}
