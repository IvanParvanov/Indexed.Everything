using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Caching;

using Indexed.Everything.Contracts;

namespace Indexed.Everything
{
    internal class FunkyFactory : IFunkyFactory
    {
        private static readonly MemoryCache Cache;
        private static readonly CacheItemPolicy CacheItemPolicy;
        private static readonly IReadOnlyDictionary<Type, Func<object>> ValueTypeConstructors;
        private static readonly ParameterExpression InstanceParameter;
        private static readonly ParameterExpression ValueParameter;
        private readonly MethodInfo constructorMethod;

        static FunkyFactory()
        {
            Cache = MemoryCache.Default;
            CacheItemPolicy = new CacheItemPolicy { SlidingExpiration = TimeSpan.FromMinutes(15) };
            InstanceParameter = Expression.Parameter(typeof(object));
            ValueParameter = Expression.Parameter(typeof(object));
            ValueTypeConstructors = new Dictionary<Type, Func<object>>
                                    {
                                        { typeof(bool), () => new bool() },
                                        { typeof(char), () => new char() },
                                        { typeof(byte), () => new byte() },
                                        { typeof(sbyte), () => new sbyte() },
                                        { typeof(short), () => new short() },
                                        { typeof(ushort), () => new ushort() },
                                        { typeof(int), () => new int() },
                                        { typeof(uint), () => new uint() },
                                        { typeof(long), () => new long() },
                                        { typeof(ulong), () => new ulong() },
                                        { typeof(double), () => new double() },
                                        { typeof(decimal), () => new decimal() },
                                        { typeof(float), () => new float() },
                                        { typeof(string), () => null },
                                        { typeof(IntPtr), () => new IntPtr() },
                                        { typeof(UIntPtr), () => new UIntPtr() },
                                    };
            ;
        }

        public FunkyFactory()
        {
            this.constructorMethod = typeof(FunkyFactory).GetMethod(nameof(this.GetDefaultValueGeneric), BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public IReadOnlyDictionary<string, IGetSetPair> GetPropertyAccessorFuncs(Type type)
        {
            type = type ?? throw new ArgumentNullException(nameof(type));

            IReadOnlyDictionary<string, IGetSetPair> Get()
            {
                var factories = new Dictionary<string, IGetSetPair>();

                IEnumerable<PropertyInfo> props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                      .Where(p => p.DeclaringType?.FullName != typeof(Indexed).FullName);
                foreach (PropertyInfo prop in props)
                {
                    IGetSetPair functions = this.CompilePropertyMethods(prop);
                    factories.Add(prop.Name, functions);
                }

                return factories;
            }

            return GetOrAdd(type, "æ", Get);
        }

        public object GetDefaultValue(Type type)
        {
            type = type ?? throw new ArgumentNullException(nameof(type));

            return ValueTypeConstructors.TryGetValue(type, out Func<object> ctor)
                ? ctor()
                : this.GetConstructor(type)();
        }

        internal IGetSetPair CompilePropertyMethods(PropertyInfo prop)
        {
            prop = prop ?? throw new ArgumentNullException(nameof(prop));

            Type type = prop.ReflectedType;
            Action<object, object> setter = this.GetCompiledSetter(prop, type);
            Func<object, object> getter = this.GetCompiledGetter(prop, type);

            return new GetSetPair(getter, setter, prop);
        }

        private Func<object> GetConstructor(Type type)
        {
            Func<object> CompileConstructor()
            {
                if (!type.IsValueType)
                {
                    return () => null;
                }

                MethodInfo method = this.constructorMethod.MakeGenericMethod(type);

                Func<object> result = Expression.Lambda<Func<object>>(Expression.Convert(Expression.Call(Expression.Constant(this), method), typeof(object))).Compile();
                return result;
            }

            return GetOrAdd(type, "¬", CompileConstructor);
        }

        private T GetDefaultValueGeneric<T>() where T : new()
        {
            return new T();
        }

        private Action<object, object> GetCompiledSetter(PropertyInfo propInfo, Type type)
        {
            type = type ?? throw new ArgumentNullException(nameof(type));
            propInfo = propInfo ?? throw new ArgumentNullException(nameof(propInfo));

            MethodInfo setMethod = propInfo.GetSetMethod();
            if (setMethod == null)
            {
                return null;
            }

            UnaryExpression convertedInstance = Expression.Convert(InstanceParameter, type);
            MemberExpression propertyAccess = Expression.Property(convertedInstance, setMethod);

            UnaryExpression convertedValue = Expression.Convert(ValueParameter, setMethod.GetParameters()[0].ParameterType);

            BinaryExpression assignment = Expression.Assign(propertyAccess, convertedValue);

            Action<object, object> compiledSet = Expression.Lambda<Action<object, object>>(assignment, InstanceParameter, ValueParameter)
                                                           .Compile();

            return compiledSet;
        }

        private Func<object, object> GetCompiledGetter(PropertyInfo propInfo, Type type)
        {
            type = type ?? throw new ArgumentNullException(nameof(type));
            propInfo = propInfo ?? throw new ArgumentNullException(nameof(propInfo));

            MethodInfo getMethod = propInfo.GetGetMethod();
            if (getMethod == null)
            {
                return null;
            }

            UnaryExpression convertedInstance = Expression.Convert(InstanceParameter, type);

            MethodCallExpression callGet = Expression.Call(convertedInstance, getMethod);
            Expression conversion = Expression.Convert(callGet, typeof(object));

            Func<object, object> compiledGet = Expression.Lambda<Func<object, object>>(conversion, InstanceParameter)
                                                         .Compile();

            return compiledGet;
        }

        private static T GetOrAdd<T>(Type t, string cachePrefix, Func<T> getter) where T : class
        {
            string key = cachePrefix + t.FullName;
            if (Cache.Contains(key))
            {
                return Cache.Get(key) as T;
            }

            T value = getter();
            Cache.Add(key, value, CacheItemPolicy);
            return value;
        }
    }
}
