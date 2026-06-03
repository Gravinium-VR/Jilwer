using UdonSharp;
using UnityEngine;

namespace Gravinium.Jilwer.Core.Collections
{
    [JilwerType]
    public class ObjectArrayList : UdonSharpBehaviour
    {
        private int _count;
        private object[] _items;

        public static ObjectArrayList New(JilwerRuntime runtime, int size = 4)
        {
            var type = TypeRegistry.Create(runtime, nameof(ObjectArrayList)).GetComponent<ObjectArrayList>();

            type._count = 0;
            type._items = new object[size];

            return type;
        }

        public int Length()
        {
            return _count;
        }

        public void Add(object item)
        {
            if (_count >= _items.Length)
            {
                int newSize = _items.Length > 0 ? _items.Length * 2 : 4;
                object[] newItems = new object[newSize];

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