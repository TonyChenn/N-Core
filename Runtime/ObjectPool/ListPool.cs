using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NCore
{
    public class ListPool<T> where T :new()
    {
        private static readonly DefaultObjectPool<List<T>> _pool = new DefaultObjectPool<List<T>>((item) => item.Clear());

        public static List<T> Alloc()
        {
            return _pool.Alloc();
        }

        public static void Recycle(List<T> item)
        {
            _pool.Recycle(item);
        }
    }

    public static class ListPoolExtention
    {
        public static void Recycle<T>(this List<T> list) where T : new()
        {
            ListPool<T>.Recycle(list);
        }
    }
}

