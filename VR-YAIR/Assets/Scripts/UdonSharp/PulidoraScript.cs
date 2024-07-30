
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class PulidoraScript : UdonSharpBehaviour
{
    [SerializeField] public bool Rotating;

    [SerializeField] float RotationVelocity = 1f;

    [SerializeField] const float MaxTemp = 100f;
    [SerializeField] float Temperature = 24f;

    [SerializeField] private TextMeshProUGUI RPMText;

    [SerializeField] public bool isEnergized = false;

    [Header("Audio Config")]
    [SerializeField] private AudioSource _StartStopAudioSource;
    [SerializeField] private AudioSource _LoopAudioSource;
    [SerializeField] private AudioClip _StartAudioClip;
    [SerializeField] private AudioClip _LoopAudioClip;
    [SerializeField] private AudioClip _StopAudioClip;
    [SerializeField] private bool AudioStart;
    [SerializeField] private float StartTimer;
    [SerializeField] private bool AudioLoop;

    public bool GrisLoaded = false;
    public bool BlancaLoaded = false;
    public GameObject colliderGris;
    public GameObject colliderBlanco;

    private void Update()
    {
        if (Rotating && isEnergized)
        {
            gameObject.transform.Rotate(Vector3.forward * RotationVelocity * Time.deltaTime);
            colliderGris.SetActive(GrisLoaded);
            colliderBlanco.SetActive(BlancaLoaded);
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

        if (!Rotating || !isEnergized)
        {
            colliderGris.SetActive(false);
            colliderBlanco.SetActive(false);
        }
    }

    public void StartMachine()
    {
        Rotating = true;
        gameObject.GetComponent<BoxCollider>().isTrigger = true;
    }

    public void StopMachine()
    {
         Rotating = false;
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
        RPMText.text = "8888";
        RPMText.color = Color.gray;
    }

    private void OnParticleCollision(GameObject other)
    {
        GrisLoaded = false;
        BlancaLoaded = false;


        string tipo = other.GetComponentInParent<BotellaLab>().Tipo;
        //Debug.Log(tipo);

        if (tipo == "AGris")
        {
            GrisLoaded = true;
            Debug.Log("Alumina Gris");
        }
        else if (tipo == "ABlanca")
        {
            BlancaLoaded = true;
            Debug.Log("Alumina Blanca");
        }
    }
}
