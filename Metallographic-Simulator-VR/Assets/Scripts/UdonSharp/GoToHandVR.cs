using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class GoToHandVR : UdonSharpBehaviour
{
    [Header("References")]
    [SerializeField] Transform LHandHandle;
    [SerializeField] Transform RHandHandle;
    [SerializeField] bool[] LFingers = new bool[3];
    [SerializeField] bool[] RFingers = new bool[3];
    bool NoFingers = true;
    VRCPlayerApi LocalPlayerInstance = null;

    void Start()
    {
        if (!Networking.LocalPlayer.IsUserInVR())
        {
            LHandHandle.gameObject.SetActive(false);
            RHandHandle.gameObject.SetActive(false);
            enabled = false;
            return;
        }
        CheckForFingers();
    }

    private void Update()
    {
        if (!Networking.LocalPlayer.IsValid() || LocalPlayerInstance == null)
        {
            CheckForFingers();
            return;
        }
        if (NoFingers)
        {
            LHandHandle.transform.position = LocalPlayerInstance.GetTrackingData(VRCPlayerApi.TrackingDataType.LeftHand).position;
            RHandHandle.transform.position = LocalPlayerInstance.GetTrackingData(VRCPlayerApi.TrackingDataType.RightHand).position;
            return;
        }

        UpdateLFingers();
        UpdateRFingers();

        //if(Networking.LocalPlayer.IsValid() && LocalPlayerInstance != null)
        //{
        //    if (LFingers[0])
        //    {
        //        LHandHandle.transform.position = LocalPlayerInstance.GetBoneTransform
        //    }
        //    LHandHandle.transform.position = LocalPlayerInstance.GetTrackingData(VRCPlayerApi.TrackingDataType.LeftHand).position;
        //    RHandHandle.transform.position = LocalPlayerInstance.GetTrackingData(VRCPlayerApi.TrackingDataType.RightHand).position;
        //    LocalPlayerInstance.GetBonePosition(HumanBodyBones.LeftIndexDistal);
        //}
        //else
        //{
        //    CheckForFingers();
        //}
    }

    private void UpdateRFingers()
    {
        if (RFingers[0] == true)
        {
            RHandHandle.transform.position = LocalPlayerInstance.GetBonePosition(HumanBodyBones.RightIndexDistal);
            NoFingers = false;
        }
        if (RFingers[1] == true)
        {
            RHandHandle.transform.position = LocalPlayerInstance.GetBonePosition(HumanBodyBones.RightIndexIntermediate);
            NoFingers = false;
        }
        if (RFingers[2] == true)
        {
            RHandHandle.transform.position = LocalPlayerInstance.GetBonePosition(HumanBodyBones.RightIndexProximal);
            NoFingers = false;
        }
    }

    private void UpdateLFingers()
    {
        if (LFingers[0] == true)
        {
            LHandHandle.transform.position = LocalPlayerInstance.GetBonePosition(HumanBodyBones.LeftIndexDistal);
            NoFingers = false;
        }
        if (LFingers[1] == true)
        {
            LHandHandle.transform.position = LocalPlayerInstance.GetBonePosition(HumanBodyBones.LeftIndexIntermediate);
            NoFingers = false;
        }
        if (LFingers[2] == true)
        {
            LHandHandle.transform.position = LocalPlayerInstance.GetBonePosition(HumanBodyBones.LeftIndexProximal);
            NoFingers = false;
        }
    }

    private void CheckForFingers()
    {
        LocalPlayerInstance = Networking.LocalPlayer;
        Debug.Log(LocalPlayerInstance);
        Debug.Log(LocalPlayerInstance.displayName);
        if (LocalPlayerInstance != null)
        {
            LFingers[0] = LocalPlayerInstance.GetBonePosition(HumanBodyBones.LeftIndexDistal) != Vector3.zero;
            LFingers[1] = LocalPlayerInstance.GetBonePosition(HumanBodyBones.LeftIndexIntermediate) != Vector3.zero;
            LFingers[2] = LocalPlayerInstance.GetBonePosition(HumanBodyBones.LeftIndexProximal) != Vector3.zero;
            RFingers[0] = LocalPlayerInstance.GetBonePosition(HumanBodyBones.RightIndexDistal) != Vector3.zero;
            RFingers[1] = LocalPlayerInstance.GetBonePosition(HumanBodyBones.RightIndexIntermediate) != Vector3.zero;
            RFingers[2] = LocalPlayerInstance.GetBonePosition(HumanBodyBones.RightIndexProximal) != Vector3.zero;
        }
    }
}