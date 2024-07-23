using System.Collections.Generic;
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDKBase;
using VRC.Udon;

public class LijaCuadrada : UdonSharpBehaviour
{

    public LijaDataholder ReferenceGOComponent;

    public GameObject[] Lijas;
    public int TamañoDeGrano;

    public TextMeshProUGUI text;
    [SerializeField] private GameObject[] Placers;

    private void Start()
    {
        if (ReferenceGOComponent == null)
            Debug.LogError($"{this}: No ReferenceGOComponent found, add one to the scene and assign it");
        text.text = TamañoDeGrano.ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("trigger " + other);
        if (other.GetComponentInParent<TijerasBehabiour>())
        {
            OnTijeraTrigger();
            //SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "OnTijeraTrigger");
        }
    }

    public void OnTijeraTrigger()
    {
        DataToken index;
        if (ReferenceGOComponent.LijaDict.TryGetValue(TamañoDeGrano, TokenType.Int, out index))
        {
            Networking.SetOwner(Networking.LocalPlayer, ReferenceGOComponent.LijaPool.gameObject);
            var go = ReferenceGOComponent.LijaPool.TryToSpawn();
            go.transform.SetPositionAndRotation(transform.position, transform.rotation);
            go.GetComponent<LijaCircularBehabiour>().OnPoolSpawn(ref ReferenceGOComponent, TamañoDeGrano);
            //var go = Instantiate(Lijas[index.Int], gameObject.transform.position, gameObject.transform.rotation);
            if (Placers.Length != 0)
                go.gameObject.gameObject.gameObject.GetComponent<EventCallerOnHoldOnDrop>().SetPlacers(Placers);
            gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning(string.Format("No lija de tamaño {0} en el diccionario", TamañoDeGrano));
        }
    }
}
