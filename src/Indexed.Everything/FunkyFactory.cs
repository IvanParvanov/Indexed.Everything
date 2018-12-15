using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Caching;

using Indexed.Everything.Contracts;
using Indexed.Everything.Extensions;

namespace Indexed.Everything
{
    internal class FunkyFactory : IFunkyFactory
    {
        private static readonly MemoryCache Cache;
        private static readonly ParameterExpression InstanceParameter;
        private static readonly ParameterExpression ValueParameter;
        private readonly MethodInfo getDefaultValueMethod;

        static FunkyFactory()
        {
            Cache = MemoryCache.Default;
            InstanceParameter = Expression.Parameter(typeof(object));
            ValueParameter = Expression.Parameter(typeof(object));
        }

        internal FunkyFactory()
        {
            this.getDefaultValueMethod = typeof(FunkyFactory).GetMethod(nameof(this.GetDefaultValueGeneric), BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public IReadOnlyDictionary<string, IGetSetPair> GetPropertyAccessorFuncs(Type type)
        {
            type = type ?? throw new ArgumentNullException(nameof(type));

            return Cache.GetOrAdd("æ" + type.FullName,
                                  () =>
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
                                  });
        }

        public object GetDefaultValue(Type type)
        {
            type = type ?? throw new ArgumentNullException(nameof(type));

            return this.GetDefaultValueFactory(type)();
        }

        protected internal IGetSetPair CompilePropertyMethods(PropertyInfo prop)
        {
            prop = prop ?? throw new ArgumentNullException(nameof(prop));

            Type type = prop.ReflectedType;
            Action<object, object> setter = GetCompiledSetter(prop, type);
            Func<object, object> getter = GetCompiledGetter(prop, type);

            return new GetSetPair(getter, setter, prop);
        }

        private Func<object> GetDefaultValueFactory(Type type)
        {
            return Cache.GetOrAdd("¬" + type.FullName,
                                  () =>
                                  {
                                      if (!type.IsValueType)
                                      {
                                          return () => null;
                                      }

                                      MethodInfo method = this.getDefaultValueMethod.MakeGenericMethod(type);

                                      MethodCallExpression call = Expression.Call(Expression.Constant(this), method);
                                      Func<object> result = Expression.Lambda<Func<object>>(Expression.Convert(call, typeof(object))).Compile();
                                      return result;
                                  });
        }

        // ReSharper disable once MemberCanBeMadeStatic.Local
        private T GetDefaultValueGeneric<T>() where T : new()
        {
            return new T();
        }

        private static Action<object, object> GetCompiledSetter(PropertyInfo propInfo, Type type)
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

        private static Func<object, object> GetCompiledGetter(PropertyInfo propInfo, Type type)
        {
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
