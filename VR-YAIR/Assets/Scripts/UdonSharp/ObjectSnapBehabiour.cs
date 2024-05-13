using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.Udon;
public class ObjectSnapBehabiour : UdonSharpBehaviour
{
    [Header("Snap config")]
    [SerializeField] bool SnapPosition;
    [SerializeField] bool SnapRotation;

    [SerializeField] Vector3 PositionOffset;
    [SerializeField] Vector3 RotationOffset;

    [Header("Snap ref")]
    [SerializeField] float DetectionRadius;

    [Header("OnSnap References")]
    [SerializeField] GameObject[] GOListeners;
    [SerializeField] UdonBehaviour[] UdonBehabiourListenersRef;

    [Header("Events configuration")]
    [SerializeField] string[] EventNames;

    [SerializeField] Collider[] GameObjectsNear;

    private void Start()
    {
        DataList list = new DataList();
        for (int i = 0; i < GOListeners.Length; i++)
        {
            foreach (var item in GOListeners[i].GetComponents<UdonBehaviour>())
            {
                list.Add(item);
            }
        }
        UdonBehabiourListenersRef = new UdonBehaviour[list.Count];
        for (int i = 0; i < UdonBehabiourListenersRef.Length; i++)
        {
            UdonBehabiourListenersRef[i] = (UdonBehaviour)list[i].Reference;
        }
    }

    public void SnapFunc()
    {
        GameObject GOToSnap = null;
        GameObjectsNear = Physics.OverlapSphere(transform.position, DetectionRadius);

        float Distance = float.PositiveInfinity;
        var GrabablesNear = "";
        foreach (Collider item in GameObjectsNear)
        {
            if(null == item) continue;
            if(item.gameObject.GetComponent<placeable>() == null) continue;
            GrabablesNear += item.name + " ";
            var dist = Vector3.Distance(transform.position, item.transform.position);
            if (Distance >= dist && item != gameObject)
            {
                Distance = dist;
                GOToSnap = item.gameObject;
            }
        }

        if(GOToSnap == null)
        {
            Debug.LogWarning("No hay objetos cercanos, ¿hace falta asignar 'placeable'?");
            return;
        }


        var GOpos = gameObject.transform.position;
        var GOrot = gameObject.transform.rotation;
        if(SnapPosition)
        {
            GOToSnap.transform.position = GOpos + PositionOffset;
        }
        if(SnapRotation)
        {
            GOToSnap.transform.rotation = Quaternion.Euler(GOrot.eulerAngles + RotationOffset);
        }

        CallEvent();
    }

    private void CallEvent()
    {
        for (int i = 0; i < UdonBehabiourListenersRef.Length; i++)
        {
            for (int j = 0; j < EventNames.Length; j++)
            {
                UdonBehabiourListenersRef[i].SendCustomNetworkEvent(
                VRC.Udon.Common.Interfaces.NetworkEventTarget.All,
                EventNames[j]);
            }
            
        }
    }
}
