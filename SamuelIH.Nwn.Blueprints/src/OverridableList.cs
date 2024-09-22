using System;
using System.Collections;
using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace SamuelIH.Nwn.Blueprints
{
    [Serializable]
    public class OverridableList<T> : IOverridableConstruct
    {
        public List<T>? Replace { get; set; }
        public List<T>? Remove { get; set; }
        public List<T>? Add { get; set; }
    
        private List<T>? _list;
    
        public void ResolveFromParent(object? parent)
        {
            _list = new List<T>();
        
            if (Replace != null)
            {
                _list.AddRange(Replace);
            }
        
            if (Replace == null && parent is OverridableList<T> parentList)
            {
                _list.AddRange(parentList._list!);
            }
        
            if (Remove != null)
            {
                _list.RemoveAll(Remove.Contains);
            }
        
            if (Add != null)
            {
                _list.AddRange(Add);
            }
        }

        [YamlIgnore]
        public bool IsResolved => _list != null;

        public IEnumerator<T> GetEnumerator()
        {
            return _list!.GetEnumerator();
        }

        // IEnumerator IEnumerable.GetEnumerator()
        // {
        //     return ((IEnumerable)_list).GetEnumerator();
        // }

        [YamlIgnore]
        public int Count => _list!.Count;
    }
}