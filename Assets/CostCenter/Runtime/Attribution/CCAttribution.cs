using System.Collections;
using UnityEngine;

namespace CostCenter.Attribution
{
    public class CCAttribution : MonoBehaviour
    {
        void Start()
        {
            StartCoroutine(WaitToTracking());
        }

        private IEnumerator WaitToTracking() {
            Debug.Log(CCFirebase.IsInitialized);
            yield return new WaitUntil(() => CCFirebase.IsInitialized);
            if (CCTracking.IsFirstOpen) {
                yield return StartCoroutine(CCTracking.CallAppOpen());

                CCTracking.IsFirstOpen = false;
            }
        }
    }
}
