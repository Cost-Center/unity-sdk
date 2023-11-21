using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace CostCenter.Attribution
{
    public class CCAttribution : MonoBehaviour
    {
        void Start()
        {
            if (CCTracking.IsFirstOpen) {
                StartCoroutine(CCTracking.AppOpen());
                CCTracking.IsFirstOpen = false;
            }
        }

    }
}
