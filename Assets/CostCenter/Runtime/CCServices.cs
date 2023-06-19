using UnityEngine;
using CostCenter.Attribution;

namespace CostCenter {
    public class CCServices : MonoBehaviour
    {
        public void AddAttribution()
        {
            if (gameObject.GetComponent<CCAttribution>()) {
                Debug.LogError("CCAttribution has already added.");
                return;
            }
            gameObject.AddComponent<CCAttribution>();
        }
    }
}
