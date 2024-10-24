using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class CubreLijaBehabiour : UdonSharpBehaviour
{
    public GameObject cubrelijaReference;
    public GameObject rotor;
    public GameObject collisionRotor;
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

        /*if (isDesbastadora && cubrelijaReference.GetComponent<CubreLijaSnap>().CubreLijaLoaded && !rotor.GetComponent<LijaRotation>().enableMeshCL)
        {
            gameObject.GetComponent<MeshCollider>().enabled = rotor.GetComponent<LijaRotation>().enableMeshCL;
            if (rotor.GetComponent<LijaRotation>().Rotating)
                gameObject.GetComponent<MeshCollider>().enabled = false;
        }

        if (isDesbastadora && !rotor.GetComponent<LijaRotation>().Rotating)
            gameObject.GetComponent<MeshCollider>().enabled = true;*/

        if(isDesbastadora)
            gameObject.GetComponent<MeshCollider>().enabled = !rotor.GetComponent<LijaRotation>().Rotating;




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

        if (isPulidora)
        {
            if(collisionRotor != null)
                collisionRotor.GetComponent<BoxCollider>().enabled = rotor.GetComponent<PulidoraScript>().Rotating;
        }

    }
}