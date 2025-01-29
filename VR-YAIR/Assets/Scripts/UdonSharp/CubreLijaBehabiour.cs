using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class CubreLijaBehabiour : UdonSharpBehaviour
{
    public GameObject cubrelijaReference;
    public LijaRotation lijaRotation;
    public PulidoraScript pulidoraScript;

    public bool isPulidora;
    public bool isDesbastadora;

    private void Update()
    {
        if(lijaRotation != null)
        {
            if (lijaRotation.Rotating)
            {
                gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                gameObject.GetComponent<MeshCollider>().enabled = false;
            }
            if (!lijaRotation.Rotating)
            {
                gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                gameObject.GetComponent<MeshCollider>().enabled = true;
            }
        }

        if(pulidoraScript != null)
        {
            gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            gameObject.GetComponent<MeshCollider>().enabled = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.GetComponent<LijaRotation>() != null)
        {
            gameObject.GetComponent<MeshCollider>().excludeLayers = LayerMask.GetMask("Nothing");

        }
    }
}