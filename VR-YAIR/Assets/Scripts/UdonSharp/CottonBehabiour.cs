
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
        cottonMat.SetInteger("_HasAlcohol", 1);
    }

}
