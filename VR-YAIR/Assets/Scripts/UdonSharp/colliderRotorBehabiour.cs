
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class colliderRotorBehabiour : UdonSharpBehaviour
{
    public bool isRotating = false;
    public bool colliderRotor()
    {
        return isRotating;
    }
}
