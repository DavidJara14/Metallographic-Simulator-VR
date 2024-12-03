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

    [SerializeField] float humedad = 0;

    public TextMeshProUGUI text;
    public MeshRenderer meshRenderer;

    public float GetHumedad()
    {
        if(humedad <= 0)
            return 0;
        humedad = Mathf.Clamp(humedad, 0, 50);
        return humedad;
    }

    private void OnParticleCollision(GameObject other)
    {

        var elemento = other.GetComponentInParent<BotellaLab>();

        if (elemento != null)
        {
            string tipo = elemento.Tipo;
            if (tipo == "Agua")
            {
                humedad += 2;
            }
        }
    }

    public void OnPoolSpawn(ref LijaDataholder referenceGOComponent, int tamañoDeGrano)
    {
        TamañoDeGrano = tamañoDeGrano;
        if(referenceGOComponent != null && referenceGOComponent != null) 
            referenceDataholder = referenceGOComponent;
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
        Debug.Log($"Deserialization: Tamaño de grano TEXT set to {TamañoDeGrano}");
        meshRenderer.material = referenceDataholder.MaterialesSegunTamañosDeLija[referenceDataholder.LijaDict[TamañoDeGrano].Int];
    }
}
