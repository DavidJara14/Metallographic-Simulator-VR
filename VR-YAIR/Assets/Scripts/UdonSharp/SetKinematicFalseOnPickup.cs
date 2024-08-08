
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

public class SetKinematicFalseOnPickup : UdonSharpBehaviour
{

    [SerializeField] private bool HasObjSync;
    [SerializeField] private VRCObjectSync _ObjSync;
    [SerializeField] private Rigidbody _rigidbody;
    [UdonSynced] private bool WasPicked;


    void Start()
    {
        if (HasObjSync)
        {
            if(_ObjSync == null )
            {
                _ObjSync = GetComponent<VRCObjectSync>();
            }
            _ObjSync.SetKinematic(true);
        }
        else
        {
            if (_rigidbody == null)
                _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.isKinematic = true;
        }
        Debug.Log($"RB set to {_rigidbody.isKinematic}, expected true");
    }


    public override void OnPickup()
    {
        if (_ObjSync == null)
        {
            _ObjSync = GetComponent<VRCObjectSync>();
        }
        _ObjSync.SetKinematic(false);
        if (_rigidbody == null)
            _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.isKinematic = false;
        WasPicked = true;
        Debug.Log($"WasPicked set to {WasPicked}, expected true");
        Debug.Log($"RB set to {_rigidbody.isKinematic}, expected false");
        Debug.Log($"Deactivating");
        this.enabled = false;
    }

    public override void OnDeserialization()
    {
        if (WasPicked)
        {
            if (_ObjSync == null)
            {
                _ObjSync = GetComponent<VRCObjectSync>();
            }
            _ObjSync.SetKinematic(false);
            if (_rigidbody == null)
                _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.isKinematic = false;
            Debug.Log($"WasPicked set to {WasPicked}, expected true to work");
            Debug.Log($"enable set to {_rigidbody.isKinematic}, expected false");
            Debug.Log($"Deactivating");
            enabled = false;
        }
    }

}
