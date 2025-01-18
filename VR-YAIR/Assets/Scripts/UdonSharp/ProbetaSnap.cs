
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ProbetaSnap : UdonSharpBehaviour
{

    [SerializeField] bool ProbetaLoaded;
    [SerializeField] bool Stayed;

    [SerializeField] public GameObject ProbetaGO = null;
    [SerializeField] public VRC_Pickup Probeta = null;

    private void Update()
    {
        if (Probeta == null)
        {
            if (ProbetaGO != null)
            {
                Probeta = ProbetaGO.GetComponent<VRC_Pickup>();
            }
        }
    }

    public void OnProbetaSnap(Transform go)
    {
        ProbetaLoaded = true;
        go.SetParent(gameObject.transform);
        Debug.Log(go);
        ProbetaGO = go.gameObject;
        Debug.Log(ProbetaGO);
        Probeta = ProbetaGO.GetComponent<VRC_Pickup>();
        Debug.Log(Probeta);
    }

    public void RemoveProbeta(Transform go)
    {
        ProbetaLoaded = false;
        go.GetComponent<VRC_Pickup>().pickupable = true;
        go.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        go.parent = null;
        ProbetaGO = null;
        Probeta = null;
    }


    private void OnTriggerStay(Collider other)
    {
        if (Probeta != null) return;
        if (!other.GetComponent<ProbeBehabiour>()) { return; }
        if (other.gameObject.GetComponent<VRC_Pickup>().currentPlayer != null) { return; }
        if (!Stayed)
        {
            //Debug.Log("es Probeta");
            OnProbetaSnap(other.gameObject.transform);
            Stayed = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.GetComponent<ProbeBehabiour>())
            return;
        RemoveProbeta(other.gameObject.transform);
        Stayed = false;
    }


}
