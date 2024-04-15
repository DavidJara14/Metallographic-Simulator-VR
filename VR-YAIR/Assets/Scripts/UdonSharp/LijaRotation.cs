using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class LijaRotation : UdonSharpBehaviour
{

    [SerializeField] bool LijaLoaded;
    [SerializeField] bool CanRotate;

    [SerializeField] bool Stayed;

    [SerializeField] float RotationVelocity = 1f;

    private void Update()
    {
        if(CanRotate)
        {
            gameObject.transform.Rotate(Vector3.forward * RotationVelocity * Time.deltaTime);
        }
    }

    public void OnLijaSnap(Transform go)
    {
        LijaLoaded = true;
        go.SetParent(gameObject.transform);
    }

    public void RemoveLija(Transform go)
    {
        LijaLoaded = false;
        go.parent = null;
    }

    public void StartMachine()
    {
        if (LijaLoaded)
        {
            CanRotate = !CanRotate;
        }
        else
        {
            CanRotate = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log(other.gameObject.GetComponent<VRC_Pickup>().currentPlayer);
        if(!other.GetComponent<LijaCircularBehabiour>()) { return; }
        if(other.gameObject.GetComponent<VRC_Pickup>().currentPlayer != null) { return; }
        if(!Stayed)
        {
            OnLijaSnap(other.gameObject.transform);
            Stayed = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.GetComponent<LijaCircularBehabiour>()) { return; }
        RemoveLija(other.gameObject.transform);
        Stayed = false;
    }

}
