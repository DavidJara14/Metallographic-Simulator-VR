using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class PistolaDeCalor : UdonSharpBehaviour
{
    //public GameObject TriggerGO;
    public CapsuleCollider collisionPistol;
    [SerializeField] private bool Used;
    [SerializeField] private Animator _animator;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private ParticleSystem _particleSystem;
    public override void OnPickupUseDown()
    {
        Used = !Used;
        if(Used)
        {
            _particleSystem.Play();
        }
        else
        {
            _particleSystem.Stop();
        }
        //TriggerGO.SetActive(Used);
        collisionPistol.enabled = Used;
       _animator.SetBool("Used", Used);
        if(Used)
        {
            _audioSource.Play();
        }
        else
        {
            _audioSource.Stop();
        }
    }

}
