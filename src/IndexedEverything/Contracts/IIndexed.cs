namespace IndexedEverything.Contracts
{
    // ReSharper disable once UnusedTypeParameter
    public interface IIndexed<T> : IIndexed
    {
    }

    public interface IIndexed : IReadOnlyIndexed
    {
        new object this[string prop] { get; set; }

        void Set<T>(string prop, T value);
    }
}
