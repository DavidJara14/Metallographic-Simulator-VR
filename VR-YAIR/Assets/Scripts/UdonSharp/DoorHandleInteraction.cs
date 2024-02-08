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
        //if (VRUserHold)
        //{
            Vector3 dir = new Vector3(gameObject.transform.position.x, Door.transform.position.y, gameObject.gameObject.transform.position.z);
            Door.transform.LookAt(dir);
        //}
    }

    public override void OnPickup()
    {
        //Si es VR, entonces nada
        //si no es VR, entonces mueves el objeto a la nueva posicion
        VRUserHold = _Pickup.currentPlayer.IsUserInVR();
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
        float IsClosedLenght = Vector3.Distance(Door.transform.localRotation.eulerAngles, DoorRotation[0]);//Vector3.Distance(HandleDefaultPosition.position, NonvrDoorPosition[0].position);
        //Debug.Log(Door.transform.localRotation.eulerAngles);
        //Debug.Log(DoorRotation[0]);
        float IsOpenedLenght = Vector3.Distance(Door.transform.localRotation.eulerAngles, DoorRotation[1]);//Vector3.Distance(HandleDefaultPosition.position, NonvrDoorPosition[1].position);
        //Debug.Log(Door.transform.localRotation.eulerAngles);
        //Debug.Log(DoorRotation[1]);
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
        //Debug.Log("IsClossed: " + IsClosedLenght + ", IsOppened: " + IsOpenedLenght);
        //if (!VRUserHold)
        //{        
        //    IsDoorOpened = !IsDoorOpened;
        //}
        VRUserHold = false;
        GrabbableHandle.transform.position = HandleDefaultPosition.transform.position;
    }

}
