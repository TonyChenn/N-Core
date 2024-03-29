using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;

/// <summary>
/// This improved version of the System.Collections.Generic.List that doesn't release the buffer on Clear(),
/// resulting in better performance and less garbage collection.
/// PRO: BetterList performs faster than List when you Add and Remove items (although slower if you remove from the beginning).
/// CON: BetterList performs worse when sorting the list. If your operations involve sorting, use the standard List instead.
/// </summary>
namespace NCore
{
    public class BetterList<T>
    {
        public T[] buffer;

        public int size = 0;

        /// <summary>
        /// For 'foreach' functionality.
        /// </summary>

        [DebuggerHidden]
        [DebuggerStepThrough]
        public IEnumerator<T> GetEnumerator()
        {
            if (buffer != null)
            {
                for (int i = 0; i < size; ++i)
                {
                    yield return buffer[i];
                }
            }
        }


        [DebuggerHidden]
        public T this[int i]
        {
            get { return buffer[i]; }
            set { buffer[i] = value; }
        }

        public int Count
        {
            get { return buffer.Length; }
        }

        /// <summary>
        /// Helper function that expands the size of the array, maintaining the content.
        /// </summary>

        void AllocateMore()
        {
            T[] newList = (buffer != null) ? new T[Mathf.Max(buffer.Length << 1, 32)] : new T[32];
            if (buffer != null && size > 0) buffer.CopyTo(newList, 0);
            buffer = newList;
        }

        /// <summary>
        /// Trim the unnecessary memory, resizing the buffer to be of 'Length' size.
        /// Call this function only if you are sure that the buffer won't need to resize anytime soon.
        /// </summary>

        void Trim()
        {
            if (size > 0)
            {
                if (size < buffer.Length)
                {
                    T[] newList = new T[size];
                    for (int i = 0; i < size; ++i) newList[i] = buffer[i];
                    buffer = newList;
                }
            }
            else buffer = null;
        }

        /// <summary>
        /// Clear the array by resetting its size to zero. Note that the memory is not actually released.
        /// </summary>

        public void Clear() { size = 0; }

        /// <summary>
        /// Clear the array and release the used memory.
        /// </summary>

        public void Release() { size = 0; buffer = null; }

        /// <summary>
        /// Add the specified item to the end of the list.
        /// </summary>

        public void Add(T item)
        {
            if (buffer == null || size == buffer.Length) AllocateMore();
            buffer[size++] = item;
        }

        public void AddRange(BetterList<T> temp)
        {
            if (size + temp.size > buffer.Length)
            {
                System.Array.Resize(ref buffer, size + temp.size);
            }
            System.Array.Copy(temp.buffer, 0, buffer, size, temp.size);
            size += temp.size;
        }

        /// <summary>
        /// Insert an item at the specified index, pushing the entries back.
        /// </summary>

        public void Insert(int index, T item)
        {
            if (buffer == null || size == buffer.Length) AllocateMore();

            if (index > -1 && index < size)
            {
                for (int i = size; i > index; --i) buffer[i] = buffer[i - 1];
                buffer[index] = item;
                ++size;
            }
            else Add(item);
        }

        /// <summary>
        /// Returns 'true' if the specified item is within the list.
        /// </summary>

        public bool Contains(T item)
        {
            if (buffer == null) return false;
            for (int i = 0; i < size; ++i) if (buffer[i].Equals(item)) return true;
            return false;
        }

        /// <summary>
        /// Return the index of the specified item.
        /// </summary>

        public int IndexOf(T item)
        {
            if (buffer == null) return -1;
            for (int i = 0; i < size; ++i) if (buffer[i].Equals(item)) return i;
            return -1;
        }

        /// <summary>
        /// Remove the specified item from the list. Note that RemoveAt() is faster and is advisable if you already know the index.
        /// </summary>

        public bool Remove(T item)
        {
            if (buffer != null)
            {
                EqualityComparer<T> comp = EqualityComparer<T>.Default;

                for (int i = 0; i < size; ++i)
                {
                    if (comp.Equals(buffer[i], item))
                    {
                        --size;
                        buffer[i] = default(T);
                        for (int b = i; b < size; ++b) buffer[b] = buffer[b + 1];
                        buffer[size] = default(T);
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Remove an item at the specified index.
        /// </summary>

        public void RemoveAt(int index)
        {
            if (buffer != null && index > -1 && index < size)
            {
                --size;
                buffer[index] = default(T);
                for (int b = index; b < size; ++b) buffer[b] = buffer[b + 1];
                buffer[size] = default(T);
            }
        }

        /// <summary>
        /// Remove an item from the end.
        /// </summary>

        public T Pop()
        {
            if (buffer != null && size != 0)
            {
                T val = buffer[--size];
                buffer[size] = default(T);
                return val;
            }
            return default(T);
        }

        /// <summary>
        /// Mimic List's ToArray() functionality, except that in this case the list is resized to match the current size.
        /// </summary>

        public T[] ToArray() { Trim(); return buffer; }

        /// <summary>
        /// List.Sort equivalent. Manual sorting causes no GC allocations.
        /// </summary>

        [DebuggerHidden]
        [DebuggerStepThrough]
        public void Sort(CompareFunc comparer)
        {
            int start = 0;
            int max = size - 1;
            bool changed = true;

            while (changed)
            {
                changed = false;

                for (int i = start; i < max; ++i)
                {
                    // Compare the two values
                    if (comparer(buffer[i], buffer[i + 1]) > 0)
                    {
                        // Swap the values
                        T temp = buffer[i];
                        buffer[i] = buffer[i + 1];
                        buffer[i + 1] = temp;
                        changed = true;
                    }
                    else if (!changed)
                    {
                        // Nothing has changed -- we can start here next time
                        start = (i == 0) ? 0 : i - 1;
                    }
                }
            }
        }

        /// <summary>
        /// Comparison function should return -1 if left is less than right, 1 if left is greater than right, and 0 if they match.
        /// </summary>

        public delegate int CompareFunc(T left, T right);

        public delegate bool Predicate<in U>(U obj);

        /// <summary>
        /// 查找第一个
        /// </summary>
        public T FindFirst(Predicate<T> match)
        {
            if (match == null) return default(T);
            int index = FindFirstIndex(match);
            return index == -1 ? default(T) : buffer[index];
        }
        /// <summary>
        /// 查找第一个索引
        /// </summary>
        public int FindFirstIndex(Predicate<T> match)
        {
            if (match == null) return -1;
            for (int i = 0, iMax = buffer.Length; i < iMax; i++)
            {
                if (match(buffer[i]))
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// 查找最后一个
        /// </summary>
        public T FindLast(Predicate<T> match)
        {
            if (match == null) return default(T);
            int index = FindLastIndex(match);
            return index == -1 ? default(T) : buffer[index];
        }
        /// <summary>
        /// 查找最后一个索引
        /// </summary>
        public int FindLastIndex(Predicate<T> match)
        {
            if (match == null) return -1;
            for (int iMax = buffer.Length, i = iMax; i >= 0; i--)
            {
                if (match(buffer[i]))
                    return i;
            }
            return -1;
        }
    }
}


