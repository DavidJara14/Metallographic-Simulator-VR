using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ButtonInteraction3D : UdonSharpBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody _RB;
    [SerializeField] ConfigurableJoint _Joint;
    [SerializeField] UdonBehaviour UdonEventBehabiour;
    [Header("Configuration")]
    [SerializeField] float deadZone;
    [Tooltip("Global, not local")][SerializeField] private Vector3 ForceVector;
    [SerializeField] private string OnPressedEventName;
    private bool Pushed;
    private bool FirstActivation;
    Vector3 _StartPos;


    void Start()
    {
        _StartPos = transform.localPosition;
        //PLAYER
    }

    private void Update()
    {
        var PressValue = Vector3.Distance(_StartPos, transform.localPosition) / _Joint.linearLimit.limit;
        if (Mathf.Abs(PressValue) < deadZone)
        {
            PressValue = 0;
        }
        PressValue = Mathf.Clamp(PressValue, -1, 1);
        Pushed = PressValue < 0.3;
        if(Pushed && FirstActivation)
        {
            FirstActivation = false;
            Pressed();
        }
        FirstActivation = PressValue >= 0.3;
    }

    public override void Interact()
    {
        _RB.AddForce(ForceVector);
    }

    private void Pressed()
    {
        if(UdonEventBehabiour == null)
        {
            Debug.Log("No udon behabiour specified, using own script");
            UdonEventBehabiour = (UdonBehaviour)GetComponent(typeof(UdonBehaviour));
        }
        UdonEventBehabiour.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, OnPressedEventName);
    }
}
