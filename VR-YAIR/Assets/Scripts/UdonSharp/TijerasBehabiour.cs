using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class TijerasBehabiour : UdonSharpBehaviour
{
    public Animator animator;
    [Header("Audio Config")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _audioClip;

    public void SetAnimTrue()
    {
        animator.SetBool("Used", true);
        _audioSource.PlayOneShot(_audioClip);
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
