using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CostCenter {
    public class CCFirebase : MonoBehaviour
    {
        public static CCFirebase instance;

        public static bool IsInitialized {
            private set;
            get;
        }
        private const float RetryInitDelayTime = 60.0f;

        private void Awake() {
            instance = this;
        }

        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine("InitFirebaseApp");
        }

        private IEnumerator InitFirebaseApp() {
            while (!IsInitialized) {
                System.Threading.Tasks.Task<Firebase.DependencyStatus> task = null;
                try {
                    // Debug.Log("CCFirebase: start InitFirebaseApp");
                    task = Firebase.FirebaseApp.CheckAndFixDependenciesAsync();
                } catch(System.Exception e) {
                    Debug.LogError("CCFirebase: InitFirebaseApp failed.");
                    Debug.LogError(e);
                    OnFirebaseInitialized(false);
                }
                if (task != null) {
                    yield return new WaitUntil(() => task.IsCompleted);
                    var dependencyStatus = task.Result;
                    if (dependencyStatus == Firebase.DependencyStatus.Available) {
                        // Debug.Log("CCFirebase: InitFirebaseApp success");
                        OnFirebaseInitialized(true);
                    } else {
                        Debug.LogError(System.String.Format(
                        "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                        // Firebase Unity SDK is not safe to use here.
                        OnFirebaseInitialized(false);
                        yield return new WaitForSeconds(RetryInitDelayTime);
                    }
                } else {
                    Debug.Log("CCFirebase: init failed. Retrying...");
                    yield return new WaitForSeconds(RetryInitDelayTime);
                }
            }
        }

        public void OnFirebaseInitialized(bool success) {
            IsInitialized = success;
        }
    }
}
