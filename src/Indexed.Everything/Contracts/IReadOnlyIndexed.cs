using System.Collections.Generic;

namespace Indexed.Everything.Contracts
{
    public interface IReadOnlyIndexed : IReadOnlyDictionary<string, object>
    {
        bool ThrowOnMissing { get; set; }

        KeyValuePair<string, object> this[int index] { get; }

        object Get(string prop);

        T Get<T>(string prop);
    }
}
