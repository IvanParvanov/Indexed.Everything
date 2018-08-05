using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using IndexedEverything.Contracts;

namespace IndexedEverything
{
    internal class FunkyFactory : IFunkyFactory
    {
        private static readonly ConcurrentDictionary<string, IReadOnlyDictionary<string, IGetSetPair>> ValueFactoryCache;
        private static readonly IReadOnlyDictionary<Type, Func<object>> PrimitiveTypeConstructors;
        private static readonly ParameterExpression InstanceParameter;
        private static readonly ParameterExpression ValueParameter;

        static FunkyFactory()
        {
            ValueFactoryCache = new ConcurrentDictionary<string, IReadOnlyDictionary<string, IGetSetPair>>();
            InstanceParameter = Expression.Parameter(typeof(object));
            ValueParameter = Expression.Parameter(typeof(object));
            PrimitiveTypeConstructors = new Dictionary<Type, Func<object>>
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
                                            { typeof(float), () => new float() },
                                            { typeof(IntPtr), () => new IntPtr() },
                                            { typeof(UIntPtr), () => new UIntPtr() },
                                        };
        }

        public IReadOnlyDictionary<string, IGetSetPair> GetPropertyAccessorFuncs(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (ValueFactoryCache.TryGetValue(type.FullName, out IReadOnlyDictionary<string, IGetSetPair> valueFactories))
            {
                return valueFactories;
            }

            Dictionary<string, IGetSetPair> factories = new Dictionary<string, IGetSetPair>();

            IEnumerable<PropertyInfo> props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                  .Where(p => p.DeclaringType?.FullName != typeof(Indexed).FullName);
            foreach (PropertyInfo prop in props)
            {
                IGetSetPair functions = this.CompilePropertyMethods(prop);
                factories.Add(prop.Name, functions);
            }

            ValueFactoryCache[type.FullName] = factories;

            return factories;
        }

        public object GetDefaultValue(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return PrimitiveTypeConstructors.TryGetValue(type, out Func<object> ctor)
                ? ctor()
                : null;
        }

        internal IGetSetPair CompilePropertyMethods(PropertyInfo prop)
        {
            if (prop == null)
            {
                throw new ArgumentNullException(nameof(prop));
            }

            Type type = prop.ReflectedType;
            Action<object, object> setter = this.GetCompiledSetter(prop, type);
            Func<object, object> getter = this.GetCompiledGetter(prop, type);

            return new GetSetPair(getter, setter, prop);
        }

        protected virtual Action<object, object> GetCompiledSetter(PropertyInfo propInfo, Type type)
        {
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

        protected virtual Func<object, object> GetCompiledGetter(PropertyInfo propInfo, Type type)
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
    }
}
