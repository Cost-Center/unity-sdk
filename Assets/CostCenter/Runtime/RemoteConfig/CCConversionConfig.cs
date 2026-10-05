using System;
using System.Collections.Generic;
using UnityEngine;

namespace CostCenter.RemoteConfig {
    [Serializable]
    public class CCConversionConfig
    {
        public string campaign;
        public string campaign_id;
        public string adset;
        public string adset_id;
        public string adgroup_id;
        public string media_source;
        public string install_time;
        public string af_siteid;
        public object value;

        /// <summary>
        /// Field mapping for different MMP sources to AppsFlyer format
        /// Key: AppsFlyer field name
        /// Value: Array of possible field names from MMP sources (check in order)
        /// </summary>
        private static readonly Dictionary<string, string[]> MMP_FIELD_MAPPING = new Dictionary<string, string[]>()
        {
            { "media_source", new[] { "media_source", "network" } },           // AppsFlyer, Adjust
            { "campaign", new[] { "campaign" } },               // AppsFlyer, Adjust
            { "campaign_id", new[] { "campaign_id" } },                        // AppsFlyer
            { "adgroup_id", new[] { "adgroup_id", "adgroup" } },               // AppsFlyer, Adjust
            { "adset", new[] { "adset", "creative" } },                        // AppsFlyer, Adjust
            { "adset_id", new[] { "adset_id" } },                              // AppsFlyer
            { "install_time", new[] { "install_time" } },                      // AppsFlyer
            { "af_siteid", new[] { "af_siteid", "trackerToken" } },            // AppsFlyer, Adjust
        };

        /// <summary>
        /// Maps conversion data from any MMP source to AppsFlyer format using field mapping rules
        /// </summary>
        /// <param name="rawConversionData">Raw conversion data from the MMP source</param>
        /// <returns>Dictionary with AppsFlyer-formatted fields</returns>
        public Dictionary<string, object> MappingConversionData(Dictionary<string, object> rawConversionData)
        {
            if (rawConversionData == null || rawConversionData.Count < 1)
            {
                return null;
            }

            var mappedData = new Dictionary<string, object>();

            // Apply mapping rules: for each AppsFlyer field, check possible source field names
            foreach (var mapping in MMP_FIELD_MAPPING)
            {
                string appsflyerField = mapping.Key;
                string[] possibleSourceFields = mapping.Value;

                // Try each possible source field name
                foreach (var sourceField in possibleSourceFields)
                {
                    if (rawConversionData.ContainsKey(sourceField) && rawConversionData[sourceField] != null)
                    {
                        mappedData[appsflyerField] = rawConversionData[sourceField];
                        break; // Use the first match found
                    }
                }
            }

            return mappedData;
        }

        /// <summary>
        /// Compares config value with conversion value as string (conversion value may be non-string, e.g. long)
        /// </summary>
        private static bool IsMatchValue(string configValue, object conversionValue)
        {
            return !string.IsNullOrEmpty(configValue) && configValue == conversionValue?.ToString();
        }

        public bool IsMapWithConversionData(Dictionary<string, object> conversionData)
        {
            // Automatically map conversion data from any MMP format to AppsFlyer format
            var mappedConversionData = MappingConversionData(conversionData);
            
            if (mappedConversionData == null || mappedConversionData.Count < 1)
            {
                return false;
            }

            // Match by campaign OR campaign_id (fields left empty in config are ignored)
            if (!string.IsNullOrEmpty(campaign) || !string.IsNullOrEmpty(campaign_id))
            {
                if (
                    !IsMatchValue(campaign, mappedConversionData.GetValueOrDefault("campaign", null))
                    && !IsMatchValue(campaign_id, mappedConversionData.GetValueOrDefault("campaign_id", null))
                )
                {
                    return false;
                }
            }

            // Match by adset OR adset_id (fields left empty in config are ignored)
            if (!string.IsNullOrEmpty(adset) || !string.IsNullOrEmpty(adset_id))
            {
                if (
                    !IsMatchValue(adset, mappedConversionData.GetValueOrDefault("adset", null))
                    && !IsMatchValue(adset_id, mappedConversionData.GetValueOrDefault("adset_id", null))
                )
                {
                    return false;
                }
            }

            if (!string.IsNullOrEmpty(adgroup_id))
            {
                if (!IsMatchValue(adgroup_id, mappedConversionData.GetValueOrDefault("adgroup_id", null)))
                {
                    return false;
                }
            }

            if (!string.IsNullOrEmpty(media_source))
            {
                if (!IsMatchValue(media_source, mappedConversionData.GetValueOrDefault("media_source", null)))
                {
                    return false;
                }
            }

            if (!string.IsNullOrEmpty(install_time))
            {
                if (!IsMatchValue(install_time, mappedConversionData.GetValueOrDefault("install_time", null)))
                {
                    return false;
                }
            }

            if (!string.IsNullOrEmpty(af_siteid))
            {
                if (!IsMatchValue(af_siteid, mappedConversionData.GetValueOrDefault("af_siteid", null)))
                {
                    return false;
                }
            }

            return (
                !string.IsNullOrEmpty(campaign)
                || !string.IsNullOrEmpty(campaign_id)
                || !string.IsNullOrEmpty(adset)
                || !string.IsNullOrEmpty(adset_id)
                || !string.IsNullOrEmpty(adgroup_id)
                || !string.IsNullOrEmpty(media_source)
                || !string.IsNullOrEmpty(install_time)
                || !string.IsNullOrEmpty(af_siteid)
            );
        }
    }
}
