using UdonSharp;
using UnityEngine;
using TMPro;
using VRC.SDKBase;
using VRC.Udon;

public class LijaCircularBehabiour : UdonSharpBehaviour
{
    //Etiqueta, no eliminar

    [SerializeField] public int TamañoDeGrano;
    public TextMeshProUGUI text;

    private void Start()
    {
        text.text = TamañoDeGrano.ToString();
    }

}
