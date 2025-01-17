
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Heatable : UdonSharpBehaviour
{

    [SerializeField] private UdonBehaviour[] UdonBehabiourListenersRef;
    const string VAR_NAME = "newcalor";

    public void ActivateCalor()
    {
        Debug.LogWarning("Sending Activation event to " + UdonBehabiourListenersRef.Length + " scripts");
        for (int i = 0; i < UdonBehabiourListenersRef.Length; i++)
        {
            UdonBehabiourListenersRef[i].SetProgramVariable(VAR_NAME, true);
            Debug.Log("Activated '" + VAR_NAME + "' to script " + UdonBehabiourListenersRef[i].gameObject.name);
        }
    }

    public void DeactivateCalor()
    {
        Debug.LogWarning("Sending Deactivation event to " + UdonBehabiourListenersRef.Length + " scripts");
        for (int i = 0; i < UdonBehabiourListenersRef.Length; i++)
        {
            UdonBehabiourListenersRef[i].SetProgramVariable(VAR_NAME, false);
            Debug.Log("Deactivated '" + VAR_NAME + "' to script " + UdonBehabiourListenersRef[i].gameObject.name);
        }
    }

}
