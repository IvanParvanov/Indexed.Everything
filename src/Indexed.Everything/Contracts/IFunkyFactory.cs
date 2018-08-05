using System;
using System.Collections.Generic;

namespace Indexed.Everything.Contracts
{
    /// <summary>
    /// The Funk, The Whole Funk and Nothing But The Funk
    /// </summary>
    internal interface IFunkyFactory
    {
        IReadOnlyDictionary<string, IGetSetPair> GetPropertyAccessorFuncs(Type type);

        object GetDefaultValue(Type type);
    }
}
