using System;
using System.Reflection;

using IndexedEverything.Contracts;

namespace IndexedEverything
{
    internal class GetSetPair : IGetSetPair, IEquatable<IGetSetPair>
    {
        internal GetSetPair(Func<object, object> get, Action<object, object> set, PropertyInfo propInfo)
            : this(get, set)
        {
            this.PropertyInfo = propInfo ?? throw new ArgumentNullException(nameof(propInfo));
        }

        internal GetSetPair(Func<object, object> get, Action<object, object> set)
        {
            this.Get = get;
            this.Set = set;
        }

        public Func<object, object> Get { get; }

        public Action<object, object> Set { get; }

        public PropertyInfo PropertyInfo { get; }

        public bool Equals(IGetSetPair other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return string.Equals(this.PropertyInfo?.Name, other.PropertyInfo?.Name, StringComparison.Ordinal)
                   && this.PropertyInfo?.ReflectedType == other.PropertyInfo?.ReflectedType;
        }
    }
}
