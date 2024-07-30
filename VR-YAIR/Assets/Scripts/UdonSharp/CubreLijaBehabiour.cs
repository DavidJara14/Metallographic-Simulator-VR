
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class CubreLijaBehabiour : UdonSharpBehaviour
{
    public GameObject cubrelijaReference;
    void Start()
    {

    }

    private void Update()
    {
        if (cubrelijaReference.GetComponent<CubreLijaSnap>().CubreLijaLoaded)
        {
            gameObject.GetComponent<MeshCollider>().enabled = false;
        }
        else if (!cubrelijaReference.GetComponent<CubreLijaSnap>().CubreLijaLoaded)
        {
            gameObject.GetComponent<MeshCollider>().enabled = true;

        }
    }
}
