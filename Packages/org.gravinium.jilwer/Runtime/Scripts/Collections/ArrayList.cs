using UdonSharp;
using Gravinium.Jilwer.Core;

namespace Gravinium.Jilwer.Collections
{
    /// <summary>
    /// A resizable array that can grow automatically as new items are added.
    /// </summary>
    /// <seealso href="http://docs.gravinium.org/projects/jilwer/api/collections/arraylist"/>
    [JilwerType]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ArrayList : UdonSharpBehaviour
    {
        private const int DefaultCapacity = 10;
        
        private int _length = 0;
        private int _capacity;
        private float _resizeIncreasePercent = 0.5f;
        
        private object[] _items;
        
        /* Constructors */
        /// <summary>
        /// Creates a new ArrayList.
        /// </summary>
        /// <param name="size">Initial size of the array.</param>
        /// <seealso href="http://docs.gravinium.org/projects/jilwer/api/collections/arraylist#constructor"/>
        public ArrayList(int size = DefaultCapacity)
        {
            _capacity = size;
            _items = new object[size];
        }
        
        /* Public */
        /// <summary>
        /// Sets the percentage of size to increase when the array expands.
        /// </summary>
        /// <param name="factor">The percentage increase on resize.</param>
        /// <seealso href="http://docs.gravinium.org/projects/jilwer/api/collections/arraylist#setresizeincreasepercent"/>
        public void SetResizeIncreasePercent(float factor)
        {
            if (factor <= 0) factor = 0.01f;
            
            _resizeIncreasePercent = factor;
        }

        /// <summary>
        /// Returns the current length of the array (the number of elements present).
        /// </summary>
        /// <returns>The length of the list.</returns>
        /// <seealso href="http://docs.gravinium.org/projects/jilwer/api/collections/arraylist#length"/>
        public int Length() => _length;

        /// <summary>
        /// Returns the total capacity of the list before it must expand.
        /// </summary>
        /// <returns>The total capacity.</returns>
        /// <seealso href="http://docs.gravinium.org/projects/jilwer/api/collections/arraylist#capacity"/>
        public int Capacity() => _capacity;

        /// <summary>
        /// Returns true or false whether the array is empty or not (if there are elements present).
        /// </summary>
        /// <returns>Boolean of empty state.</returns>
        /// <seealso href="http://docs.gravinium.org/projects/jilwer/api/collections/arraylist#isempty"/>
        public bool IsEmpty() => _length == 0;

        /// <summary>
        /// Adds more capacity to the ArrayList ensuring there is enough space to avoid the ArrayList expanding
        /// multiple times.
        /// </summary>
        /// <param name="capacity">Extra capacity to add.</param>
        /// <seealso href="http://docs.gravinium.org/projects/jilwer/api/collections/arraylist#ensurecapacity"/>
        public void EnsureCapacity(int capacity)
        {
            _capacity += capacity;
            Expand();
        }

        /// <summary>
        /// Puts the item passed in at the end of the ArrayList. If the capacity is full, it will be expanded.
        /// </summary>
        /// <param name="item">Object being added.</param>
        /// <seealso href="http://docs.gravinium.org/projects/jilwer/api/collections/arraylist#add-last"/>
        public void Add(object item)
        {
            UpdateCapacity();

            // Add to the end of the array
            _items[_length] = item;
            _length++;
        }

        /// <summary>
        /// Puts the item passed in at the specified index of the ArrayList. If the capacity is full, it will be expanded.
        /// </summary>
        /// <param name="index">Index to add item.</param>
        /// <param name="item">Object to add.</param>
        /// <returns>None or IndexOutOfBounds</returns>
        /// <seealso href="http://docs.gravinium.org/projects/jilwer/api/collections/arraylist#add-at"/>
        public Error Add(int index, object item)
        {
            if (index < 0 || index >= _length)
            {
                return Error.IndexOutOfBounds;
            }
            
            UpdateCapacity();
            ShiftAllRightStartingAt(index);
            _items[index] = item;
            _length++;

            return Error.None;
        }

        /// <summary>
        /// Replaces the object at the index with the item passed in.
        /// </summary>
        /// <param name="index">Index to replace.</param>
        /// <param name="item">Object being added.</param>
        /// <returns>None or IndexOutOfBounds</returns>
        /// <seealso href="http://docs.gravinium.org/projects/jilwer/api/collections/arraylist#set"/>
        public Error Set(int index, object item)
        {
            if (index < 0 || index >= _length) return Error.IndexOutOfBounds;

            _items[index] = item;
            
            return Error.None;
        }
        
        /// <summary>
        /// Returns the item at the provided index. Get can fail, in which case the item returned will always be null.
        /// </summary>
        /// <param name="index">Index to retrieve.</param>
        /// <param name="item">Returned object.</param>
        /// <returns>Error and item</returns>
        /// <seealso href="http://docs.gravinium.org/projects/jilwer/api/collections/arraylist#get"/>
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
        
        /// <summary>
        /// Returns true or false if the item is found in the array.
        /// </summary>
        /// <param name="item">Object to match.</param>
        /// <returns>True if found, false if not.</returns>
        /// <seealso href="http://docs.gravinium.org/projects/jilwer/api/collections/arraylist#contains"/>
        public bool Contains(object item)
        {
            for (int i = 0; i < _length; i++)
            {
                if (_items[i] == item) return true;
            }

            return false;
        }

        /// <summary>
        /// Recreates the array to have a capacity equal to the size.
        /// </summary>
        /// <seealso href="http://org.gravinium.org/projects/jilwer/api/collections/arraylist#trimtosize"/>
        public void TrimToSize()
        {
            object[] arr = new object[_length];

            for (int i = 0; i < _length; i++)
            {
                arr[i] = _items[i];
            }

            _items = arr;
            _capacity = _length;
        }

        /// <summary>
        /// Removes the item at the provided index, shifting all items to its right by one position left. This
        /// operation can fail.
        /// </summary>
        /// <param name="index">Index of the item to remove.</param>
        /// <returns>None or IndexOutOfBounds</returns>
        /// <seealso href="http://docs.gravinium.org/projects/jilwer/api/collections/arraylist#remove"/>
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

        /// <summary>
        /// Resets the array to the default size, clearing all items.
        /// </summary>
        /// <seealso href="http://docs.gravinium.org/projects/jilwer/api/collections/arraylist#clear"/>
        public void Clear()
        {
            _items = new object[DefaultCapacity];
            _capacity = DefaultCapacity;
            _length = 0;
        }
        
        /// <summary>
        /// Returns the ArrayList as an array.
        /// </summary>
        /// <returns>Object array.</returns>
        /// <seealso href="http://docs.gravinium.org/projects/jilwer/api/collections/arraylist#toarray"/>
        public object[] ToArray()
        {
            object[] arr = new object[_length];

            for (int i = 0; i < _length; i++)
            {
                arr[i] = _items[i];
            }
            
            return arr;
        }

        /// <summary>
        /// Destroys the underlying GameObject and the ArrayList. Used to clean up the scene after use of the array.
        /// </summary>
        /// <seealso href="http://docs.gravinium.org/projects/jilwer/api/collections/arraylist#destroy"/>
        public void Destroy() => Destroy(gameObject);
        
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
                    _capacity += (int)(_capacity * _resizeIncreasePercent); // Increase by 50%
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