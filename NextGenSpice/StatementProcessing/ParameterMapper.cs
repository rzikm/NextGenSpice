using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace NextGenSpice
{
    /// <summary>
    /// Helper class for mapping statement parameters onto properties of an object.
    /// </summary>
    /// <typeparam name="TParam"></typeparam>
    public class ParameterMapper<TParam>
    {
        public ParameterMapper()
        {
            settersByKey = new Dictionary<string, Action<TParam, double>>();
            settersByIndex = new Dictionary<int, Action<TParam, double>>();
        }

        /// <summary>
        /// Count of parameters mapped by Key.
        /// </summary>
        public int ByKeyCount => settersByKey.Count;

        /// <summary>
        /// Count of parameters mapped by index.
        /// </summary>
        public int ByIndexCount => settersByIndex.Count;

        private readonly Dictionary<string, Action<TParam, double>> settersByKey;
        private readonly Dictionary<int, Action<TParam, double>> settersByIndex;

        /// <summary>
        /// Instance of an object to which parameters are mapped.
        /// </summary>
        public TParam Target { get; set; }

        private static PropertyInfo GetMappedProperty(Expression<Func<TParam, double>> mapping)
        {
            if (!((mapping.Body as MemberExpression)?.Member is PropertyInfo prop))
                throw new NotSupportedException($"Mapping expression must be simple member access expression lambda");
            return prop;
        }

        /// <summary>
        /// Returns true if there is a mapping from given index onto a property.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public bool HasIndex(int i)
        {
            return settersByIndex.ContainsKey(i);
        }

        /// <summary>
        /// Returns true if there is a mapping from given key onto a property.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool HasKey(string key)
        {
            return settersByKey.ContainsKey(key);
        }

        /// <summary>
        /// Maps given key onto given property.
        /// </summary>
        /// <param name="mapping"></param>
        /// <param name="paramKey"></param>
        public void Map(Expression<Func<TParam, double>> mapping, string paramKey)
        {
            PropertyInfo prop =  GetMappedProperty(mapping);
            settersByKey.Add(paramKey, (target, value) => prop.SetValue(target, value));
        }

        /// <summary>
        /// Maps given key onto given property with specified conversion function.
        /// </summary>
        /// <param name="mapping"></param>
        /// <param name="paramKey"></param>
        /// <param name="transform"></param>
        public void Map(Expression<Func<TParam, double>> mapping, string paramKey, Func<double, double> transform)
        {
            PropertyInfo prop = GetMappedProperty(mapping);
            settersByKey.Add(paramKey, (target, value) => prop.SetValue(target, transform(value)));
        }

        /// <summary>
        /// Maps given key onto given index.
        /// </summary>
        /// <param name="mapping"></param>
        /// <param name="index"></param>
        public void Map(Expression<Func<TParam, double>> mapping, int index)
        {
            PropertyInfo prop = GetMappedProperty(mapping);
            settersByIndex.Add(index, (target, value) => prop.SetValue(target, value));
        }

        /// <summary>
        /// Maps given key onto given index with specified conversion function.
        /// </summary>
        /// <param name="mapping"></param>
        /// <param name="index"></param>
        /// <param name="transform"></param>
        public void Map(Expression<Func<TParam, double>> mapping, int index, Func<double, double> transform)
        {
            PropertyInfo prop = GetMappedProperty(mapping);
            settersByIndex.Add(index, (target, value) => prop.SetValue(target, transform(value)));
        }

        /// <summary>
        /// Sets value of parameter that maps to given key to specified value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Set(string key, double value)
        {
            if (!settersByKey.TryGetValue(key, out var setter)) throw new ArgumentException($"Parameter '{key}' is not mapped to any property");
            setter(Target, value);
        }
        /// <summary>
        /// Sets value of parameter that maps to given index to specified value
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void Set(int index, double value)
        {
            if (!settersByIndex.TryGetValue(index, out var setter)) throw new ArgumentException($"Index '{index}' is not mapped to any property");
            setter(Target, value);
        }
    }
}