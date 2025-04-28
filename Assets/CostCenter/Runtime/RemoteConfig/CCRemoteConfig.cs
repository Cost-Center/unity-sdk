using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.RemoteConfig;

namespace CostCenter.RemoteConfig {
    public class CCRemoteConfig : MonoBehaviour
    {
        public static CCRemoteConfig instance;

        protected static bool _isConversionDataGet = false;
        public static Dictionary<string, object> ConversionData {
            get;
            protected set;
        }

        [SerializeField] private string[] _conversionFields = new string[] {
            "media_source",
            "install_time",
            "af_siteid",
            "adgroup_id",
            "adset",
            "adset_id",
            "campaign_id",
            "campaign"
        };

        public static Action<bool> OnFetchRemoteConfig;

        void Awake() {
            instance = this;
            ConversionData = new Dictionary<string, object>();
        }

        public void OnConversionDataSuccess(Dictionary<string, object> conversionData)
        {
            // Nếu đã nhận conversion data rồi thì không nhận nữa
            if (_isConversionDataGet) {
                return;
            }

            _isConversionDataGet = true;
            // Debug.Log("onConversionDataSuccess: " + conversionData);

            if (conversionData == null || conversionData.Count < 1) {
                return;
            }

            ConversionData = conversionData;

            // Loop qua các cặp data có trong conversion data để set user property
            foreach (var pair in conversionData)
            {
                switch (pair.Key)
                {
                    // Các key cần set có thể tự custom lại (thêm hoặc bớt)
                    case "media_source":
                    case "install_time":
                    case "af_siteid":
                    case "adgroup_id":
                    case "adset":
                    case "adset_id":
                    case "campaign_id":
                    case "campaign":  // <- Đây là key mình cần và đã set ở trên dashboard Firebase
                        var value = string.Empty;
                        if (pair.Value != null)
                        {
                            value = pair.Value.ToString();
                        }
                        // Debug.Log("conversion data check " + pair.Key + " " + value);
                        // _conversionDataDictionary.TryAdd(pair.Key, value);
                        // Set user property lên firebase
                        Firebase.Analytics.FirebaseAnalytics.SetUserProperty(pair.Key, value);
                        break;
                }
            }

            if (CCConstant.IsFirstOpen) {
                FetchRemoteConfig();
            }
        }

        private IEnumerator IFetchRemoteConfig() 
        {
            // Đợi firebase khởi tạo thành công
            yield return new WaitUntil(() => CCFirebase.IsInitialized);
            // Gọi fetch remote config
            var taskConfig = FirebaseRemoteConfig.DefaultInstance.SetConfigSettingsAsync(new ConfigSettings()
            {
                MinimumFetchInternalInMilliseconds = 0,
                FetchTimeoutInMilliseconds = 3000
            });
            yield return new WaitUntil(() => taskConfig.IsCompleted);
            var taskFetchAndActive = FirebaseRemoteConfig.DefaultInstance.FetchAndActivateAsync();
            yield return new WaitUntil(() => taskFetchAndActive.IsCompleted);
            // if (FirebaseRemoteConfig.DefaultInstance.Info.LastFetchStatus == LastFetchStatus.Success)
            // {
            //     // Fetch thành công
            // }
            // else
            // {
            //     // Fetch thất bại
            // }
            OnFetchRemoteConfig?.Invoke(FirebaseRemoteConfig.DefaultInstance.Info.LastFetchStatus == LastFetchStatus.Success);
        }

        public void FetchRemoteConfig()
        {
            StartCoroutine(IFetchRemoteConfig());
        }

        public object GetDataByConversion(string key) {
            if (
                ConversionData == null
                || ConversionData.Count <= 0
            ) {
                return null;
            }
            string stringValue = FirebaseRemoteConfig.DefaultInstance.GetValue($"cc__{key}").StringValue;
            if (string.IsNullOrEmpty(stringValue)) {
                return null;
            }
            CCConversionConfig[] configures = JsonUtility.FromJson<CCConversionConfig[]>(stringValue);
            if (configures == null || configures.Length <= 0) {
                return null;
            }
            foreach (var config in configures) {
                bool isCompare = true;
                foreach (var field in _conversionFields) {
                    if (
                        ConversionData.ContainsKey(field)
                        && ConversionData[field] != null
                        && config.CompareValue(field, ConversionData[field])
                    ) {
                        continue;
                    }
                    isCompare = false;
                    break;
                }

                if (isCompare) {
                    return config.value;
                }
            }
            return null;
        }

        public string GetStringValue(string key) {
            string stringValue = FirebaseRemoteConfig.DefaultInstance.GetValue(key).StringValue;
            object valueByConversion = GetDataByConversion(key);
            Debug.Log($"GetStringValue - Conversion: {valueByConversion} - Value: {stringValue}");
            return valueByConversion != null ? valueByConversion.ToString() : stringValue;
        }

        public bool GetBooleanValue(string key) {
            bool booleanValue = FirebaseRemoteConfig.DefaultInstance.GetValue(key).BooleanValue;
            object valueByConversion = GetDataByConversion(key);
            Debug.Log($"GetBooleanValue - Conversion: {valueByConversion} - Value: {booleanValue}");
            return valueByConversion != null ? (bool)valueByConversion : booleanValue;
        }

        public long GetLongValue(string key) {
            long longValue = FirebaseRemoteConfig.DefaultInstance.GetValue(key).LongValue;
            object valueByConversion = GetDataByConversion(key);
            Debug.Log($"GetLongValue - Conversion: {valueByConversion} - Value: {longValue}");
            return valueByConversion != null ? (long)valueByConversion : longValue;
        }

        public double GetDoubleValue(string key) {
            double doubleValue = FirebaseRemoteConfig.DefaultInstance.GetValue(key).DoubleValue;
            object valueByConversion = GetDataByConversion(key);
            Debug.Log($"GetDoubleValue - Conversion: {valueByConversion} - Value: {doubleValue}");
            return valueByConversion != null ? (double)valueByConversion : doubleValue;
        }
    }
}
