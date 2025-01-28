using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class LijaRotation : UdonSharpBehaviour
{

    public bool LijaLoaded;
    public bool Rotating;

    [SerializeField] bool Stayed;

    [SerializeField] float RotationVelocity = 1f;

    [SerializeField] const float MaxTemp = 100f;
    [SerializeField] float Temperature = 24f;

    [SerializeField] public GameObject LijaGO = null;
    [SerializeField] public VRC_Pickup Lija = null;
    [SerializeField] private TextMeshProUGUI RPMText;

    [UdonSynced][SerializeField] public bool isEnergized = false;

    [Header("Audio Config")]
    [SerializeField] private AudioSource _StartStopAudioSource;
    [SerializeField] private AudioSource _LoopAudioSource;
    [SerializeField] private AudioClip _StartAudioClip;
    [SerializeField] private AudioClip _LoopAudioClip;
    [SerializeField] private AudioClip _StopAudioClip;
    [SerializeField] private bool AudioStart;
    [SerializeField] private float StartTimer;
    [SerializeField] private bool AudioLoop;

    public CubreLijaSnap thisCubreLijaSnap;

    private void Update()
    {
        if (Rotating && isEnergized)
        {
            gameObject.transform.Rotate(Vector3.forward * RotationVelocity * Time.deltaTime);
        }
        if (Lija == null)
        {
            if (LijaGO != null)
            {
                Lija = LijaGO.GetComponent<VRC_Pickup>();
            }
        }

        if (Rotating & !AudioStart && !AudioLoop)
        {
            AudioStart = true;
            _StartStopAudioSource.Stop();
            _StartStopAudioSource.clip = _StartAudioClip;
            _StartStopAudioSource.Play();
            StartTimer = 0f;
        }

        if (StartTimer > 0.9 * _StartAudioClip.length && !AudioLoop)
        {
            AudioStart = false;
            AudioLoop = true;
            _StartStopAudioSource.Stop();
            _LoopAudioSource.clip = _LoopAudioClip;
            _LoopAudioSource.Play();
        }

        if (!Rotating && (AudioStart || AudioLoop))
        {
            AudioStart = false;
            AudioLoop = false;
            StartTimer = 0f;
            _LoopAudioSource.Stop();
            _StartStopAudioSource.Stop();
            _StartStopAudioSource.clip = _StopAudioClip;
            _StartStopAudioSource.Play();
        }

        if (AudioStart)
            StartTimer += Time.deltaTime;

        if(Lija != null)
        {
            Lija.pickupable = !thisCubreLijaSnap.CubreLijaLoaded;

            if (LijaLoaded)
            {
                Lija.GetComponent<BoxCollider>().excludeLayers = LayerMask.GetMask("Pickup");
            }
        }

    }

    public void OnLijaSnap(Transform go)
    {
        LijaLoaded = true;
        go.SetParent(gameObject.transform);
        LijaGO = go.gameObject;
        Lija = LijaGO.GetComponent<VRC_Pickup>();
        Debug.Log("<color=green>lija snapped on LijaRotation</color>");
    }

    public void RemoveLija(Transform go)
    {
        LijaLoaded = false;
        go.GetComponent<VRC_Pickup>().pickupable = true;
        go.parent = null;
        LijaGO = null;
        Lija = null;
        Debug.Log("<color=#ff9900>Removed lija</color>");
    }

    public void StartMachine()
    {
        if (thisCubreLijaSnap.CubreLijaLoaded)
        {
            if (LijaLoaded && isEnergized)
            {
                Rotating = true;
            }
        }
    }

    public void StopMachine()
    {
        if (Rotating)
        {
            Rotating = false;
        }
    }

    public void MachineEnergy_On()
    {
        isEnergized = true;
        RPMText.text = "0900";
        RPMText.color = Color.red;
    }

    public void MachineEnergy_Off()
    {
        isEnergized = false;
        if(Rotating) {Rotating = false;}
        if(Lija != null)
            Lija.pickupable = true;
        RPMText.text = "8888";
        RPMText.color = Color.gray;
    }

    private void OnTriggerStay(Collider other)
    {
        if (Lija != null) return; //si ya existe una lija, regresa
        if (!other.GetComponent<LijaCircularBehabiour>()) { return; } //si no es una lija, regresa
        if (other.gameObject.GetComponent<VRC_Pickup>().currentPlayer != null) { return; } //si tiene a alguien agarrandolo, regresa
        if (Stayed) { return; } //si estaba ya dentro del collider, regresa
        if (!thisCubreLijaSnap.GetComponent<CubreLijaSnap>().CubreLijaLoaded) //si no tiene el cubrelija, puedes snapear
        {
            //Debug.Log("es lija");
            OnLijaSnap(other.gameObject.transform);
            Stayed = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.GetComponent<LijaCircularBehabiour>())
            return; 
        RemoveLija(other.gameObject.transform);
        Stayed = false;
    }

}
