using UdonSharp;
using Unity.Mathematics;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class DoorHandleInteraction : UdonSharpBehaviour
{
    [SerializeField] VRC_Pickup _Pickup;
    [SerializeField] Vector3[] DoorRotation;
    [SerializeField] Transform Door;
    [SerializeField] GameObject GrabbableHandle;
    [SerializeField] Transform HandleDefaultPosition;
    bool IsDoorOpened;
    bool VRUserHold;
    bool IsPickedUp;


    private void Update()
    {
        if (VRUserHold)
        {
            Vector3 dir = new Vector3(gameObject.transform.position.x, Door.transform.position.y, gameObject.gameObject.transform.position.z);
            Door.transform.LookAt(dir);
        }

        if(IsPickedUp && Networking.IsOwner(Networking.LocalPlayer, Door.gameObject))
        {
            Door.localRotation = Quaternion.Euler(DoorRotation[IsDoorOpened ? 1 : 0]);
            IsPickedUp = false;
        }
    }

    public override void OnPickup()
    {
        //Si es VR, entonces nada
        //si no es VR, entonces mueves el objeto a la nueva posicion
        VRUserHold = _Pickup.currentPlayer.IsUserInVR();
        Networking.SetOwner(Networking.LocalPlayer, Door.gameObject);
        IsPickedUp = true;
        if (VRUserHold)
        {

        }
        else
        {
            IsDoorOpened = !IsDoorOpened;
        }
    }

    public override void OnDrop()
    {
        //Si es VR, entonces calcular si esta mas cerca de abrir o cerrar
        //En cualquier caso, mueves el handle nuevamente a su posicion
        IsPickedUp = false;
        Vector4 LocalRotation = new Vector4(Door.transform.localRotation.x,Door.transform.localRotation.y,Door.transform.localRotation.z,Door.transform.localRotation.w);
        Quaternion DoorRotation0Q = Quaternion.Euler(DoorRotation[0]);
        Vector4 DoorRotation0 = new Vector4(
            DoorRotation0Q.x,
            DoorRotation0Q.y,
            DoorRotation0Q.z,
            DoorRotation0Q.w);
        Quaternion DoorRotation1Q = Quaternion.Euler(DoorRotation[1]);
        Vector4 DoorRotation1 = new Vector4(
            DoorRotation1Q.x,
            DoorRotation1Q.y,
            DoorRotation1Q.z,
            DoorRotation1Q.w);
        float IsClosedLenght = Vector4.Distance(LocalRotation, DoorRotation0);
        float IsOpenedLenght = Vector4.Distance(LocalRotation, DoorRotation1);


        if (IsClosedLenght < IsOpenedLenght)
        {
            IsDoorOpened = false;
        }
        else
        {
            IsDoorOpened = true; 
        }
        Debug.Log("IsClossed: " + IsClosedLenght + ", IsOppened: " + IsOpenedLenght + ", EstaAbierto: " + IsDoorOpened);
        VRUserHold = false;
        GrabbableHandle.transform.position = HandleDefaultPosition.transform.position;
    }

}
