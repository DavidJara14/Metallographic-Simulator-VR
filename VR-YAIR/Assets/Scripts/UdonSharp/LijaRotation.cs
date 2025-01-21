using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class LijaRotation : UdonSharpBehaviour
{

    [SerializeField] bool LijaLoaded;
    [UdonSynced][SerializeField] public bool Rotating;
    //[UdonSynced][SerializeField] public bool enableMeshCL;

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

    //public GameObject rotorChildren;
    public GameObject thisCubreLijaSnap;


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

        //if(rotorChildren != null)
//            rotorChildren.gameObject.SetActive(Rotating);
            //rotorChildren.GetComponent<BoxCollider>().enabled = Rotating;
           // rotorChildren.GetComponent<colliderRotorBehabiour>().isRotating = Rotating;
    }

    public void OnLijaSnap(Transform go)
    {
        LijaLoaded = true;
        go.SetParent(gameObject.transform);
        Debug.Log(go);
        LijaGO = go.gameObject;
        Debug.Log(LijaGO);
        Lija = LijaGO.GetComponent<VRC_Pickup>();
        Debug.Log(Lija);
    }

    public void RemoveLija(Transform go)
    {
        LijaLoaded = false;
        Rotating = false;
        go.GetComponent<VRC_Pickup>().pickupable = true;
        go.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        go.parent = null;
        LijaGO = null;
        Lija = null;
    }

    public void StartMachine()
    {
        if (thisCubreLijaSnap.GetComponent<CubreLijaSnap>().CubreLijaLoaded)
        {
            //enableMeshCL = false;
            if (LijaLoaded && isEnergized)
            {
                Rotating = true;
                Lija.pickupable = !Rotating;
                if (Lija.pickupable)
                    Lija.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                else
                    Lija.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
            }
            else
            {
                Rotating = false;
            }
        }
    }

    public void StopMachine()
    {
        //enableMeshCL = true;
        Rotating = false;
        if (LijaLoaded)
        {
            Lija.pickupable = !Rotating;
            if (Lija.pickupable)
                Lija.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            else
                Lija.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
        }
    }

    public void MachineEnergy_On()
    {
        isEnergized = true;
        Rotating = false;
        RPMText.text = "0900";
        RPMText.color = Color.red;
    }

    public void MachineEnergy_Off()
    {
        isEnergized = false;
        Rotating = false;
        if(Lija != null)
            Lija.pickupable = true;
        RPMText.text = "8888";
        RPMText.color = Color.gray;
    }

    private void OnTriggerStay(Collider other)
    {
        if (Lija != null) return;
        if(!other.GetComponent<LijaCircularBehabiour>()) { return; }
        if(other.gameObject.GetComponent<VRC_Pickup>().currentPlayer != null) { return; }
        if(!Stayed)
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
