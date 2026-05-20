using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDK3.Data;
using VRC.SDKBase;
using VRC.Udon;

public class LijaDataholder : UdonSharpBehaviour
{
    public VRCObjectPool LijaPool;

    public Material[] MaterialesSegunTamañosDeLija;

    public DataDictionary LijaDict = new DataDictionary()
    {
        {120, 0},
        {180, 1},
        {240, 2},
        {360, 3},
        {400, 4},
        {500, 5},
        {600, 6},
        {800, 7},
    };

    private void Start()
    {
        Debug.Log(MaterialesSegunTamañosDeLija.Length);
    }

}
