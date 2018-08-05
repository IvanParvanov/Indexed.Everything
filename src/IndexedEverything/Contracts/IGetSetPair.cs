using System;
using System.Reflection;

namespace IndexedEverything.Contracts
{
    internal interface IGetSetPair
    {
        Func<object, object> Get { get; }

        Action<object, object> Set { get; }

        PropertyInfo PropertyInfo { get; }
    }
}
