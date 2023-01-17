using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NCore
{
    /// <summary>
    /// 下载的原子
    /// </summary>
    public class BundleUnit
    {
        public string Name;
        public string Hash;
        public int Length;
        public BundleState State;

        public bool NeedDownload
        {
            get
            {
                return State == BundleState.Add || State == BundleState.Modify;
            }
        }

        public BundleUnit(string name, string hash, int length)
        {
            Name = name;
            Hash = hash;
            Length = length > 0 ? length : 1024 * 100;
            State = BundleState.None;
        }
    }

    /// <summary>
    /// bundle 状态
    /// </summary>
    public enum BundleState
    {
        None,
        Add,        // 增
        Remove,     // 删
        Modify,     // 改
        NoChange,   // 无变化
    }
}

