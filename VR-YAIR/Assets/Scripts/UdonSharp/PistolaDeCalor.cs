using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class PistolaDeCalor : UdonSharpBehaviour
{
    public GameObject TriggerGO;
    [SerializeField] private Animator _animator;
    [SerializeField] private AudioSource _audioSource;
    public override void OnPickupUseDown()
    {
        TriggerGO.SetActive(!TriggerGO.activeSelf);
        _animator.SetBool("Used", true);
        _audioSource.Play();
    }

    public override void OnPickupUseUp()
    {
        _animator.SetBool("Used", false);
        _audioSource.Stop();
    }

    public override void OnDrop()
    {
        _animator.SetBool("Used", false);
        _audioSource.Stop();
    }
}
