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
    //private Material _materialLija;

    public TextMeshProUGUI text;
    public MeshRenderer meshRenderer;

    private void Start()
    {
        
    }

    public void OnPoolSpawn(ref LijaDataholder referenceGOComponent, int tamañoDeGrano)
    {
        TamañoDeGrano = tamañoDeGrano;
        if(referenceGOComponent != null && referenceGOComponent != null) 
            referenceDataholder = referenceGOComponent;
        //_materialLija = referenceGOComponent.MaterialesSegunTamañosDeLija[referenceGOComponent.LijaDict[tamañoDeGrano].Int];
        Debug.Log($"Tamaño de grano set to {TamañoDeGrano}");
        text.text = TamañoDeGrano.ToString();
        meshRenderer.material = referenceDataholder.MaterialesSegunTamañosDeLija[referenceDataholder.LijaDict[tamañoDeGrano].Int];
        RequestSerialization();
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
