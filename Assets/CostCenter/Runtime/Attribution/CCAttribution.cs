using UnityEngine;
using Firebase.Extensions;

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
        }
    }
}
