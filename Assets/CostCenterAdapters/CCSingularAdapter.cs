using System;
using UnityEngine;

namespace CostCenter.Attribution
{
    public class CCSingularAdapter : CCMMP
    {
        public override void CheckAndGetAttributionID(Action<string> callback)
        {
            base.CheckAndGetAttributionID(callback);
            string attributionId = System.Guid.NewGuid().ToString();
            
            if (!string.IsNullOrEmpty(attributionId))
            {
                Singular.SingularSDK.SetCustomUserId(attributionId);
                Debug.Log($"CCSDK - CCSingularAdapter: Found an attributionID {attributionId}");
                onGetAttributionID?.Invoke(attributionId);
            }
        }
    }
}
