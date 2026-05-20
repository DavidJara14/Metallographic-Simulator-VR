using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class DirSelectorScript : UdonSharpBehaviour
{
    [SerializeField] private VRC_Pickup _pickupComp;
    [SerializeField] private Transform GripDirVr;
    [SerializeField] private Transform GripDirPC;

    private void Start()
    {        
        if(Networking.LocalPlayer.IsUserInVR())
        {
            _pickupComp.orientation = VRC_Pickup.PickupOrientation.Any;
            Debug.Log("IsVrUser");
            //_pickupComp.ExactGrip = GripDirVr;
        }
        else
        {
            Debug.Log("IsPcUser");
            _pickupComp.ExactGrip = GripDirPC;
        }
    }
}
