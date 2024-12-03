
using BestHTTP.SecureProtocol.Org.BouncyCastle.Math.Field;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

public class BoteBasura : UdonSharpBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.gameObject.GetComponent<LijaCircularBehabiour>())
        {
            collision.gameObject.GetComponent<LijaCircularBehabiour>().ReturnToPool();
        }
        else if (collision.gameObject.GetComponent<VRCObjectSync>())
        {
            collision.gameObject.GetComponent<VRCObjectSync>().Respawn();
        }
        else
        {
            Debug.Log(collision.gameObject.name + " touched " +  gameObject.name);
        }
    }
}
