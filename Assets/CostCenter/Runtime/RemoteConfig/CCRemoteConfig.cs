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
        [Tooltip("Support for A/B Testing (only on first open)")]
        [SerializeField] private bool _autoReFetchRemoteConfig = false;

        public static Action<bool> OnFetchRemoteConfig;

        void Awake() {
            instance = this;
            ConversionData = CCConversionData.Load();
            #if UNITY_ANDROID
            if (IsOrganicConversionData()) {
                CCInstallReferrer.Fetch((referrer) => {
                    if (!string.IsNullOrEmpty(referrer) && IsOrganicConversionData()) {
                        ApplyInstallReferrerToConversionData(referrer);
                    }
                });
            }
            #endif
        }

        private static bool IsOrganicConversionData() {
            if (ConversionData == null || ConversionData.Count == 0) return true;
            if (!ConversionData.TryGetValue("media_source", out var ms)) return true;
            string mediaSource = ms?.ToString();
            return string.IsNullOrEmpty(mediaSource) || mediaSource.Equals("organic", StringComparison.OrdinalIgnoreCase);
        }

        private static void ApplyInstallReferrerToConversionData(string referrer) {
            if (string.IsNullOrEmpty(referrer)) return;
            ConversionData ??= new Dictionary<string, object>();
            
            // Parse query string
            var parsed = referrer
                .Split('&')
                .Select(p => p.Split(new char[]{'='}, 2))
                .Where(p => p.Length == 2)
                .ToDictionary(p => Uri.UnescapeDataString(p[0]), 
                            p => Uri.UnescapeDataString(p[1]));

            // Map utm params → conversion data fields
            if (parsed.TryGetValue("utm_campaign", out var campaign))
                ConversionData["campaign"] = campaign;

            if (parsed.TryGetValue("utm_id", out var campaignId))
                ConversionData["campaign_id"] = campaignId;

            if (parsed.TryGetValue("utm_source", out var source))
                ConversionData["media_source"] = source;

            if (parsed.TryGetValue("utm_content", out var content))
                ConversionData["adgroup_id"] = content;

            Debug.Log($"CCRemoteConfig apply referrer: {referrer}");
            CCConversionData.Save(ConversionData);
        }

        public void ResetDefaultValues() {
            _conversionFields = DEFAULT_CONVERSION_FIELDS;
        }

        private void TrackConversionDataToFirebase()
        {
            // Loop qua các cặp data có trong conversion data để set user property
            foreach (var pair in ConversionData)
            {
                if (_conversionFields.Contains(pair.Key))
                {
                    var value = string.Empty;
                    if (pair.Value != null)
                    {
                        value = pair.Value.ToString();
                    }
                    Firebase.Analytics.FirebaseAnalytics.SetUserProperty(pair.Key, value);
                }
            }

            if (_autoReFetchRemoteConfig && CCConstant.IsFirstOpen)
            {
                FetchRemoteConfig();
            }
        }

        public void OnConversionDataSuccess(Dictionary<string, object> conversionData)
        {
            // Nếu đã nhận conversion data rồi thì không nhận nữa
            if (_isConversionDataGet)
            {
                return;
            }

            _isConversionDataGet = true;
            Debug.Log("CCRemoteConfig onConversionDataSuccess: " + conversionData);

            if (conversionData == null || conversionData.Count < 1)
            {
                return;
            }

            ConversionData = conversionData;

            CCConversionData.Save(ConversionData);

            if (!CCFirebase.IsInitialized)
            {
                CCFirebase.OnFirebaseInitialized += () => {
                    TrackConversionDataToFirebase();
                };
            } else {
                TrackConversionDataToFirebase();
            }
        }

        private IEnumerator IFetchRemoteConfig() 
        {
            yield return new WaitUntil(() => CCFirebase.IsInitialized);
            
            var taskConfig = FirebaseRemoteConfig.DefaultInstance.SetConfigSettingsAsync(new ConfigSettings()
            {
                MinimumFetchIntervalInMilliseconds = 0,
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
            if (ConversionData == null || ConversionData.Count <= 0) {
                return null;
            }

            string stringValue = FirebaseRemoteConfig.DefaultInstance.GetValue($"cc__{key}").StringValue;
            if (string.IsNullOrEmpty(stringValue)) {
                return null;
            }

            try
            {
                CCConversionConfig[] configures = JsonConvert.DeserializeObject<CCConversionConfig[]>(stringValue);
                if (configures == null || configures.Length <= 0)
                {
                    return null;
                }

                foreach (var config in configures)
                {
                    if (config.IsMapWithConversionData(ConversionData))
                    {
                        return config.value;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error parsing conversion config for key '{key}', value: {stringValue}: {ex.Message}");
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
            if (valueByConversion != null)
            {
                if (bool.TryParse(valueByConversion.ToString(), out var result))
                {
                    return result;
                }
            }
            return booleanValue;
        }

        public long GetLongValue(string key) {
            long longValue = FirebaseRemoteConfig.DefaultInstance.GetValue(key).LongValue;
            object valueByConversion = GetDataByConversion(key);
            // Debug.Log($"GetLongValue - Conversion: {valueByConversion} - Value: {longValue}");
            if (valueByConversion != null)
            {
                if (long.TryParse(valueByConversion.ToString(), out var result))
                {
                    return result;
                }
            }
            return longValue;
        }

        public double GetDoubleValue(string key) {
            double doubleValue = FirebaseRemoteConfig.DefaultInstance.GetValue(key).DoubleValue;
            object valueByConversion = GetDataByConversion(key);
            // Debug.Log($"GetDoubleValue - Conversion: {valueByConversion} - Value: {doubleValue}");
            if (valueByConversion != null)
            {
                if (double.TryParse(valueByConversion.ToString(), out var result))
                {
                    return result;
                }
            }
            return doubleValue;
        }
    }
}
