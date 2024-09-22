using System;
using System.Collections;
using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace SamuelIH.Nwn.Blueprints
{
    [Serializable]
    public class OverridableDictionary<T> : IOverridableConstruct
    {
        public Dictionary<string, T>? Replace { get; set; }
        public Dictionary<string, T>? Remove { get; set; }
        public Dictionary<string, T>? Set { get; set; }
    
        private Dictionary<string, T>? _dictionary;
    
        public void ResolveFromParent(object? parent)
        {
            _dictionary = new Dictionary<string, T>();
        
            if (Replace != null)
            {
                _dictionary = new Dictionary<string, T>(Replace);
            }
        
            if (Replace == null && parent is OverridableDictionary<T> parentDictionary)
            {
                _dictionary = new Dictionary<string, T>(parentDictionary._dictionary!);
            }
        
            if (Remove != null)
            {
                foreach (var key in Remove.Keys)
                {
                    _dictionary.Remove(key);
                }
            }
        
            if (Set != null)
            {
                foreach (var (key, value) in Set)
                {
                    _dictionary[key] = value;
                }
            }
        }

        [YamlIgnore]
        public bool IsResolved => _dictionary != null;

        public IEnumerator<KeyValuePair<string, T>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        [YamlIgnore]
        public int Count => _dictionary.Count;

        public bool ContainsKey(string key)
        {
            return _dictionary.ContainsKey(key);
        }

        public bool TryGetValue(string key, out T value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        public T this[string key] => _dictionary[key];

        [YamlIgnore]
        public IEnumerable<string> Keys => _dictionary.Keys;

        [YamlIgnore]
        public IEnumerable<T> Values => _dictionary.Values;
    }
}