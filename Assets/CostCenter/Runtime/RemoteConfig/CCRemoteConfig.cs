using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.RemoteConfig;
using Newtonsoft.Json;

namespace CostCenter.RemoteConfig {
    public class CCRemoteConfig : MonoBehaviour
    {
        public static CCRemoteConfig instance;

        protected static bool _isConversionDataGet = false;
        public static Dictionary<string, object> ConversionData {
            get;
            protected set;
        }

        protected static readonly string[] DEFAULT_CONVERSION_FIELDS = new string[] {
            "media_source",
            "install_time",
            "af_siteid",
            "adgroup_id",
            "adset",
            "adset_id",
            "campaign_id",
            "campaign"
        };
        [SerializeField] private string[] _conversionFields = DEFAULT_CONVERSION_FIELDS;

        public static Action<bool> OnFetchRemoteConfig;

        void Awake() {
            instance = this;
            ConversionData = new Dictionary<string, object>();
        }

        public void ResetDefaultValues() {
            _conversionFields = DEFAULT_CONVERSION_FIELDS;
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
                // switch (pair.Key)
                // {
                //     // Các key cần set có thể tự custom lại (thêm hoặc bớt)
                //     case "media_source":
                //     case "install_time":
                //     case "af_siteid":
                //     case "adgroup_id":
                //     case "adset":
                //     case "adset_id":
                //     case "campaign_id":
                //     case "campaign":
                //         var value = string.Empty;
                //         if (pair.Value != null)
                //         {
                //             value = pair.Value.ToString();
                //         }
                //         Firebase.Analytics.FirebaseAnalytics.SetUserProperty(pair.Key, value);
                //         break;
                // }
                if (_conversionFields.Contains(pair.Key)) {
                    var value = string.Empty;
                    if (pair.Value != null)
                    {
                        value = pair.Value.ToString();
                    }
                    Firebase.Analytics.FirebaseAnalytics.SetUserProperty(pair.Key, value);
                }
            }

            if (CCConstant.IsFirstOpen) {
                FetchRemoteConfig();
            }
        }

        private IEnumerator IFetchRemoteConfig() 
        {
            yield return new WaitUntil(() => CCFirebase.IsInitialized);
            
            var taskConfig = FirebaseRemoteConfig.DefaultInstance.SetConfigSettingsAsync(new ConfigSettings()
            {
                MinimumFetchInternalInMilliseconds = 0,
                FetchTimeoutInMilliseconds = 3000
            });
            yield return new WaitUntil(() => taskConfig.IsCompleted);
            var taskFetchAndActive = FirebaseRemoteConfig.DefaultInstance.FetchAndActivateAsync();
            yield return new WaitUntil(() => taskFetchAndActive.IsCompleted);
            
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
            CCConversionConfig[] configures = JsonConvert.DeserializeObject<CCConversionConfig[]>(stringValue);
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
            // Debug.Log($"GetStringValue - Conversion: {valueByConversion} - Value: {stringValue}");
            return valueByConversion != null ? valueByConversion.ToString() : stringValue;
        }

        public bool GetBooleanValue(string key) {
            bool booleanValue = FirebaseRemoteConfig.DefaultInstance.GetValue(key).BooleanValue;
            object valueByConversion = GetDataByConversion(key);
            // Debug.Log($"GetBooleanValue - Conversion: {valueByConversion} - Value: {booleanValue}");
            return valueByConversion != null ? (bool)valueByConversion : booleanValue;
        }

        public long GetLongValue(string key) {
            long longValue = FirebaseRemoteConfig.DefaultInstance.GetValue(key).LongValue;
            object valueByConversion = GetDataByConversion(key);
            // Debug.Log($"GetLongValue - Conversion: {valueByConversion} - Value: {longValue}");
            return valueByConversion != null ? (long)valueByConversion : longValue;
        }

        public double GetDoubleValue(string key) {
            double doubleValue = FirebaseRemoteConfig.DefaultInstance.GetValue(key).DoubleValue;
            object valueByConversion = GetDataByConversion(key);
            // Debug.Log($"GetDoubleValue - Conversion: {valueByConversion} - Value: {doubleValue}");
            return valueByConversion != null ? (double)valueByConversion : doubleValue;
        }
    }
}
