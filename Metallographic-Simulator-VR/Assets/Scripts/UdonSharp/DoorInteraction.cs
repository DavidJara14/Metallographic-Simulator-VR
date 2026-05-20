
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class DoorInteraction : UdonSharpBehaviour
{
    [SerializeField] VRC_Pickup _Pickup;
    [SerializeField] Rigidbody _RigidbodyDoor;
    [SerializeField] GameObject Door;
    [SerializeField] GameObject ObjectToLookAt;
    [SerializeField] GameObject Manija;
    [SerializeField] GameObject[] NonVrPositions;
    [SerializeField] bool isOpened = false;
    bool updateDoorState;
    [SerializeField] float AddedForce;
    bool VRHolds = false;
    public override void OnPickup()
    {
        if(_Pickup.currentPlayer.IsUserInVR())
        {
            VRHolds = true;
        }
        else
        {
            isOpened = !isOpened;
            updateDoorState = true;
            //_RigidbodyDoor.AddForce((gameObject.transform.position - _Pickup.currentPlayer.GetPosition()).normalized * AddedForce);
        }
    }

    private void Update()
    {
        if(updateDoorState)
        {
            if (isOpened)
            {
                Vector3 dir = new Vector3(NonVrPositions[0].transform.position.x, Door.transform.position.y, NonVrPositions[0].gameObject.transform.position.z);
                Door.transform.LookAt(dir);
            }
            else
            {
                Vector3 dir = new Vector3(NonVrPositions[1].transform.position.x, Door.transform.position.y, NonVrPositions[1].gameObject.transform.position.z);
                Door.transform.LookAt(dir);
            }
            updateDoorState = false;
        }
        if (VRHolds)
        {
            Vector3 dir = new Vector3(gameObject.transform.position.x, Door.transform.position.y, gameObject.gameObject.transform.position.z);
            Door.transform.LookAt(dir);
        }
        else
        {
            gameObject.transform.position = Manija.transform.position;
        }
    }

    public override void OnDrop()
    {
        VRHolds = false;
    }
}
