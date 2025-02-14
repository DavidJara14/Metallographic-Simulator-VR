
using Cinemachine.Utility;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class EjectObject : UdonSharpBehaviour
{

    [SerializeField] private GameObject objectEjectable;
    [SerializeField] private VRC_Pickup objectPickup;


    [SerializeField] private Vector3 RotorToObjSize;
    [SerializeField] private Vector3 Up;
    [SerializeField] private Vector3 VectorDeDireccion;

    private void Update()
    {
        if (gameObject.GetComponent<LijaRotation>() != null)
        {
            if (gameObject.GetComponent<LijaRotation>().Rotating)
            {
                ejectObject();
            }
        }

        if (gameObject.GetComponent<PulidoraScript>() != null)
        {
            if (gameObject.GetComponent<PulidoraScript>().Rotating)
            {
                ejectObject();
            }
        }
    }

    private void ejectObject()
    {
        if (objectEjectable != null)
        {
            RotorToObjSize = new Vector3(objectEjectable.transform.position.x - gameObject.transform.position.x, 0f, objectEjectable.transform.position.z - gameObject.transform.position.z);
            Up = objectEjectable.transform.up;
            VectorDeDireccion = Vector3.Cross(RotorToObjSize, Up);
            Debug.Log("Vector asigned");

            if(objectPickup.currentPlayer == null)
            {
                VectorDeDireccion.y = Mathf.Abs(VectorDeDireccion.y); 
                objectEjectable.GetComponent<Rigidbody>().AddForce(VectorDeDireccion.normalized * 200f);
                Debug.Log("Object Ejected");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Ejectable>() != null && Networking.IsOwner(Networking.LocalPlayer, other.gameObject))
        {
            objectEjectable = other.gameObject;
            objectPickup = other.GetComponent<VRC_Pickup>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Ejectable>() != null && Networking.IsOwner(Networking.LocalPlayer, other.gameObject))
        {
            objectEjectable = null;
            objectPickup = null;
        }
    }
}
