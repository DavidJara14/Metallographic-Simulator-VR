using UdonSharp;
using UnityEngine;
using UnityEngine.Events;
using VRC.SDK3.Data;
using VRC.SDKBase;
using VRC.Udon;

public class EventCallerOnHoldOnDrop : UdonSharpBehaviour
{
    [Header("References")]
    [SerializeField] UdonBehaviour[] UdonBehabiourListeners;
    [Header("Configuration")]
    [SerializeField] string OnPickupEventName = "";
    [SerializeField] string OnDropEventName = "";

    public override void OnPickup()
    {
        for (int i = 0; i < UdonBehabiourListeners.Length; i++)
        {
            UdonBehabiourListeners[i].SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All,
                OnPickupEventName);
        }
    }

    public override void OnDrop()
    {
        for (int i = 0; i < UdonBehabiourListeners.Length; i++)
        {
            UdonBehabiourListeners[i].SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All,
                OnDropEventName);
        }
    }
}
