using UdonSharp;
using UnityEngine;
using Gravinium.Jilwer.Core;
using UnityEngine.PlayerLoop;

namespace Gravinium.Jilwer.Collections
{
    [JilwerType]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ArrayList : UdonSharpBehaviour
    {
        private const int DefaultCapacity = 10;
        
        private int _length;
        private int _capacity;
        private float _resizePercentFactor = 0.5f;
        
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
            UpdateCapacity();

            // Add to the end of the array
            _items[_length] = item;
            _length++;
        }

        public Error Insert(object item, int index)
        {
            if (index < 0 || index >= _length)
            {
                return Error.IndexOutOfBounds;
            }
            
            UpdateCapacity();
            
            ShiftAllRightStartingAt(index);
            _items[index] = item;

            return Error.None;
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

        public object[] Array()
        {
            return _items;
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

        private void UpdateCapacity()
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
                    _capacity += (int)(_capacity * _resizePercentFactor); // Increase by 50%
                }

                Expand();
            }
        }

        private void ShiftAllRightStartingAt(int index)
        {
            for (int i = _length - 1; i >= index; i--)
            {
                _items[i + 1] = _items[i];
            }
        }
    }
}