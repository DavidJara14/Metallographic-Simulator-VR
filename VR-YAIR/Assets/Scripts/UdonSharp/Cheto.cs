
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Cheto : UdonSharpBehaviour
{
    [SerializeField] private Transform _ref;
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private AudioSource _audioSource;

    public override void OnDrop()
    {
        gameObject.transform.position = _ref.position;
        _particleSystem.Emit(1);
        _audioSource.Play();
    }
}
