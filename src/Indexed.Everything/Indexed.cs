using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

using Indexed.Everything.Contracts;

namespace Indexed.Everything
{
    public class Indexed<T> : Indexed, IIndexed<T>
    {
        /// <summary>
        ///     Creates a new instance of <see cref="Indexed{T}" />.
        /// </summary>
        /// <param name="instance">The object to index.</param>
        /// <param name="throwOnMissing">
        ///     Specifies whether <see cref="MissingMethodException" /> is thrown when a property was not
        ///     found.
        /// </param>
        /// <exception cref="ArgumentNullException">Whenever <paramref name="instance"/> is null.</exception>
        public Indexed(T instance, bool throwOnMissing = true)
            : base(instance, throwOnMissing, null, typeof(T))
        {
        }

        internal Indexed(T instance, bool throwOnMissing, IFunkyFactory funkyFactory)
            : base(instance, throwOnMissing, funkyFactory, typeof(T))
        {
        }

        /// <summary>
        ///     Converts the <see cref="Indexed{T}"/> by returning the underlying instance.
        /// </summary>
        /// <param name="indexed">The <see cref="Indexed{T}"/> to convert.</param>
        public static explicit operator T(Indexed<T> indexed)
        {
            return (T)indexed.Instance;
        }
    }

    public class Indexed : IIndexed
    {
        private const string AlreadyIndexedErrorMessage = "The instance parameter is already " + nameof(Indexed) + "!";

        private static readonly IFunkyFactory DefaultFunkyFactory = new FunkyFactory();

        private readonly IFunkyFactory funkyFactory;
        private readonly Type type;
        private readonly IReadOnlyDictionary<string, IGetSetPair> propertyAccessors;

        /// <summary>
        ///     Creates a new instance of <see cref="Indexed" />.
        /// </summary>
        /// <param name="instance">The object to index.</param>
        /// <param name="throwOnMissing">
        ///     Specifies whether <see cref="MissingMethodException" /> is thrown when a property was not
        ///     found.
        /// </param>
        /// <exception cref="ArgumentNullException">Whenever instance is null.</exception>
        public Indexed(object instance, bool throwOnMissing = true)
            : this(instance, throwOnMissing, null)
        {
        }

        internal Indexed(object instance, bool throwOnMissing = true, IFunkyFactory funkyFactory = null, Type type = null)
        {
            this.Instance = instance ?? throw new ArgumentNullException(nameof(instance));
            if (instance is Indexed)
            {
                throw new ArgumentException(AlreadyIndexedErrorMessage);
            }

            this.type = type ?? instance.GetType();
            this.funkyFactory = funkyFactory ?? DefaultFunkyFactory;
            this.propertyAccessors = this.funkyFactory.GetPropertyAccessorFuncs(this.type);
            this.ThrowOnMissing = throwOnMissing;
        }

        public IEnumerable<string> Keys => this.propertyAccessors.Keys;

        public IEnumerable<object> Values => this.Keys.Select(key => this[key]);

        public int Count => this.propertyAccessors.Count;

        public bool ThrowOnMissing { get; set; }

        protected internal object Instance { get; }

        [SuppressMessage("Microsoft.Design", "CA1065", Justification = "An exception is thrown only when the user has explicitly set throwOnMissing.")]
        public object this[string prop]
        {
            get
            {
                this.propertyAccessors.TryGetValue(prop, out IGetSetPair valueGetter);
                if (valueGetter?.Get != null)
                {
                    return valueGetter.Get(this.Instance);
                }

                if (this.ThrowOnMissing)
                {
                    throw new MissingMethodException(this.type.FullName, $"{prop}_Get");
                }

                return null;
            }

            set
            {
                this.propertyAccessors.TryGetValue(prop, out IGetSetPair valueSetter);
                if (this.ThrowOnMissing && valueSetter?.Set == null)
                {
                    throw new MissingMethodException(this.type.FullName, $"{prop}_Set");
                }

                object finalValue = value;
                if (finalValue == null
                    && valueSetter != null)
                {
                    finalValue = this.funkyFactory.GetDefaultValue(valueSetter.PropertyInfo.PropertyType);
                }

                valueSetter?.Set(this.Instance, finalValue);
            }
        }

        public object Get(string prop)
        {
            return this[prop];
        }

        public T Get<T>(string prop)
        {
            object value = this[prop];

            // ReSharper disable once MergeConditionalExpression
            return value == null ? default(T) : (T)value;
        }

        public void Set<T>(string prop, T value)
        {
            this[prop] = value == null ? default(T) : value;
        }

        public bool ContainsKey(string key)
        {
            return this.propertyAccessors.ContainsKey(key);
        }

        public bool TryGetValue(string key, out object value)
        {
            this.propertyAccessors.TryGetValue(key, out IGetSetPair pair);
            if (pair?.Get == null)
            {
                value = null;
                return false;
            }

            value = pair.Get(this.Instance);
            return true;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return this.Keys.Select(x => new KeyValuePair<string, object>(x, this[x]))
                       .GetEnumerator();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(this.type.Name);
            sb.Append(Environment.NewLine);
            foreach (string name in this.Keys)
            {
                sb.Append(name);
                sb.Append(": ");
                sb.Append(this[name]);
                sb.Append(Environment.NewLine);
            }

            return sb.ToString().TrimEnd(' ');
        }
    }
}
