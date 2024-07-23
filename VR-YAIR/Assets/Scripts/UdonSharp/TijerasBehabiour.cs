using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class TijerasBehabiour : UdonSharpBehaviour
{
    public Animator animator;

    public void SetAnimTrue()
    {
        animator.SetBool("Used", true);
    }

    public void SetAnimFalse()
    {
        animator.SetBool("Used", false);
    }

    public override void OnPickupUseDown()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "SetAnimTrue");
        //animator.SetBool("Used", true);
    }

    public override void OnPickupUseUp()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "SetAnimFalse");
    }

    public override void OnDrop()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "SetAnimFalse");
    }

}
