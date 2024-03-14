using UdonSharp;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using VRC.SDK3.Data;
using VRC.SDKBase;
using VRC.Udon;

public class EventCallerOnHoldOnDrop : UdonSharpBehaviour
{
    [Header("References")]
    [SerializeField] GameObject[] GOListeners;
    [SerializeField] UdonBehaviour[] UdonBehabiourListenersRef;
    [Header("Configuration")]
    [SerializeField] string[] OnPickupEventNames;
    [SerializeField] string[] OnDropEventNames;

    private void Start()
    {
        //var count = 0;
        DataList list = new DataList();
        for (int i = 0; i < GOListeners.Length; i++)
        {
            foreach (var item in GOListeners[i].GetComponents<UdonBehaviour>())
            {
                list.Add(item);
            }
        }
        UdonBehabiourListenersRef = new UdonBehaviour[list.Count];
        for (int i = 0;i < UdonBehabiourListenersRef.Length; i++)
        {
            UdonBehabiourListenersRef[i] = (UdonBehaviour)list[i].Reference;
        }
    }

    public override void OnPickup()
    {
        for (int i = 0; i < UdonBehabiourListenersRef.Length; i++)
        {
            for(int j = 0; j < OnPickupEventNames.Length; j++)
            {
                UdonBehabiourListenersRef[i].SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All,
                    OnPickupEventNames[j]);
            }
        }
    }

    public override void OnDrop()
    {
        for (int i = 0; i < UdonBehabiourListenersRef.Length; i++)
        {
            for( int j = 0;j < OnDropEventNames.Length;j++)
            {
                UdonBehabiourListenersRef[i].SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All,
                    OnDropEventNames[j]);
            }
        }
    }
}
