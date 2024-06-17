using System.Collections.Generic;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDKBase;
using VRC.Udon;

public class LijaCuadrada : UdonSharpBehaviour
{

    public GameObject[] Lijas;
    public int TamañoDeGrano;

    [SerializeField] private GameObject[] Placers;
    private DataDictionary LijaDict = new DataDictionary()
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

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("trigger " + other);
        if (other.GetComponentInParent<TijerasBehabiour>())
        {
            DataToken index;
            if (LijaDict.TryGetValue(TamañoDeGrano, TokenType.Int, out index))
            {
                var go = Instantiate(Lijas[index.Int], gameObject.transform.position, gameObject.transform.rotation);
                if(Placers.Length != 0) 
                    go.gameObject.gameObject.gameObject.GetComponent<EventCallerOnHoldOnDrop>().SetPlacers(Placers);
                gameObject.SetActive(false);
            }
            else
            {
                Debug.LogWarning(string.Format("No lija de tamaño {0} en el diccionario", TamañoDeGrano));
            }
        }
    }
}
