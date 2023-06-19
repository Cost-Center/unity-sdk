using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace CostCenter.Attribution {
    public class CCTracking
    {
        private const int MAXIMUM_RETRY = 5;
        internal static bool IsFirstOpen {
            get {
                if (PlayerPrefs.HasKey(CCConstant.FIRST_OPEN_KEY)) {
                    int isFirst = PlayerPrefs.GetInt(CCConstant.FIRST_OPEN_KEY);
                    return isFirst <= 0 && isFirst > -MAXIMUM_RETRY;
                }
                return true;
            }
            set {
                if (!value) {
                    PlayerPrefs.SetInt(CCConstant.FIRST_OPEN_KEY, 1);
                    return;
                }
                if (!PlayerPrefs.HasKey(CCConstant.FIRST_OPEN_KEY)) {
                    PlayerPrefs.SetInt(CCConstant.FIRST_OPEN_KEY, 0);
                    return;
                }
                int lastFirst = PlayerPrefs.GetInt(CCConstant.FIRST_OPEN_KEY);
                PlayerPrefs.SetInt(CCConstant.FIRST_OPEN_KEY, lastFirst - 1);
            }
        }

        internal static IEnumerator CallAppOpen()
        {
            string bundleId = Application.identifier;
            string platform = Application.platform == RuntimePlatform.Android ? "android" : "ios";

            System.Threading.Tasks.Task<string> task = Firebase.Analytics.FirebaseAnalytics.GetAnalyticsInstanceIdAsync();
            yield return new WaitUntil(() => task.IsCompleted);

            string fbAppInstanceId = task.Result;

            Debug.Log($"{bundleId} - {platform}: {fbAppInstanceId}");
            UnityWebRequest www = UnityWebRequest.Get($"https://attribution.costcenter.net/appopen?bundle_id={bundleId}&platform={platform}&firebase_app_instance_id={fbAppInstanceId}");
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError) {
                Debug.Log(www.error);
            } else {
                Debug.Log("CCAttribution CallAppOpen: success");
            }
        }
    }
}
