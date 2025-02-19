
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class CottonBehabiour : UdonSharpBehaviour
{
    //Clase etiqueta, no borrar

    [SerializeField] Material cottonMat;

    private void Start()
    {
        cottonMat = GetComponentInParent<MeshRenderer>().material;
    }

    public void AddAlcohol()
    {
        cottonMat.SetFloat("_AditionalPower", 3.5f);
        cottonMat.SetFloat("_HasAlcohol", 1); // https://www.reddit.com/r/Unity3D/comments/wgmias/property_rect_already_exists_in_the_property/?rdt=53009
    }

}
