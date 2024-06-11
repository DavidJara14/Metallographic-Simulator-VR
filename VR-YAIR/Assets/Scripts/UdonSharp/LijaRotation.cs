using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class LijaRotation : UdonSharpBehaviour
{

    [SerializeField] bool LijaLoaded;
    [SerializeField] public bool Rotating;

    [SerializeField] bool Stayed;

    [SerializeField] float RotationVelocity = 1f;

    [SerializeField] const float MaxTemp = 100f;
    [SerializeField] float Temperature = 24f;

    [SerializeField] public GameObject LijaGO = null;
    [SerializeField] public VRC_Pickup Lija = null;

    private void Update()
    {
        if(Rotating)
        {
            gameObject.transform.Rotate(Vector3.forward * RotationVelocity * Time.deltaTime);
        }
        if(Lija == null)
        {
            if(LijaGO != null)
            {
                Lija = LijaGO.GetComponent<VRC_Pickup>();
            }
        }
    }

    public void OnLijaSnap(Transform go)
    {
        LijaLoaded = true;
        go.SetParent(gameObject.transform);
        Debug.Log(go);
        LijaGO = go.gameObject;
        Debug.Log(LijaGO);
        Lija = LijaGO.GetComponent<VRC_Pickup>();
        Debug.Log(Lija);
    }

    public void RemoveLija(Transform go)
    {
        LijaLoaded = false;
        Rotating = false;
        go.GetComponent<VRC_Pickup>().pickupable = true;
        go.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        go.parent = null;
        LijaGO = null;
        Lija = null;
    }

    public void StartMachine()
    {
        if (LijaLoaded)
        {
            Rotating = !Rotating;
            Lija.pickupable = !Rotating;
            if(Lija.pickupable)
                Lija.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            else
                Lija.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
        }
        else
        {
            Rotating = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (Lija != null) return;
        if(!other.GetComponent<LijaCircularBehabiour>()) { return; }
        if(other.gameObject.GetComponent<VRC_Pickup>().currentPlayer != null) { return; }
        if(!Stayed)
        {
            Debug.Log("es lija");
            OnLijaSnap(other.gameObject.transform);
            Stayed = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.GetComponent<LijaCircularBehabiour>())
            return; 
        RemoveLija(other.gameObject.transform);
        Stayed = false;
    }

}
