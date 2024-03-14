using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class OnParticleHitChangeVisual : UdonSharpBehaviour
{
    [SerializeField] private MeshRenderer m_Renderer;

    private void Start()
    {
        if (m_Renderer == null)
            m_Renderer = GetComponent<MeshRenderer>();
    }

    private void OnParticleCollision(GameObject other)
    {
        Debug.Log("PartHit");
        m_Renderer.material.color = other.GetComponent<ParticleSystem>().main.startColor.color;
    }
}
