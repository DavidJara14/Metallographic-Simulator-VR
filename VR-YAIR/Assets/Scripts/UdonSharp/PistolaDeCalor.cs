using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class PistolaDeCalor : UdonSharpBehaviour
{
    public GameObject TriggerGO;
    public override void OnPickupUseDown()
    {
        TriggerGO.SetActive(!TriggerGO.activeSelf);
    }
}
