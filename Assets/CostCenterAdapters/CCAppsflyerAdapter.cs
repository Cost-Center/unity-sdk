using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using AppsFlyerSDK;
namespace CostCenter.Attribution
{
    public class CCAppsflyerAdapter : CCMMP
    {
        public override void CheckAndGetAttributionID(Action<string> callback)
        {
            base.CheckAndGetAttributionID(callback);
            StartCoroutine(C_WaitForAttributionID());
        }
        private IEnumerator C_WaitForAttributionID()
        {

            
            string attributionID = string.Empty;
            List<float> waitingTimes = new List<float>() { 2, 5, 5, 10, 10, 20, 20, 50, 50, 100 };
            
            while (string.IsNullOrEmpty(_attributionID) == true && waitingTimes.Count > 0)
            {
                _attributionID = AppsFlyer.getAttributionId();
                
                   
                SetAttributionIDToThirdParty(_attributionID);
                
                Debug.Log($"CCSDK - CCAppsflyerAdapter: Try to Get Adid");
                float waitingTime = waitingTimes[0];
                waitingTimes.RemoveAt(0);
                yield return new WaitForSeconds(waitingTime);
                if (string.IsNullOrEmpty(_attributionID) == false)
                {
                    break;
                }
            }
        }

        private string _attributionID = string.Empty;
        private void SetAttributionIDToThirdParty(string attributionID)
        {
            
            if (string.IsNullOrEmpty(attributionID) == true)
            {
                return;
            }
            Debug.Log($"CCSDK - CCAppsflyerAdapter: Found an attributionID {attributionID}");
            if (onGetAttributionID != null)
            {
                onGetAttributionID?.Invoke(attributionID);
            }
        }
    }
}