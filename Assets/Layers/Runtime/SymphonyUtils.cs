using System.Collections;
using UnityEngine;

namespace ABXY.Layers.Runtime
{
    public class SymphonyUtils
    {
        public static IEnumerator WaitForDSPTime(double waitTime, System.Action onReady)
        {
            while (waitTime > AudioSettings.dspTime)
                yield return null;
            onReady?.Invoke();
        }
    }
}
