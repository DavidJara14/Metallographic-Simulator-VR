
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class RotateProbe : UdonSharpBehaviour
{
    public bool activate = false;
    public Rigidbody probeRB;
    public Vector3 newCenterOfMass;
    public Vector3 OldCenterOfMass;


    void Start()
    {
        probeRB = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        probeRB = GetComponent<Rigidbody>();
    }

    public override void Interact()
    {
        activate = !activate;
        if (activate) 
        {
            probeRB.centerOfMass = newCenterOfMass;
        }
        else if (!activate)
        {
            probeRB.centerOfMass = OldCenterOfMass;
        }
    }
}
