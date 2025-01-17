using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDK3.Data;
using VRC.SDKBase;
using VRC.Udon;

public class PistolaDeCalor : UdonSharpBehaviour
{
    //public GameObject TriggerGO;
    public CapsuleCollider collisionPistol;
    [SerializeField] private bool Used;
    [SerializeField] private Animator _animator;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private VRCPickup _pickup;
    private DataDictionary ActivePositionZ = new DataDictionary()
    {
        {false, new DataList(){ -0.2f, 0.01f} },
        {true, new DataList(){0.05f , 0.05f} },
    };

    [Header("Audio Config")]
    [SerializeField] private AudioSource _StartStopAudioSource;
    [SerializeField] private AudioSource _LoopAudioSource;
    [SerializeField] private AudioClip _StartAudioClip;
    [SerializeField] private AudioClip _LoopAudioClip;
    [SerializeField] private AudioClip _StopAudioClip;
    [SerializeField] private bool AudioStart;
    [SerializeField] private float StartTimer;
    [SerializeField] private bool AudioLoop;

    private void Start()
    {
        ActivePositionZ.TryGetValue(Used, out DataToken var);
        collisionPistol.center = new Vector3(0, 0, var.DataList[0].Float);
        collisionPistol.radius = var.DataList[1].Float;
        _pickup = gameObject.GetComponent<VRCPickup>();
    }

    private void Update()
    {
        AudioSystem();
    }

    private void AudioSystem()
    {
        if (Used & !AudioStart && !AudioLoop)
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

        if (!Used && (AudioStart || AudioLoop))
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
    }

    public override void OnPickupUseDown()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "usePistol");
    }

    public void usePistol()
    {
        Used = !Used;
        ActivePositionZ.TryGetValue(Used, out DataToken var);
        collisionPistol.center = new Vector3 (0, 0, var.DataList[0].Float);
        collisionPistol.radius = var.DataList[1].Float;
        //collisionPistol.enabled = Used;
        if (Used)
        {
            _particleSystem.Play();
        }
        else
        {
            _particleSystem.Stop();
        }
        //TriggerGO.SetActive(Used);
        _animator.SetBool("Used", Used);
        //if(Used)
        //{
        //    _audioSource.Play();
        //}
        //else
        //{
        //    _audioSource.Stop();
        //}
    }

    //OnTriggerEnter para una bandera de entrada y on trigger exit para una de salida, y un funcion de timer en el objeto original

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Heatable>() != null)
        {
            Debug.Log("Heatable element found, trying send message");
            if(_pickup.currentPlayer == null) //esta suelto
            {
                other.gameObject.GetComponent<Heatable>().SendCustomEvent("ActivateCalor");
                Debug.Log("SCE");
            }
            else if(Networking.IsOwner(Networking.LocalPlayer, this.gameObject)) //el trigger lo activa el owner del objeto
            {
                Debug.Log("SCNE");
                other.gameObject.GetComponent<Heatable>().SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ActivateCalor");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Heatable>() != null)
        {
            if (_pickup.currentPlayer == null) //esta suelto
            {
                other.gameObject.GetComponent<Heatable>().SendCustomEvent("DeactivateCalor");
            }
            else if (Networking.IsOwner(Networking.LocalPlayer, this.gameObject)) //el trigger lo activa el owner del objeto
            {
                other.gameObject.GetComponent<Heatable>().SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "DeactivateCalor");
            }
        }
    }

}
