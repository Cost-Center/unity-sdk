using UnityEngine;
using CostCenter.Attribution;

namespace CostCenter {
    public class CCServices : MonoBehaviour
    {
        [SerializeField] private bool _dontDestroy = false;

        void Start() {
            if (_dontDestroy) {
                DontDestroyOnLoad(gameObject);
            }
        }

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
