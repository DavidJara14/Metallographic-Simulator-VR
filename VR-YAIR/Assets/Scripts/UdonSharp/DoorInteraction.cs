
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class DoorInteraction : UdonSharpBehaviour
{
    [SerializeField] VRC_Pickup _Pickup;
    [SerializeField] Rigidbody _RigidbodyDoor;
    [SerializeField] GameObject LookatObj;
    [SerializeField] GameObject ObjectToLookAt;
    [SerializeField] GameObject Manija;
    [SerializeField] float AddedForce;
    bool Holds = false;
    public override void OnPickup()
    {
        if(_Pickup.currentPlayer.IsUserInVR())
        {
            Holds = true;
        }
        else
        {
            _RigidbodyDoor.AddForce((gameObject.transform.position - _Pickup.currentPlayer.GetPosition()).normalized * AddedForce);
        }
    }

    private void Update()
    {
        if(Holds)
        {
            Vector3 dir = new Vector3(gameObject.transform.position.x, LookatObj.transform.position.y, gameObject.gameObject.transform.position.z);
            LookatObj.transform.LookAt(dir);
        }
        else
        {
            gameObject.transform.position = Manija.transform.position;
        }
    }

    public override void OnDrop()
    {
        Holds = false;
    }
}
