
using System.Collections.Generic;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class TestGetCustomParticleData : UdonSharpBehaviour
{
    public List<Vector4> ahahahaha;

    private void OnParticleCollision(GameObject other)
    {
        Debug.Log(other);  
        Debug.Log(other.GetComponent<ParticleSystem>());
        Debug.Log(other.GetComponent<ParticleSystem>().GetCustomParticleData(ahahahaha, ParticleSystemCustomData.Custom1));
        other.GetComponent<ParticleSystem>().GetCustomParticleData(ahahahaha, ParticleSystemCustomData.Custom1);
        Debug.Log(ahahahaha);
    }


}
