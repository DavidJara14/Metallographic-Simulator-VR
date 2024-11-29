using System.Collections.Generic;
using TMPro;
using UdonSharp;
using Unity.Properties;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDKBase;
using VRC.Udon;

public class LijaCuadrada : UdonSharpBehaviour
{

    public LijaDataholder ReferenceGOComponent;

    public GameObject[] Lijas;
    public int TamañoDeGrano;

    //[UdonSynced] private bool isActive = true;

    public TextMeshProUGUI text;
    [SerializeField] private GameObject[] Placers;

    private void Start()
    {
        if (ReferenceGOComponent == null)
            Debug.LogError($"{this}: No ReferenceGOComponent found, add one to the scene and assign it");
        text.text = TamañoDeGrano.ToString();
        gameObject.GetComponent<MeshRenderer>().material.SetFloat("_TamanioDeLija", TamañoDeGrano);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<TijerasBehabiour>())
        {
            Debug.Log("trigger " + other);  
            //OnTijeraTrigger();
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "OnTijeraTriggerNE");
        }
    }

    public void OnTijeraTrigger()
    {
        
        DataToken index;
        if (ReferenceGOComponent.LijaDict.TryGetValue(TamañoDeGrano, TokenType.Int, out index))
        {
            Networking.SetOwner(Networking.LocalPlayer, ReferenceGOComponent.LijaPool.gameObject);
            var go = ReferenceGOComponent.LijaPool.TryToSpawn();
            Debug.Log(go.name);
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

    public void OnTijeraTriggerNE()
    {
        DataToken index;
        if (ReferenceGOComponent.LijaDict.TryGetValue(TamañoDeGrano, TokenType.Int, out index))
        {
            if (!Networking.IsOwner(ReferenceGOComponent.LijaPool.gameObject))
            {
                gameObject.SetActive(false);
                return;
            }
            //Networking.SetOwner(Networking.LocalPlayer, ReferenceGOComponent.LijaPool.gameObject);
            var go = ReferenceGOComponent.LijaPool.TryToSpawn();
            if(go == null)
            {
                Debug.Log($"{gameObject.name}: No Pool Object spawned");
                return;
            }
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

    public override void OnDeserialization()
    {
        //if(!isActive)
        //    gameObject.SetActive(false);
    }

}
