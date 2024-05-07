using UnityEngine;
using Firebase.Extensions;
#if UNITY_IOS && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

namespace CostCenter.Attribution
{
    public class CCAttribution : MonoBehaviour
    {
        public static CCAttribution instance;

        [SerializeField] private bool _autoTracking = true;
        
        void Awake() {
            instance = this;
            if (_autoTracking) {
                CCFirebase.OnFirebaseInitialized += () => {
                    TrackingAttribution(null);
                };
            }
        }

        public void TrackingAttribution(string firebaseAppInstanceId) {
            if (CCTracking.IsFirstOpen) {
                StartCoroutine(CCTracking.AppOpen(
                    firebaseAppInstanceId: firebaseAppInstanceId
                ));
                CCTracking.IsFirstOpen = false;
            }
            if (!CCTracking.IsTrackedATT) {
                StartCoroutine(CCTracking.TrackATT(
                    firebaseAppInstanceId: firebaseAppInstanceId,
                    delayTime: 2.0f
                ));
            }
        }

        #if UNITY_IOS && !UNITY_EDITOR
        [DllImport ("__Internal")]
	    private static extern void _CCRequestTrackingAuthorization();
        #endif
        public static void RequestAppTrackingTransparency() {
            #if UNITY_IOS && !UNITY_EDITOR
                _CCRequestTrackingAuthorization();
            #endif
        }
    }
}
