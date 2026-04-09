using UdonSharp;
using UnityEngine;

namespace Org.Gravinium.Jilwer.Runtime.Core.Collections
{
    public class ObjectArrayList : UdonSharpBehaviour
    {
        private int _count;
        private object[] _items;

        public static ObjectArrayList New(int size = 4)
        {
            return TypeRegistry.Create(nameof(ObjectArrayList)).GetComponent<ObjectArrayList>();
        }

        public int Length()
        {
            return _count;
        }

        public void Add(object item)
        {
            if (_count >= _items.Length)
            {
                object[] newItems = new object[_count * 2];
                for (int i = 0; i < _count; i++)
                {
                    newItems[i] = _items[i];
                }
                _items = newItems;
            }

            _items[_count] = item;
            _count++;
        }

        public object Get(int index)
        {
            if (_count <= 0) return null;
            if (index >= _count)
            {
                Debug.LogError($"[Jilwer] Index Out Of Bounds! Index: {index} > Count: {_count}");
            }

            return _items[index];
        }

        public void Remove(int index)
        {
            if (_count <= 0) return;
            if (index >= _count)
            {
                Debug.LogError($"[Jilwer] Index Out Of Bounds! Index: {index} > Count: {_count}");
            }

            for (int i = index; i < _count - 1; i++)
            {
                _items[i] = _items[i + 1];
            }

            _items[_count - 1] = null;
            _count--;
        }
    }
}