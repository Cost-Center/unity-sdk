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
        
        public bool IsMapWithConversionData(Dictionary<string, object> conversionData)
        {
            // Automatically map conversion data from any MMP format to AppsFlyer format
            var mappedConversionData = MappingConversionData(conversionData);
            
            if (mappedConversionData == null || mappedConversionData.Count < 1)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(campaign) || !string.IsNullOrEmpty(campaign_id))
            {
                object conversionCampaign = mappedConversionData.GetValueOrDefault("campaign", null);
                object conversionCampaignId = mappedConversionData.GetValueOrDefault("campaign_id", null);
                if (
                    (
                        (string.IsNullOrEmpty(campaign) && (conversionCampaign == null || string.IsNullOrEmpty(conversionCampaign.ToString())))
                        || (campaign != null && !campaign.Equals(conversionCampaign))
                    )
                    && (
                        (string.IsNullOrEmpty(campaign_id) && (conversionCampaignId == null || string.IsNullOrEmpty(conversionCampaignId.ToString())))
                        || (campaign_id != null && !campaign_id.Equals(conversionCampaignId))
                    )
                )
                {
                    return false;
                }
            }

            if (!string.IsNullOrEmpty(adset) || !string.IsNullOrEmpty(adset_id))
            {
                object conversionAdset = mappedConversionData.GetValueOrDefault("adset", null);
                object conversionAdsetId = mappedConversionData.GetValueOrDefault("adset_id", null);
                if (
                    (
                        (string.IsNullOrEmpty(adset) && (conversionAdset == null || string.IsNullOrEmpty(conversionAdset.ToString())))
                        || (adset != null && !adset.Equals(conversionAdset))
                    )
                    && (
                        (string.IsNullOrEmpty(adset_id) && (conversionAdsetId == null || string.IsNullOrEmpty(conversionAdsetId.ToString())))
                        || (adset_id != null && !adset_id.Equals(conversionAdsetId))
                    )
                )
                {
                    return false;
                }
            }

            if (!string.IsNullOrEmpty(adgroup_id))
            {
                if (!adgroup_id.Equals(mappedConversionData.GetValueOrDefault("adgroup_id", string.Empty)))
                {
                    return false;
                }
            }

            if (!string.IsNullOrEmpty(media_source))
            {
                if (!media_source.Equals(mappedConversionData.GetValueOrDefault("media_source", string.Empty)))
                {
                    return false;
                }
            }

            if (!string.IsNullOrEmpty(install_time))
            {
                if (!install_time.Equals(mappedConversionData.GetValueOrDefault("install_time", string.Empty)))
                {
                    return false;
                }
            }

            if (!string.IsNullOrEmpty(af_siteid))
            {
                if (!af_siteid.Equals(mappedConversionData.GetValueOrDefault("af_siteid", string.Empty)))
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
