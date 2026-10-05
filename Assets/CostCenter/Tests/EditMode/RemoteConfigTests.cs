using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace CostCenter.RemoteConfig.Tests
{
    public class RemoteConfigTests
    {

        [Test]
        public void IsMapWithConversionData_Campaign_Case1()
        {
            var config = new CCConversionConfig
            {
                campaign = "test_campaign",
                campaign_id = "12345",
            };

            var conversionData = new Dictionary<string, object>
            {
                { "campaign", "test_campaign" },
                { "campaign_id", "12345" },
            };

            Assert.IsTrue(config.IsMapWithConversionData(conversionData));
        }

        [Test]
        public void IsMapWithConversionData_Campaign_Case2()
        {
            var config = new CCConversionConfig
            {
                campaign = "test_campaign",
                campaign_id = "12345",
            };

            var conversionData = new Dictionary<string, object>
            {
                { "campaign", "test_campaign_new" },
                { "campaign_id", "12345" },
                { "adset", "test_adset" },
                { "adset_id", null },
                { "adgroup_id", null },
            };

            Assert.IsTrue(config.IsMapWithConversionData(conversionData));
        }

        [Test]
        public void IsMapWithConversionData_Campaign_Case3()
        {
            var config = new CCConversionConfig
            {
                campaign = "test_campaign",
                campaign_id = "12345",
            };

            var conversionData = new Dictionary<string, object>
            {
                { "campaign", "test_campaign" },
                { "campaign_id", "123456" },
                { "adset", "test_adset" },
                { "adset_id", null },
                { "adgroup_id", null },
                { "media_source", "test_media_source" },
                { "install_time", "2023-10-01T12:00:00Z" },
                { "af_siteid", "test_af_siteid" },
            };

            Assert.IsTrue(config.IsMapWithConversionData(conversionData));
        }

        [Test]
        public void IsMapWithConversionData_Campaign_Case4()
        {
            var config = new CCConversionConfig
            {
                campaign = "test_campaign",
                campaign_id = "12345",
            };

            var conversionData = new Dictionary<string, object>
            {
                { "campaign", "test_campaign" },
                { "adset", "test_adset" },
                { "adset_id", null },
                { "adgroup_id", null },
                { "media_source", "test_media_source" },
                { "install_time", "2023-10-01T12:00:00Z" },
                { "af_siteid", "test_af_siteid" },
                { "extra_field", "extra_value" } // Extra field should not affect the result
            };

            Assert.IsTrue(config.IsMapWithConversionData(conversionData));
        }

        [Test]
        public void IsMapWithConversionData_Campaign_Case5()
        {
            var config = new CCConversionConfig
            {
                campaign = "",
            };

            var conversionData = new Dictionary<string, object>
            {
                { "campaign", "test_campaign" },
                { "adset", "test_adset" },
                { "adset_id", null },
                { "adgroup_id", null },
                { "media_source", "test_media_source" },
                { "install_time", "2023-10-01T12:00:00Z" },
                { "af_siteid", "test_af_siteid" },
                { "extra_field", "extra_value" } // Extra field should not affect the result
            };

            Assert.IsFalse(config.IsMapWithConversionData(conversionData));
        }

        [Test]
        public void IsMapWithConversionData_Campaign_Case6()
        {
            var config = new CCConversionConfig
            {
                campaign_id = "",
            };

            var conversionData = new Dictionary<string, object>
            {
                { "campaign", "test_campaign" },
                { "adset", "test_adset" },
                { "adset_id", null },
                { "adgroup_id", null },
                { "media_source", "test_media_source" },
                { "install_time", "2023-10-01T12:00:00Z" },
                { "af_siteid", "test_af_siteid" },
                { "extra_field", "extra_value" } // Extra field should not affect the result
            };

            Assert.IsFalse(config.IsMapWithConversionData(conversionData));
        }

        [Test]
        public void IsMapWithConversionData_Adset_Case1()
        {
            var config = new CCConversionConfig
            {
                adset_id = "",
            };

            var conversionData = new Dictionary<string, object>
            {
                { "campaign", "test_campaign" },
                { "adset", "test_adset" },
                { "adset_id", null },
                { "adgroup_id", null },
                { "media_source", "test_media_source" },
                { "install_time", "2023-10-01T12:00:00Z" },
                { "af_siteid", "test_af_siteid" },
                { "extra_field", "extra_value" } // Extra field should not affect the result
            };

            Assert.IsFalse(config.IsMapWithConversionData(conversionData));
        }

        [Test]
        public void IsMapWithConversionData_Adset_Case2()
        {
            var config = new CCConversionConfig
            {
                adset = "test",
                adset_id = null,
            };

            var conversionData = new Dictionary<string, object>
            {
                { "campaign", "test_campaign" },
                { "adset", "test_adset" },
                { "adset_id", null },
                { "adgroup_id", null },
                { "media_source", "test_media_source" },
                { "install_time", "2023-10-01T12:00:00Z" },
                { "af_siteid", "test_af_siteid" },
                { "extra_field", "extra_value" } // Extra field should not affect the result
            };

            Assert.IsFalse(config.IsMapWithConversionData(conversionData));
        }

        [Test]
        public void IsMapWithConversionData_Adgroup_Case1()
        {
            var config = new CCConversionConfig
            {
                adgroup_id = "test_adgroup",
            };

            var conversionData = new Dictionary<string, object>
            {
                { "campaign", "test_campaign" },
                { "adset", "test_adset" },
                { "adset_id", null },
                { "adgroup_id", null },
                { "media_source", "test_media_source" },
                { "install_time", "2023-10-01T12:00:00Z" },
                { "af_siteid", "test_af_siteid" },
                { "extra_field", "extra_value" } // Extra field should not affect the result
            };

            Assert.IsFalse(config.IsMapWithConversionData(conversionData));
        }

        [Test]
        public void IsMapWithConversionData_Adgroup_Case2()
        {
            var config = new CCConversionConfig
            {
                adgroup_id = "",
            };

            var conversionData = new Dictionary<string, object>
            {
                { "campaign", "test_campaign" },
                { "adset", "test_adset" },
                { "adset_id", null },
                { "adgroup_id", null },
                { "media_source", "test_media_source" },
                { "install_time", "2023-10-01T12:00:00Z" },
                { "af_siteid", "test_af_siteid" },
                { "extra_field", "extra_value" } // Extra field should not affect the result
            };

            Assert.IsFalse(config.IsMapWithConversionData(conversionData));
        }

        [Test]
        public void IsMapWithConversionData_Adgroup_Case3()
        {
            var config = new CCConversionConfig
            {
                adgroup_id = null,
            };

            var conversionData = new Dictionary<string, object>
            {
                { "campaign", "test_campaign" },
                { "adset", "test_adset" },
                { "adset_id", null },
                { "adgroup_id", null },
                { "media_source", "test_media_source" },
                { "install_time", "2023-10-01T12:00:00Z" },
                { "af_siteid", "test_af_siteid" },
                { "extra_field", "extra_value" } // Extra field should not affect the result
            };

            Assert.IsFalse(config.IsMapWithConversionData(conversionData));
        }

        [Test]
        public void IsMapWithConversionData_Adgroup_Case4()
        {
            var config = new CCConversionConfig
            {
                adgroup_id = null,
            };

            var conversionData = new Dictionary<string, object>
            {
                { "campaign", "test_campaign" },
                { "adset", "test_adset" },
                { "adset_id", null },
                { "adgroup_id", "" },
                { "media_source", "test_media_source" },
                { "install_time", "2023-10-01T12:00:00Z" },
                { "af_siteid", "test_af_siteid" },
                { "extra_field", "extra_value" } // Extra field should not affect the result
            };

            Assert.IsFalse(config.IsMapWithConversionData(conversionData));
        }

        [Test]
        public void IsMapWithConversionData_Adgroup_Case5()
        {
            var config = new CCConversionConfig
            {
                adgroup_id = "test_adgroup",
            };

            var conversionData = new Dictionary<string, object>
            {
                { "campaign", "test_campaign" },
                { "adset", "test_adset" },
                { "adset_id", null },
                { "adgroup_id", "test_adgroup_1" },
                { "media_source", "test_media_source" },
                { "install_time", "2023-10-01T12:00:00Z" },
                { "af_siteid", "test_af_siteid" },
                { "extra_field", "extra_value" } // Extra field should not affect the result
            };

            Assert.IsFalse(config.IsMapWithConversionData(conversionData));
        }

        [Test]
        public void IsMapWithConversionData_Adgroup_Case6()
        {
            var config = new CCConversionConfig
            {
                adgroup_id = "test_adgroup",
            };

            var conversionData = new Dictionary<string, object>
            {
                { "campaign", "test_campaign" },
                { "adset", "test_adset" },
                { "adset_id", null },
                { "adgroup_id", "test_adgroup" },
                { "media_source", "test_media_source" },
                { "install_time", "2023-10-01T12:00:00Z" },
                { "af_siteid", "test_af_siteid" },
                { "extra_field", "extra_value" } // Extra field should not affect the result
            };

            Assert.IsTrue(config.IsMapWithConversionData(conversionData));
        }

        [Test]
        public void IsMapWithConversionData_Mix_Case1()
        {
            var config = new CCConversionConfig
            {
                campaign = "test_campaign",
                campaign_id = "12345",
                adgroup_id = "test_adgroup",
            };

            var conversionData = new Dictionary<string, object>
            {
                { "campaign", "test_campaign" },
                { "adset", "test_adset" },
                { "adset_id", null },
                { "adgroup_id", "test_adgroup" },
                { "media_source", "test_media_source" },
                { "install_time", "2023-10-01T12:00:00Z" },
                { "af_siteid", "test_af_siteid" },
                { "extra_field", "extra_value" } // Extra field should not affect the result
            };

            Assert.IsTrue(config.IsMapWithConversionData(conversionData));
        }

        [Test]
        public void IsMapWithConversionData_Mix_Case2()
        {
            var config = new CCConversionConfig
            {
                campaign = "test_campaign",
                campaign_id = "12345",
                adgroup_id = "test_adgroup",
            };

            var conversionData = new Dictionary<string, object>
            {
                { "campaign", "test_campaign" },
                { "campaign_id", "12345" },
                { "adset", "test_adset" },
                { "adset_id", null },
                { "adgroup_id", null },
                { "media_source", "test_media_source" },
                { "install_time", "2023-10-01T12:00:00Z" },
                { "af_siteid", "test_af_siteid" },
                { "extra_field", "extra_value" } // Extra field should not affect the result
            };

            Assert.IsFalse(config.IsMapWithConversionData(conversionData));
        }

        [Test]
        public void IsMapWithConversionData_Campaign_Case7()
        {
            // Config only has campaign, conversion has different campaign but any campaign_id
            var config = new CCConversionConfig
            {
                campaign = "test_campaign",
                campaign_id = null,
            };

            var conversionData = new Dictionary<string, object>
            {
                { "campaign", "test_campaign_new" },
                { "campaign_id", "999" },
            };

            Assert.IsFalse(config.IsMapWithConversionData(conversionData));
        }

        [Test]
        public void IsMapWithConversionData_Campaign_Case8()
        {
            // Config only has campaign_id, conversion has different campaign_id but any campaign
            var config = new CCConversionConfig
            {
                campaign = null,
                campaign_id = "12345",
            };

            var conversionData = new Dictionary<string, object>
            {
                { "campaign", "test_campaign" },
                { "campaign_id", "999" },
            };

            Assert.IsFalse(config.IsMapWithConversionData(conversionData));
        }

        [Test]
        public void IsMapWithConversionData_Campaign_Case9()
        {
            // Config only has campaign, conversion has same campaign without campaign_id
            var config = new CCConversionConfig
            {
                campaign = "test_campaign",
            };

            var conversionData = new Dictionary<string, object>
            {
                { "campaign", "test_campaign" },
            };

            Assert.IsTrue(config.IsMapWithConversionData(conversionData));
        }

        [Test]
        public void IsMapWithConversionData_Campaign_Case10()
        {
            // Config only has campaign_id, conversion has same campaign_id with different campaign
            var config = new CCConversionConfig
            {
                campaign_id = "12345",
            };

            var conversionData = new Dictionary<string, object>
            {
                { "campaign", "test_campaign" },
                { "campaign_id", "12345" },
            };

            Assert.IsTrue(config.IsMapWithConversionData(conversionData));
        }

        [Test]
        public void IsMapWithConversionData_Campaign_Case11()
        {
            // Conversion campaign_id is non-string value
            var config = new CCConversionConfig
            {
                campaign_id = "12345",
            };

            var conversionData = new Dictionary<string, object>
            {
                { "campaign", "test_campaign" },
                { "campaign_id", 12345L },
            };

            Assert.IsTrue(config.IsMapWithConversionData(conversionData));
        }

        [Test]
        public void IsMapWithConversionData_Adset_Case3()
        {
            // Config only has adset, conversion has different adset but any adset_id
            var config = new CCConversionConfig
            {
                adset = "test_adset",
                adset_id = null,
            };

            var conversionData = new Dictionary<string, object>
            {
                { "campaign", "test_campaign" },
                { "adset", "test_adset_new" },
                { "adset_id", "999" },
            };

            Assert.IsFalse(config.IsMapWithConversionData(conversionData));
        }

        [Test]
        public void IsMapWithConversionData_Adset_Case4()
        {
            // Config only has adset_id, conversion has different adset_id but any adset
            var config = new CCConversionConfig
            {
                adset = null,
                adset_id = "12345",
            };

            var conversionData = new Dictionary<string, object>
            {
                { "campaign", "test_campaign" },
                { "adset", "test_adset" },
                { "adset_id", "999" },
            };

            Assert.IsFalse(config.IsMapWithConversionData(conversionData));
        }

        [Test]
        public void IsMapWithConversionData_Adset_Case5()
        {
            // Match by adset_id while adset is different
            var config = new CCConversionConfig
            {
                adset = "test_adset",
                adset_id = "12345",
            };

            var conversionData = new Dictionary<string, object>
            {
                { "campaign", "test_campaign" },
                { "adset", "test_adset_new" },
                { "adset_id", "12345" },
            };

            Assert.IsTrue(config.IsMapWithConversionData(conversionData));
        }

        [Test]
        public void IsMapWithConversionData_Adset_Case6()
        {
            // Config only has adset, conversion has same adset without adset_id
            var config = new CCConversionConfig
            {
                adset = "test_adset",
            };

            var conversionData = new Dictionary<string, object>
            {
                { "campaign", "test_campaign" },
                { "adset", "test_adset" },
            };

            Assert.IsTrue(config.IsMapWithConversionData(conversionData));
        }

        [Test]
        public void IsMapWithConversionData_MediaSource_Case1()
        {
            var config = new CCConversionConfig
            {
                media_source = "test_media_source",
            };

            var conversionData = new Dictionary<string, object>
            {
                { "campaign", "test_campaign" },
                { "media_source", "test_media_source" },
            };

            Assert.IsTrue(config.IsMapWithConversionData(conversionData));
        }

        [Test]
        public void IsMapWithConversionData_MediaSource_Case2()
        {
            var config = new CCConversionConfig
            {
                media_source = "test_media_source",
            };

            var conversionData = new Dictionary<string, object>
            {
                { "campaign", "test_campaign" },
                { "media_source", "other_media_source" },
            };

            Assert.IsFalse(config.IsMapWithConversionData(conversionData));
        }

        [Test]
        public void IsMapWithConversionData_MediaSource_Case3()
        {
            // Conversion data has no media_source
            var config = new CCConversionConfig
            {
                media_source = "test_media_source",
            };

            var conversionData = new Dictionary<string, object>
            {
                { "campaign", "test_campaign" },
            };

            Assert.IsFalse(config.IsMapWithConversionData(conversionData));
        }

        [Test]
        public void IsMapWithConversionData_AfSiteId_Case1()
        {
            var config = new CCConversionConfig
            {
                af_siteid = "test_af_siteid",
            };

            var conversionData = new Dictionary<string, object>
            {
                { "campaign", "test_campaign" },
                { "af_siteid", "other_af_siteid" },
            };

            Assert.IsFalse(config.IsMapWithConversionData(conversionData));
        }

        [Test]
        public void IsMapWithConversionData_InstallTime_Case1()
        {
            var config = new CCConversionConfig
            {
                install_time = "2023-10-01T12:00:00Z",
            };

            var conversionData = new Dictionary<string, object>
            {
                { "campaign", "test_campaign" },
                { "install_time", "2023-10-01T12:00:00Z" },
            };

            Assert.IsTrue(config.IsMapWithConversionData(conversionData));
        }

        [Test]
        public void IsMapWithConversionData_Empty_Case1()
        {
            var config = new CCConversionConfig
            {
                campaign = "test_campaign",
            };

            Assert.IsFalse(config.IsMapWithConversionData(null));
            Assert.IsFalse(config.IsMapWithConversionData(new Dictionary<string, object>()));
        }

        [Test]
        public void IsMapWithConversionData_Adjust_Case1()
        {
            // Adjust conversion data is mapped to AppsFlyer format
            var config = new CCConversionConfig
            {
                media_source = "test_network",
                campaign = "test_campaign",
                adset = "test_creative",
                adgroup_id = "test_adgroup",
                af_siteid = "test_tracker_token",
            };

            var conversionData = new Dictionary<string, object>
            {
                { "trackerToken", "test_tracker_token" },
                { "trackerName", "test_tracker_name" },
                { "network", "test_network" },
                { "campaign", "test_campaign" },
                { "adgroup", "test_adgroup" },
                { "creative", "test_creative" },
                { "clickLabel", null },
                { "costType", null },
                { "costAmount", null },
                { "costCurrency", null },
            };

            Assert.IsTrue(config.IsMapWithConversionData(conversionData));
        }

        [Test]
        public void IsMapWithConversionData_Adjust_Case2()
        {
            // Adjust network does not match media_source
            var config = new CCConversionConfig
            {
                media_source = "test_network",
                campaign = "test_campaign",
            };

            var conversionData = new Dictionary<string, object>
            {
                { "trackerToken", "test_tracker_token" },
                { "network", "other_network" },
                { "campaign", "test_campaign" },
            };

            Assert.IsFalse(config.IsMapWithConversionData(conversionData));
        }

        [Test]
        public void IsMapWithConversionData_Adjust_Case3()
        {
            // AppsFlyer field takes priority over Adjust field
            var config = new CCConversionConfig
            {
                media_source = "test_media_source",
            };

            var conversionData = new Dictionary<string, object>
            {
                { "media_source", "test_media_source" },
                { "network", "test_network" },
            };

            Assert.IsTrue(config.IsMapWithConversionData(conversionData));
        }
    }
}
