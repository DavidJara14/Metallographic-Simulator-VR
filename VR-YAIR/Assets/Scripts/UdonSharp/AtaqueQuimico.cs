using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

public class AtaqueQuimico : UdonSharpBehaviour
{

    [Header("References")]
    [SerializeField] private FaceBehaviour face;
    [SerializeField] private ProbeBehabiour probeBehaviour;

    [Header("Nital variables")]

    public bool haveNital = false;
    public bool nitalRemoved = false;
    public bool finishedAQ = false;

    private float timer = 0f;

    private void Start()
    {
        face = GetComponent<FaceBehaviour>();
        probeBehaviour = GetComponentInParent<ProbeBehabiour>();
    }


    public void ProcessAtaque()
    {
        if (face.nitalInProbePS == null)
            return;

        var mainNitalPS = face.nitalInProbePS.main;

        if (!face.nitalInProbePS.isEmitting && haveNital)
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All,"resetHaveAlcohol");
            face.nitalInProbePS.Play();
            Debug.LogWarning("[<color=blue>Nital PS play</color>]");
        }


        if (face.limpieza.haveAlcohol && face.nitalInProbePS.isEmitting)
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All,"nitalRemovedTrue");
            mainNitalPS.startSize = new ParticleSystem.MinMaxCurve(0f, 0.005f);
            Debug.LogWarning("[<color=blue>Nital removed</color>]");
        }


        if (face.newcalor && nitalRemoved)
        {
            if (face.pulido.generalTimer(1.5f))
            {
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All,"finishedAtaqueQ");
                face.nitalInProbePS.Stop();

                mainNitalPS.startSize = new ParticleSystem.MinMaxCurve(0f, 0.02f);
                gameObject.GetComponentInParent<BorderColor>().SendCustomEvent("colorGreen");

                Debug.LogWarning("[<color=blue>Ataque Quimico Finalizado</color>]");
            }
        }
    }



    // SCNE
    public void addedNital()
    {
        haveNital = true;
    }
    public void nitalRemovedTrue()
    {
        nitalRemoved = true;
    }
    public void finishedAtaqueQ()
    {
        finishedAQ = true;
        haveNital = false;
    }
    /////////////////
}