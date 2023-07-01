using System;
using System.Collections.Generic;
using System.Text;

namespace NCore
{
    public class StringBuilderPool
    {
        private static int MaxCount = 31;
        static Stack<StringBuilder> dataStack = new Stack<StringBuilder>(16);

        public static StringBuilder Alloc()
        {
            return dataStack.Count > 0 ? dataStack.Pop() : new StringBuilder();
        }

        public static void Recycle(StringBuilder builder)
        {
            builder.Remove(0, builder.Length);
            if (dataStack.Count <= MaxCount)
                dataStack.Push(builder);
            else
                builder = null;
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
