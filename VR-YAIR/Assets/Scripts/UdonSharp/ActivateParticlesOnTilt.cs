using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ActivateParticlesOnTilt : UdonSharpBehaviour
{
    [SerializeField] private VRC_Pickup _pickupComp;
    [SerializeField] private MeshRenderer m_Renderer;
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private Transform _UP;
    [SerializeField] private Transform _DOWN;
    [SerializeField] private Transform _Visual;

    public override void OnPickupUseDown()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "UseThisThing");        
    }

    public void UseThisThing()
    {
        var Mainn = _particleSystem.main;
        Mainn.startColor = m_Renderer.material.color;
        if (_pickupComp.currentPlayer.IsUserInVR())
        {
            if (_particleSystem.isEmitting)
            {
                _particleSystem.Stop();
            }
            else if (_UP.position.y - _DOWN.position.y <= 0)
            {
                _particleSystem.gameObject.SetActive(true);
                _particleSystem.Play();
            }
        }
        else
        {
            if (_particleSystem.isEmitting)
            {
                _Visual.transform.localRotation = Quaternion.Euler(0, 0, 0);
                //transform.Rotate(-120f, 0, 0);
                _particleSystem.Stop();
            }
            else
            {
                _Visual.transform.localRotation = Quaternion.Euler(0, 0, 120f);
                //gameObject.transform.Rotate(120f, 0, 0);
                _particleSystem.gameObject.SetActive(true);
                _particleSystem.Play();
            }
        }
    }

    public void UnUseThisThing()
    {
        _particleSystem.Stop();
        _Visual.transform.localRotation = Quaternion.Euler(0, 0, 00);
    }

    public override void OnDrop()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "UnUseThisThing");
    }

}
