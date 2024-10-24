using System.Collections.Generic;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDKBase;
using VRC.Udon;

public class SkyboxSetup : UdonSharpBehaviour
{
    [SerializeField] private Material SkyboxMaterial;
    private DataDictionary StarsIntensity = new DataDictionary()
    {
        {"PC", 3f},
        {"VR", 0.6f}
    };

    void Start()
    {
        if (SkyboxMaterial == null)
            return;
        if(Networking.LocalPlayer.IsUserInVR())
        {
            SkyboxMaterial.SetFloat("_StarsIntensity", StarsIntensity["VR"].Float);
            Debug.Log(Networking.LocalPlayer.displayName + " is a VR user, setting Skybox to " + StarsIntensity["VR"].Float.ToString());
        }
        else
        {
            SkyboxMaterial.SetFloat("_StarsIntensity", StarsIntensity["PC"].Float);
            Debug.Log(Networking.LocalPlayer.displayName + " is a PC user, setting Skybox to " + StarsIntensity["PC"].Float.ToString());
        }
        gameObject.SetActive(false);
    }
}
