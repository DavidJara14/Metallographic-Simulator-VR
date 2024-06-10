using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class xdxd : UdonSharpBehaviour
{
    private void OnParticleCollision(GameObject other)
    {
        Debug.Log(other.name + "in" + this);
    }

    private void OnParticleTrigger()
    {
        Debug.Log("triggered in" + this);
    }
}
