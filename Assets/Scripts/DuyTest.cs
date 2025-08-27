using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CostCenter.Attribution;
using CostCenter;

public class DuyTest : MonoBehaviour
{
    public Text _log;

    // Start is called before the first frame update
    void Start()
    {
        // CCAttribution.RequestAppTrackingTransparency((idfa) => {
        //     _log.text = $"IDFA: {idfa}";
        // });
        // CCAttribution.instance.TrackingAttribution("duyid");
        // AppSetIdManager.Instance.GetAppSetId((appSetId) =>
        // {
        //     Debug.Log("CC App Set ID: " + appSetId);
        // });
    }

    public void RequestATT() {
        _log.text = "Requesting ATT...";
        CCAttribution.RequestAppTrackingTransparency();
    }
}
