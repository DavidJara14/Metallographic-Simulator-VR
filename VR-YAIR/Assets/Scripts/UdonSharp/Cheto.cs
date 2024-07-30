
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Cheto : UdonSharpBehaviour
{

    [SerializeField] private bool HideOnUse;

    [SerializeField] private Transform _ref;
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private AudioSource _audioSource;

    public override void OnDrop()
    {
        _particleSystem.Emit(1);
        _audioSource.Play();
        if(HideOnUse)
            gameObject.SetActive(false);
        else
            gameObject.transform.position = _ref.position;
    }
}
