
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class PourSystemTest : UdonSharpBehaviour
{
    private void OnParticleCollision(GameObject other)
    {
        Debug.Log(other.name);
    }
}
