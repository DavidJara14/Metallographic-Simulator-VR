using UdonSharp;
using UnityEngine;
using TMPro;
using VRC.SDKBase;
using VRC.Udon;
using System;
using VRC.SDK3.Data;

public class LijaCircularBehabiour : UdonSharpBehaviour
{
    //Etiqueta, no eliminar

    [SerializeField][UdonSynced] public int TamañoDeGrano;
    [SerializeField] LijaDataholder referenceDataholder;
    private bool WasPicked;

    public TextMeshProUGUI text;
    public MeshRenderer meshRenderer;

    public void OnPoolSpawn(ref LijaDataholder referenceGOComponent, int tamañoDeGrano)
    {
        TamañoDeGrano = tamañoDeGrano;
        if(referenceGOComponent != null && referenceGOComponent != null) 
            referenceDataholder = referenceGOComponent;
        Debug.Log($"Tamaño de grano set to {TamañoDeGrano}");
        text.text = TamañoDeGrano.ToString();
        meshRenderer.material = referenceDataholder.MaterialesSegunTamañosDeLija[referenceDataholder.LijaDict[tamañoDeGrano].Int];
        GetComponent<Rigidbody>().isKinematic = true;
        RequestSerialization();
    }

    public override void OnPickup()
    {
        GetComponent<Rigidbody>().isKinematic = false;
        WasPicked = true;
    }

    public override void OnDeserialization()
    {
        if (referenceDataholder == null)
        {
            Debug.Log($"No referenceDataholder assigned to {gameObject}, disabling GO.");
            gameObject.SetActive(false);
            return;
        }
        text.text = TamañoDeGrano.ToString();
        Debug.Log($"Tamaño de grano TEXT set to {TamañoDeGrano}");
        meshRenderer.material = referenceDataholder.MaterialesSegunTamañosDeLija[referenceDataholder.LijaDict[TamañoDeGrano].Int];
    }
}
