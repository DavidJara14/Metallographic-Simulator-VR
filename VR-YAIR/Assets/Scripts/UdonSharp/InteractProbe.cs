
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class InteractProbe : UdonSharpBehaviour
{
    public bool activate = false;
    public GameObject canva;

    private void Start()
    {
        canva.SetActive(false);
    }

    public override void Interact()
    {
        activate = !activate;
        if (activate)
        {
            canva.SetActive(true);
        }
        else if (!activate) 
        {
            canva.SetActive(false);
            canva.GetComponent<UpdatePreview>().SendCustomEvent("scaleAllMaterial");
        }
    }

    public void DisableCanva()
    {
        canva.SetActive(false);
    }
}





