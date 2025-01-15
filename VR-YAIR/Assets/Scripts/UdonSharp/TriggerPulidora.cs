
using System.Collections.Generic;
using System.Threading;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDKBase;
using VRC.Udon;

public class TriggerPulidora : UdonSharpBehaviour
{

    const float TIMER_MAX = 1f;
    float ResetTimer = TIMER_MAX;
    bool startTimer = false;

    [SerializeField] GameObject coliderToShrink;
    Vector3 originalSize;

    private void Start()
    {
        originalSize = coliderToShrink.transform.localScale;
    }

    private void Update()
    {
        if(startTimer)
        {
            ResetTimer -= Time.deltaTime;
            if(ResetTimer < 0 )
            {
                ResetTimer = TIMER_MAX;
                startTimer = false;
                coliderToShrink.transform.localScale = originalSize;
                Debug.LogWarning("Originalited");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<ActivateMirror>() == null)
            return;
        if (other.GetComponent<VRC_Pickup>().currentPlayer != null)
        {
            //Debug.Log("Hay Player Agarrando");
            if (!Networking.IsOwner(Networking.LocalPlayer, this.gameObject))
            {
                Debug.Log("el player no es local, regresando");
                return;
            }
            //Debug.Log("LocalPlayer");
        }

        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Shrink");
        //Shrink();
    }

    public void Shrink()
    {
        startTimer = true;
        coliderToShrink.transform.localScale = new Vector3(0.0001f, 0.0001f, 0.0001f);
        Debug.LogWarning("shrinked");
    }
}
