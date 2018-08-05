using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Indexed.Everything.Contracts;

namespace Indexed.Everything
{
    public class Indexed<T> : Indexed, IIndexed<T>
    {
        /// <summary>
        /// Creates a new instance of <see cref="Indexed{T}"/>.
        /// </summary>
        /// <param name="instance">The object to index.</param>
        /// <param name="throwOnMissing">Specifies whether <see cref="MissingMethodException"/> is thrown when a property was not found.</param>
        /// <exception cref="ArgumentNullException">Whenever instance is null.</exception>
        public Indexed(T instance, bool throwOnMissing = true)
            : base(instance, throwOnMissing, null)
        {
        }
    }

    public class Indexed : IIndexed
    {
        private const string AlreadyIndexedErrorMessage = "The instance parameter is already " + nameof(Indexed) + "!";

        private static readonly IFunkyFactory DefaultFunkyFactory = new FunkyFactory();

        private readonly object instance;
        private readonly Type type;
        private readonly IReadOnlyDictionary<string, IGetSetPair> valueFactories;
        private readonly IReadOnlyList<string> propertyNames;
        private readonly IFunkyFactory funkyFactory;

        /// <summary>
        /// Creates a new instance of <see cref="Indexed"/>.
        /// </summary>
        /// <param name="instance">The object to index.</param>
        /// <param name="throwOnMissing">Specifies whether <see cref="MissingMethodException"/> is thrown when a property was not found.</param>
        /// <exception cref="ArgumentNullException">Whenever instance is null.</exception>
        public Indexed(object instance, bool throwOnMissing = true)
            : this(instance, throwOnMissing, null)
        {
        }

        internal Indexed(object instance, bool throwOnMissing = true, IFunkyFactory funkyFactory = null)
        {
            this.instance = instance ?? throw new ArgumentNullException(nameof(instance));
            if (instance is Indexed)
            {
                throw new ArgumentException(AlreadyIndexedErrorMessage);
            }

            this.type = instance.GetType();
            this.funkyFactory = funkyFactory ?? DefaultFunkyFactory;
            this.valueFactories = this.funkyFactory.GetPropertyAccessorFuncs(this.type);
            this.propertyNames = new List<string>(this.valueFactories.Keys);
            this.ThrowOnMissing = throwOnMissing;
        }

        internal Indexed(bool throwOnMissing = true, IFunkyFactory funkyFactory = null)
        {
            this.type = this.GetType();
            this.instance = this;
            this.funkyFactory = funkyFactory ?? DefaultFunkyFactory;
            this.valueFactories = this.funkyFactory.GetPropertyAccessorFuncs(this.type);
            this.propertyNames = new List<string>(this.valueFactories.Keys);
            this.ThrowOnMissing = throwOnMissing;
        }

        public IEnumerable<string> Keys => this.propertyNames;

        public IEnumerable<object> Values => this.Keys.Select(key => this[key]);

        public int Count => this.valueFactories.Count;

        public bool ThrowOnMissing { get; set; }

        public object this[string prop]
        {
            get
            {
                this.valueFactories.TryGetValue(prop, out IGetSetPair valueGetter);
                if (valueGetter?.Get != null)
                {
                    return valueGetter.Get(this.instance);
                }

                if (this.ThrowOnMissing)
                {
                    throw new MissingMethodException(this.type.FullName, $"{prop}_Get");
                }

                return null;
            }

            set
            {
                this.valueFactories.TryGetValue(prop, out IGetSetPair valueSetter);
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

                valueSetter?.Set(this.instance, finalValue);
            }
        }

        public KeyValuePair<string, object> this[int index]
        {
            get
            {
                if (index < 0 || index >= this.Count)
                {
                    return this.ThrowOnMissing
                        ? throw new ArgumentOutOfRangeException(nameof(index))
                        : new KeyValuePair<string, object>();
                }
               
                string name = this.propertyNames[index];

                return new KeyValuePair<string, object>(name, this[name]);
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
            return this.valueFactories.ContainsKey(key);
        }

        public bool TryGetValue(string key, out object value)
        {
            this.valueFactories.TryGetValue(key, out IGetSetPair pair);
            if (pair?.Get == null)
            {
                value = null;
                return false;
            }

            value = pair.Get(this.instance);
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
            StringBuilder sb = new StringBuilder();
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
