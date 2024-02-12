using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class DoorHandleInteraction : UdonSharpBehaviour
{
    [SerializeField] VRC_Pickup _Pickup;
    //[SerializeField] Transform[] NonvrDoorPosition;
    [SerializeField] Vector3[] DoorRotation;
    [SerializeField] Transform Door;
    [SerializeField] GameObject GrabbableHandle;
    [SerializeField] Transform HandleDefaultPosition;
    bool IsDoorOpened;
    bool VRUserHold;

    private void Awake()
    {
        //_Pickup = (VRC_Pickup)GetComponent(typeof(VRC_Pickup));
        //GrabbableHandle = gameObject;
    }

    private void Update()
    {
        if (VRUserHold)
        {
            Vector3 dir = new Vector3(gameObject.transform.position.x, Door.transform.position.y, gameObject.gameObject.transform.position.z);
            Door.transform.LookAt(dir);
        }
    }

    public override void OnPickup()
    {
        //Si es VR, entonces nada
        //si no es VR, entonces mueves el objeto a la nueva posicion
        VRUserHold = _Pickup.currentPlayer.IsUserInVR();
        //VRUserHold = true;
        if (VRUserHold)
        {

        }
        else
        {
            IsDoorOpened = !IsDoorOpened;
            Door.localRotation = Quaternion.Euler(DoorRotation[IsDoorOpened ? 1 : 0]);
            //Debug.Log(Door.localRotation.eulerAngles);
            //Door.LookAt(-NonvrDoorPosition[IsDoorOpened ? 1 : 0].position);
        }
    }

    public override void OnDrop()
    {
        //Si es VR, entonces calcular si esta mas cerca de abrir o cerrar
        //En cualquier caso, mueves el handle nuevamente a su posicion
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
        float IsClosedLenght = Vector4.Distance(LocalRotation, DoorRotation0);//Vector3.Distance(HandleDefaultPosition.position, NonvrDoorPosition[0].position);
        float IsOpenedLenght = Vector4.Distance(LocalRotation, DoorRotation1);//Vector3.Distance(HandleDefaultPosition.position, NonvrDoorPosition[1].position);
        //Debug.Log("LocalRot:" + Door.transform.localRotation.eulerAngles);
        //Debug.Log("Rot" + Door.transform.rotation.eulerAngles);
        //Debug.Log("DoorRot0: " + DoorRotation[0]);
        //Debug.Log("DoorRot1: " + DoorRotation[1]);
        //Debug.Log("RotQuat: " + Door.transform.rotation);
        //Debug.Log("DoorRot0Quat: " + Quaternion.Euler(DoorRotation[0]));
        //Debug.Log("DoorRot1Quat: " + Quaternion.Euler(DoorRotation[1]));

        if (IsClosedLenght < IsOpenedLenght)
        {
            IsDoorOpened = false;
            //Debug.Log("Closed");
        }
        else
        {
            IsDoorOpened = true;
            //Debug.Log("Opened");
        }
        Debug.Log("IsClossed: " + IsClosedLenght + ", IsOppened: " + IsOpenedLenght + ", EstaAbierto: " + IsDoorOpened);
        //if (!VRUserHold)
        //{        
        //    IsDoorOpened = !IsDoorOpened;
        //}
        VRUserHold = false;
        GrabbableHandle.transform.position = HandleDefaultPosition.transform.position;
    }

}
