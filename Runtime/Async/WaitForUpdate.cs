using UnityEngine;

namespace NextFramework.Core
{
    public class WaitForUpdate:CustomYieldInstruction
    {
        public override bool keepWaiting => false;
    }
}