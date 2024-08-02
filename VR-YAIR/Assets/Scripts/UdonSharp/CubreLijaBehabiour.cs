using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class CubreLijaBehabiour : UdonSharpBehaviour
{
    public GameObject cubrelijaReference;
    public GameObject rotor;
    public bool isPulidora;
    public bool isDesbastadora;

    void Start()
    {

    }

    private void Update()
    {/*
        if(rotor.GetComponent<LijaRotation>().Rotating && rotor.GetComponent<LijaRotation>().isEnergized && isDesbastadora)
        {
            gameObject.GetComponent<MeshCollider>().enabled = false;
        }
        if (!rotor.GetComponent<LijaRotation>().Rotating && rotor.GetComponent<LijaRotation>().isEnergized && isDesbastadora)
        {
            gameObject.GetComponent<MeshCollider>().enabled = true;
        }
        */
        if (isDesbastadora && rotor.GetComponent<LijaRotation>().isEnergized && cubrelijaReference.GetComponent<CubreLijaSnap>().CubreLijaLoaded)
        {
            gameObject.GetComponent<MeshCollider>().enabled = rotor.GetComponent<LijaRotation>().enableMeshCL;
            if (rotor.GetComponent<LijaRotation>().Rotating)
                gameObject.GetComponent<MeshCollider>().enabled = false;

        }


        if (!isPulidora)
            return;
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