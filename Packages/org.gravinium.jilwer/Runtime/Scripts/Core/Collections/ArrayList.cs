using System;
using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;

namespace Gravinium.Jilwer.Core.Collections
{
    [JilwerType]
    public class ArrayList : UdonSharpBehaviour
    {
        private const int DefaultCapacity = 10;
        
        private int _length;
        private int _capacity;
        private Type _type;
        
        private object[] _items;
        
        /* Public */
        public static ArrayList New(JilwerRuntime runtime, int size = DefaultCapacity)
        {
            // Use Jilwer TypeRegistry to create the component
            var type = TypeRegistry.Create(runtime, nameof(ArrayList)).GetComponent<ArrayList>();

            type._length = 0;
            type._capacity = size;
            type._type = null;
            type._items = new object[size];

            return type;
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

        public Error Add(object item)
        {
            if (_type == null)
            {
                _type = item.GetType();
            }
            else if (_type != item.GetType())
            {
                return Error.InvalidType;
            }
            
            // If the length is equal or greater than the current capacity, we must resize to fit another item
            if (_length >= _capacity)
            {
                // Maybe have a system for setting the "resize factor" (new += old * factor)
                //int newSize = _items.Length > 0 ? _items.Length * 2 : DefaultCapacity;
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
            return Error.None;
        }
        
        public Error TryGet(int index, out object item)
        {
            if (index < 0 || index >= _length)
            {
                item = null;
                return Error.IndexOutOfBounds;
            }

            item = _items[index];
            return Error.None;
        }

        public object Remove(int index)
        {
            if (index < 0 || index >= _length)
            {
                return null;
            }

            for (int i = index; i < _length - 1; i++)
            {
                _items[i] = _items[i + 1];
            }

            var removedObj = _items[_length - 1];
            _items[_length - 1] = null;
            _length--;
            return removedObj;
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