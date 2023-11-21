using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Ugi.PlayInstallReferrerPlugin;

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

        private static Dictionary<string, object> _installReferrerInfo = null;

        internal static IEnumerator AppOpen()
        {
            yield return new WaitUntil(() => CCFirebase.IsInitialized);
            System.Threading.Tasks.Task<string> task = Firebase.Analytics.FirebaseAnalytics.GetAnalyticsInstanceIdAsync();
            yield return new WaitUntil(() => task.IsCompleted);

            string bundleId = Application.identifier;
            string platform = Application.platform == RuntimePlatform.Android ? "android" : "ios";
            string fbAppInstanceId = task.Result;

            string url = "https://attribution.costcenter.net/appopen?";
            url += $"bundle_id={bundleId}";
            url += $"&platform={platform}";
            url += $"&firebase_app_instance_id={fbAppInstanceId}";
            
            // ANDROID INSTALL REFERRER
            _installReferrerInfo = null;
            #if UNITY_ANDROID && !UNITY_EDITOR
                PlayInstallReferrerAndroid.GetInstallReferrerInfo(InstallReferrerCallback);
                yield return new WaitUntil(() => _installReferrerInfo != null);
            #elif UNITY_EDITOR
                PlayInstallReferrerEditor.GetInstallReferrerInfo(InstallReferrerCallback);
                yield return new WaitUntil(() => _installReferrerInfo != null);
            #endif
            if (_installReferrerInfo != null) {
                foreach (KeyValuePair<string, object> info in _installReferrerInfo) {
                    url += $"&{info.Key}={info.Value.ToString()}";
                }
            }

            UnityWebRequest www = UnityWebRequest.Get(url);
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError) {
                Debug.Log(www.error);
            } else {
                Debug.Log("CCAttribution CallAppOpen: success");
            }
        }

        internal static void InstallReferrerCallback(PlayInstallReferrerDetails installReferrerDetails) {
            Dictionary<string, object> result = new Dictionary<string, object>();
            Debug.Log("Install referrer details received!");

            // check for error
            if (installReferrerDetails.Error != null)
            {
                Debug.LogError("Error occurred!");
                if (installReferrerDetails.Error.Exception != null)
                {
                    Debug.LogError("Exception message: " + installReferrerDetails.Error.Exception.Message);
                }
                Debug.LogError("Response code: " + installReferrerDetails.Error.ResponseCode.ToString());
                _installReferrerInfo = result;
                return;
            }

            // print install referrer details
            if (installReferrerDetails.InstallReferrer != null)
            {
                result["install_referrer"] = installReferrerDetails.InstallReferrer;
                Debug.Log("Install referrer: " + installReferrerDetails.InstallReferrer);
            }
            // if (installReferrerDetails.ReferrerClickTimestampSeconds != null)
            // {
            //     result["click_ts"] = installReferrerDetails.ReferrerClickTimestampSeconds.ToString();
            //     Debug.Log("Referrer click timestamp: " + installReferrerDetails.ReferrerClickTimestampSeconds);
            // }
            // if (installReferrerDetails.InstallBeginTimestampSeconds != null)
            // {
            //     result["install_ts"] = installReferrerDetails.InstallBeginTimestampSeconds.ToString();
            //     Debug.Log("Install begin timestamp: " + installReferrerDetails.InstallBeginTimestampSeconds);
            // }
            if (installReferrerDetails.ReferrerClickTimestampServerSeconds != null)
            {
                result["click_ts"] = installReferrerDetails.ReferrerClickTimestampServerSeconds.ToString();
                Debug.Log("Referrer click server timestamp: " + installReferrerDetails.ReferrerClickTimestampServerSeconds);
            }
            if (installReferrerDetails.InstallBeginTimestampServerSeconds != null)
            {
                result["install_ts"]  = installReferrerDetails.InstallBeginTimestampServerSeconds.ToString();
                Debug.Log("Install begin server timestamp: " + installReferrerDetails.InstallBeginTimestampServerSeconds);
            }
            // if (installReferrerDetails.InstallVersion != null)
            // {
            //     result["install_version"] = installReferrerDetails.InstallVersion;
            //     Debug.Log("Install version: " + installReferrerDetails.InstallVersion);
            // }
            // if (installReferrerDetails.GooglePlayInstant != null)
            // {
            //     txtGooglePlayInstantFromCallback = installReferrerDetails.GooglePlayInstant.ToString();
            //     Debug.Log("Google Play instant: " + installReferrerDetails.GooglePlayInstant);
            // }
            _installReferrerInfo = result;
        }
    }
}
