using UnityEngine;

namespace NCore
{
    public class WaitForUpdate:CustomYieldInstruction
    {
        public override bool keepWaiting => false;
    }
}