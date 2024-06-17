using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class TijerasBehabiour : UdonSharpBehaviour
{
    public Animator animator;

    public override void OnPickupUseDown()
    {
        animator.SetBool("Used",true);
    }

    public override void OnPickupUseUp()
    {
        animator.SetBool("Used", false);
    }

    public override void OnDrop()
    {
        animator.SetBool("Used", false);
    }

}
