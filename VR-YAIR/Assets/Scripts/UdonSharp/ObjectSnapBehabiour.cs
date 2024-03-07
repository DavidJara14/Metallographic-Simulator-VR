
using UdonSharp;
using UdonSharpEditor;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ObjectSnapBehabiour : UdonSharpBehaviour
{
    [SerializeField] bool SnapPosition;
    [SerializeField] bool SnapRotation;

    public void OnDropCustom()
    {
        GameObject var = this.gameObject;
        if(var.GetComponentInParent<ObjectSnapBehabiour>() != null)//Checar por layer Pickup, con un spherecast
        {

        }
    }

}
