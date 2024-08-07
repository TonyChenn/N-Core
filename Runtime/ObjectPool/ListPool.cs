using System.Collections.Generic;

namespace NCore
{
    public class ListPool<T> where T :new()
    {
        private static readonly DefaultObjectPool<List<T>> pool = new DefaultObjectPool<List<T>>((item) => item.Clear());

		public static List<T> Alloc() => pool.Alloc();

		public static void Recycle(List<T> item) => pool.Recycle(item);
    }

    public static class ListPoolExtention
    {
        public static void Recycle<T>(this List<T> list) where T : new()
        {
            ListPool<T>.Recycle(list);
        }
    }
}

