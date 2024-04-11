using UdonSharp;
using UnityEngine;
using UnityEngine.Rendering;
using VRC.SDKBase;
using VRC.Udon;

public class IsProbeTouched : UdonSharpBehaviour
{
    [SerializeField] bool isTouched;
    [SerializeField] string ProgramingVarName;
    [SerializeField] UdonBehaviour ProbeMainScript;
    [SerializeField] ProbeBehabiour behabiour;

    private void OnTriggerEnter(Collider other)
    {
        isTouched = true;
        behabiour.SetVar(ProgramingVarName, isTouched);
        Debug.Log("trued " + gameObject.name);
    }

    private void OnTriggerExit(Collider other)
    {
        isTouched = false;
        behabiour.SetVar(ProgramingVarName, isTouched);
        Debug.Log("falsed " + gameObject.name);
    }

}
