﻿using System.Threading.Tasks;

namespace NCore
{
    public static class TaskExtention
    {
        /// <summary>
        /// 捕获Task异常
        /// </summary>
        /// <param name="task"></param>
        public static async void WrapErrors(this Task task)
        {
            await task;
        }
    }
}