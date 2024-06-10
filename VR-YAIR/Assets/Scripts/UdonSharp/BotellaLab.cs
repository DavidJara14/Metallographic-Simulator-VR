using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class BotellaLab : UdonSharpBehaviour
{

    private const float MaxLiquidFill = 250f;
    public bool isInfinite = false;
    public string Tipo = "";
    [SerializeField][Range(0f, MaxLiquidFill)] private float LiquidFill;
    [SerializeField] private float LiquidPourVel;

    [SerializeField] private GameObject InfillGO;
    [SerializeField] private Material WaterMaterial;
    [SerializeField] private VRC_Pickup _pickupComp;
    [SerializeField] private MeshRenderer m_Renderer;
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private Transform _Visual;
    bool LastUserWasVr = false;

    private void Start()
    {
        WaterMaterial = InfillGO.GetComponent<MeshRenderer>().material;
        var main = _particleSystem.main;
        main.startColor = WaterMaterial.GetColor("_ColorAguaSuperficie");
    }

    private void Update()
    {
        WaterMaterial.SetFloat("_FillPercentage", (Mathf.Clamp(LiquidFill, 0f, MaxLiquidFill)/ MaxLiquidFill)*100f);
        if(_particleSystem.isEmitting)
        {
            if(!isInfinite)
            {
                LiquidFill -= Time.deltaTime * LiquidPourVel;
            }
        }
        if(LiquidFill < 0 )
        {
            _particleSystem.Stop();
            if (!LastUserWasVr)
            {
                _Visual.transform.localRotation = Quaternion.Euler(0, 0, 00);
            }
        }
    }

    public override void OnPickup()
    {
        //LastUserWasVr |= _pickupComp != null;
        LastUserWasVr = _pickupComp.currentPlayer.IsUserInVR();
        if (!LastUserWasVr)
        {
            _Visual.localRotation = Quaternion.Euler(0f, 180f, 0f);
        }
    }

    public override void OnDrop()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "UnUseThisThing");
        if (!LastUserWasVr)
        {
            gameObject.transform.rotation = Quaternion.Euler(0f, gameObject.transform.rotation.eulerAngles.y, 0f);
            //_Visual.rotation = Quaternion.identity; 
            //gameObject.transform.rotation = Quaternion.Euler(0, 50f, 0);
            _Visual.localRotation = Quaternion.Euler(0f, 180f, 0f);
        }
    }

    public override void OnPickupUseDown()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "UseThisThing");
    }

    public override void OnPickupUseUp()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "UnUseThisThing");
    }

    public void UseThisThing()
    {
        var Mainn = _particleSystem.main;
        Mainn.startColor = WaterMaterial.GetColor("_ColorAgua");
        if (_pickupComp.currentPlayer.IsUserInVR())
        {
            if (LiquidFill > 0)
            {
                _particleSystem.gameObject.SetActive(true);
                _particleSystem.Play();
            }
        }
        else
        {
            if(LiquidFill > 0)
            {
                _Visual.transform.localRotation = Quaternion.Euler(-15f, _Visual.transform.localRotation.eulerAngles.y, _Visual.transform.localRotation.eulerAngles.z);
                _particleSystem.gameObject.SetActive(true);
                _particleSystem.Play();
            }
        }
    }

    public void UnUseThisThing()
    {
        _particleSystem.Stop();
        if (!LastUserWasVr)
        {
            _Visual.transform.localRotation = Quaternion.Euler(0, _Visual.transform.localRotation.eulerAngles.y, _Visual.transform.localRotation.eulerAngles.z);
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.GetComponentInParent<IsLiquidSource>() != null)
        {
            fill();
        }
    }

    private void fill()
    {
        LiquidFill += 5;
        if(LiquidFill > MaxLiquidFill)
            LiquidFill = MaxLiquidFill;
    }

}