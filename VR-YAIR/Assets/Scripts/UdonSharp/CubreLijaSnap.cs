
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class CubreLijaSnap : UdonSharpBehaviour
{
    [SerializeField] public bool CubreLijaLoaded;
    [SerializeField] bool Stayed;

    [SerializeField] public GameObject CubreLijaGo = null;
    [SerializeField] public VRC_Pickup CubreLija = null;

    private void Update()
    {
        if (CubreLija == null)
        {
            if (CubreLijaGo != null)
            {
                CubreLija = CubreLijaGo.GetComponent<VRC_Pickup>();
            }
        }
    }

    public void OnCubreLijaSnap(Transform go)
    {
        CubreLijaLoaded = true;
        go.SetParent(gameObject.transform);
        CubreLijaGo = go.gameObject;
        CubreLija = CubreLijaGo.GetComponent<VRC_Pickup>();
        go.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        //go.GetComponent<Collider>().enabled = !go.GetComponent<Collider>().enabled;
        //Debug.Log("CubreLija Sin Collider");
    }

    public void RemoveCubreLija(Transform go)
    {
        CubreLijaLoaded = false;
        go.GetComponent<VRC_Pickup>().pickupable = true;
        go.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        go.parent = null;
        CubreLijaGo = null;
        CubreLija = null;
        //go.GetComponent<Collider>().enabled = !go.GetComponent<Collider>().enabled;
        //Debug.Log("CubreLija Con Collider");
    }


    private void OnTriggerStay(Collider other)
    {
        if (CubreLija != null) return;
        if (!other.GetComponent<CubreLijaBehabiour>()) { return; }
        if (other.gameObject.GetComponent<VRC_Pickup>().currentPlayer != null) { return; }
        if (!Stayed)
        {
            OnCubreLijaSnap(other.gameObject.transform);
            Stayed = true;
        }
       // Debug.Log(Stayed);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.GetComponent<CubreLijaBehabiour>())
            return;
        RemoveCubreLija(other.gameObject.transform);
        Stayed = false;
    }

}
