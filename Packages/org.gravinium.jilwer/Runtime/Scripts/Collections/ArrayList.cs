using UdonSharp;
using UnityEngine;
using Gravinium.Jilwer.Core;

namespace Gravinium.Jilwer.Collections
{
    [JilwerType]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ArrayList : UdonSharpBehaviour
    {
        private const int DefaultCapacity = 10;
        
        private int _length;
        private int _capacity;
        
        private object[] _items;
        
        /* Public */
        public static Error New(JilwerRuntime runtime, out ArrayList value, int size = DefaultCapacity)
        {
            var err = TypeRegistry.Create(runtime, nameof(ArrayList), out GameObject obj);
            if (err != Error.None) {
                value = null;
                return err;
            };
            
            var type = obj.GetComponent<ArrayList>();

            type._length = 0;
            type._capacity = size;
            type._items = new object[size];

            value = type;
            return Error.None;
        }

        public int Length()
        {
            return _length;
        }

        public int Capacity()
        {
            return _capacity;
        }

        public void EnsureCapacity(int capacity)
        {
            _capacity += capacity;
            Expand();
        }

        public void Add(object item)
        {
            // If the length is equal or greater than the current capacity, we must resize to fit another item
            if (_length >= _capacity)
            {
                // Maybe have a system for setting the "resize factor" (new += old * factor)
                if (_capacity < 2)
                {
                    _capacity = 2;
                }
                else
                {
                    _capacity += _capacity / 2; // Increase by 50%
                }

                Expand();
            }

            // Add to the end of the array
            _items[_length] = item;
            _length++;
        }
        
        public Error Get(int index, out object item)
        {
            if (index < 0 || index >= _length)
            {
                item = null;
                return Error.IndexOutOfBounds;
            }

            item = _items[index];
            return Error.None;
        }

        public Error Remove(int index)
        {
            if (index < 0 || index >= _length)
            {
                return Error.IndexOutOfBounds;
            }

            for (int i = index; i < _length - 1; i++)
            {
                _items[i] = _items[i + 1];
            }

            _items[_length - 1] = null;
            _length--;
            return Error.None;
        }
        
        /* Private */
        private void Expand()
        {
            if (_items.Length >= _capacity) { return; }
            
            object[] newItems = new object[_capacity];

            for (int i = 0; i < _length; i++)
            {
                newItems[i] = _items[i];
            }
            _items = newItems;
        }
    }
}