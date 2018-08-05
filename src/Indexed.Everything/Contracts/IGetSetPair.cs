using System;
using System.Reflection;

namespace Indexed.Everything.Contracts
{
    internal interface IGetSetPair
    {
        Func<object, object> Get { get; }

        Action<object, object> Set { get; }

        PropertyInfo PropertyInfo { get; }
    }
}
