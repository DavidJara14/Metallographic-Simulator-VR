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
    {
        if (isDesbastadora)
        {
            gameObject.GetComponent<MeshCollider>().enabled = !rotor.GetComponent<LijaRotation>().Rotating;
        }
    }
}