using System;
using System.Collections.Generic;
using System.Text;

namespace NextFramework.Core
{
    public class StringBuilderPool
    {
        private static int MaxCount = 20;
        static Stack<StringBuilder> dataStack = new Stack<StringBuilder>();

        public static void SetMaxCount()
        {
            
        }
        public static StringBuilder Alloc()
        {
            return dataStack.Count > 0 ? dataStack.Pop() : new StringBuilder();
        }

        public static bool Recycle(StringBuilder builder)
        {
            builder.Remove(0, builder.Length);
            if (dataStack.Count <= MaxCount)
                dataStack.Push(builder);
            else
                builder = null;
            return true;
        }
    }

    /// <summary>
    /// StringBuilder 静态拓展
    /// </summary>
    public static class StringBuilderExtention
    {
        public static void Recycle(this StringBuilder builder)
        {
            StringBuilderPool.Recycle(builder);
        }
    }
}
