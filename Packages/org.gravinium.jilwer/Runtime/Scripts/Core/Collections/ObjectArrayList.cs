using UdonSharp;
using UnityEngine;

namespace Org.Gravinium.Jilwer.Runtime.Core.Collections
{
    public class ObjectArrayList : ICollection
    {
        private int count;
        private object[] items;

        public ObjectArrayList(int size = 4)
        {
            count = 0;
            items = new object[size];
        }

        public int Length()
        {
            return count;
        }

        public void Add(object item)
        {
            if (count >= items.Length)
            {
                object[] newItems = new object[count * 2];
                for (int i = 0; i < count; i++)
                {
                    newItems[i] = items[i];
                }
                items = newItems;
            }

            items[count] = item;
            count++;
        }

        public object Get(int index)
        {
            if (count <= 0) return null;
            if (index >= count)
            {
                Debug.LogError($"[Jilwer] Index Out Of Bounds! Index: {index} > Count: {count}");
            }

            return items[index];
        }

        public void Remove(int index)
        {
            if (count <= 0) return;
            if (index >= count)
            {
                Debug.LogError($"[Jilwer] Index Out Of Bounds! Index: {index} > Count: {count}");
            }

            for (int i = index; i < count - 1; i++)
            {
                items[i] = items[i + 1];
            }

            items[count - 1] = null;
            count--;
        }
    }
}