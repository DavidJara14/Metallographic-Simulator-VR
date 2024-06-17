using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class PistolaDeCalor : UdonSharpBehaviour
{
    public GameObject TriggerGO;
    [SerializeField] private bool Used;
    [SerializeField] private Animator _animator;
    [SerializeField] private AudioSource _audioSource;
    public override void OnPickupUseDown()
    {
        TriggerGO.SetActive(Used);
        _animator.SetBool("Used", Used);
        if(Used)
        {
            _audioSource.Play();
        }
        else
        {
            _audioSource.Stop();
        }
        Used = !Used;
    }

}
