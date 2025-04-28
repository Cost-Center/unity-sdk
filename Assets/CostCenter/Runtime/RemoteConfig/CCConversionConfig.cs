using System;

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
        public object value;

        public bool CompareValue(string key, object value)
        {
            if (key == null || value == null) {
                return false;
            }
            if (key.Equals("campaign")) {
                return string.IsNullOrEmpty(campaign) || campaign.Equals(value);
            } else if (key.Equals("campaign_id")) {
                return string.IsNullOrEmpty(campaign_id) || campaign_id.Equals(value);
            } else if (key.Equals("adset")) {
                return string.IsNullOrEmpty(adset) || adset.Equals(value);
            } else if (key.Equals("adset_id")) {
                return string.IsNullOrEmpty(adset_id) || adset_id.Equals(value);
            } else if (key.Equals("adgroup_id")) {
                return string.IsNullOrEmpty(adgroup_id) || adgroup_id.Equals(value);
            } else if (key.Equals("media_source")) {
                return string.IsNullOrEmpty(media_source) || media_source.Equals(value);
            }
            return false;
        }
    }
}
