using UnityEngine;

namespace SFramework.Core
{
    public class WaitForUpdate:CustomYieldInstruction
    {
        public override bool keepWaiting => false;
    }
}