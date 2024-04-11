using BestHTTP.SecureProtocol.Org.BouncyCastle.Asn1.BC;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class LijaRotation : UdonSharpBehaviour
{

    [SerializeField] bool LijaLoaded;
    [SerializeField] bool CanRotate;

    [SerializeField] float RotationVelocity = 1f;

    private void Update()
    {
        if(CanRotate)
        {
            gameObject.transform.Rotate(Vector3.forward * RotationVelocity * Time.deltaTime);
        }
    }

    public void OnLijaSnap()
    {
        LijaLoaded = true;
    }

    public void RemoveLija()
    {
        LijaLoaded = false;
    }


    public void StartMachine()
    {
        if (LijaLoaded)
        {
            CanRotate = !CanRotate;
        }
        else
        {
            CanRotate = false;
        }
    }

}
