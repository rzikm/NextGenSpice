using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace NextGenSpice
{
    public class ParameterMapper<TParam>
    {
        public ParameterMapper()
        {
            settersByKey = new Dictionary<string, Action<TParam, double>>();
            settersByIndex = new Dictionary<int, Action<TParam, double>>();
        }

        public int ByKeyCount => settersByKey.Count;
        public int ByIndexCount => settersByIndex.Count;

        private readonly Dictionary<string, Action<TParam, double>> settersByKey;
        private readonly Dictionary<int, Action<TParam, double>> settersByIndex;

        public TParam Target { get; set; }

        private static PropertyInfo GetMappedProperty(Expression<Func<TParam, double>> mapping)
        {
            if (!((mapping.Body as MemberExpression)?.Member is PropertyInfo prop))
                throw new NotSupportedException($"Mapping expression must be simple member access expression lambda");
            return prop;
        }

        public bool HasIndex(int i)
        {
            return settersByIndex.ContainsKey(i);
        }

        public bool HasKey(string key)
        {
            return settersByKey.ContainsKey(key);
        }

        public void Map(Expression<Func<TParam, double>> mapping, string paramKey)
        {
            PropertyInfo prop =  GetMappedProperty(mapping);
            settersByKey.Add(paramKey, (target, value) => prop.SetValue(target, value));
        }
        public void Map(Expression<Func<TParam, double>> mapping, string paramKey, Func<double, double> transform)
        {
            PropertyInfo prop = GetMappedProperty(mapping);
            settersByKey.Add(paramKey, (target, value) => prop.SetValue(target, transform(value)));
        }

        public void Map(Expression<Func<TParam, double>> mapping, int index)
        {
            PropertyInfo prop = GetMappedProperty(mapping);
            settersByIndex.Add(index, (target, value) => prop.SetValue(target, value));
        }

        public void Map(Expression<Func<TParam, double>> mapping, int index, Func<double, double> transform)
        {
            PropertyInfo prop = GetMappedProperty(mapping);
            settersByIndex.Add(index, (target, value) => prop.SetValue(target, transform(value)));
        }

        public void Set(string key, double value)
        {
            if (!settersByKey.TryGetValue(key, out var setter)) throw new ArgumentException($"Parameter '{key}' is not mapped to any property");
            setter(Target, value);
        }

        public void Set(int index, double value)
        {
            if (!settersByIndex.TryGetValue(index, out var setter)) throw new ArgumentException($"Index '{index}' is not mapped to any property");
            setter(Target, value);
        }
    }
}