
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class IdentifyFace : UdonSharpBehaviour
{
    [SerializeField] private GameObject myShader;
    [SerializeField] private GameObject myMirror;
    [SerializeField] private ProbeBehabiour probeBehabiour;
    [SerializeField] private ParticleSystem residuosMaterial;

    private void OnTriggerEnter(Collider other)
    {
        Debug.LogWarning("Face: " + gameObject.name + " Collision with: " + other.gameObject.name);
        if(other.gameObject.GetComponent<LijaCircularBehabiour>() || other.gameObject.GetComponent<PulidoraScript>())
        {
            probeBehabiour.probetaShader = myShader;
            probeBehabiour.Desgaste = myShader.GetComponent<Renderer>().material.GetFloat("_GranoLija");
            probeBehabiour.EsteParticleSystem = residuosMaterial;
        }
    }
}
