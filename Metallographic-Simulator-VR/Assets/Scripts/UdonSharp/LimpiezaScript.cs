using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

public class LimpiezaScript : UdonSharpBehaviour
{

    [Header("References")]
    [SerializeField] private FaceBehaviour face;
    [SerializeField] private ProbeBehabiour probeBehaviour;

    [Header("Limpieza variables")]

    public bool isCotton = false;
    public bool haveAlcohol = false;

    public bool finishCotton = false;
    public bool finishedLimpieza = false;

    private void Start()
    {
        face = GetComponent<FaceBehaviour>();
        probeBehaviour = GetComponentInParent<ProbeBehabiour>();
    }
    public void ProcessLimpieza()
    {
        if (face.alcoholPS == null)
            return;

        var mainAlcoholPS = face.alcoholPS.main;

        if (!face.alcoholPS.isEmitting && haveAlcohol && !finishedLimpieza)
        {
            face.alcoholPS.Play();
            Debug.LogWarning("[<color=blue>ALCOHOL PS play</color>]");
        }

        if (isCotton && face.alcoholPS.isEmitting && !finishedLimpieza)
        {
            if (haveAlcohol)
            {
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All,"resetCotton");
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All,"finishCottonTrue");

                mainAlcoholPS.startSize = new ParticleSystem.MinMaxCurve(0f, 0.005f);
                Debug.LogWarning("[<color=blue>Alcohol absorbed</color>]");
            }
        }


        if (face.newcalor && finishCotton)
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All,"resetHaveAlcohol");
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All,"finishedLimpiezaTrue");
            face.alcoholPS.Stop();
            mainAlcoholPS.startSize = new ParticleSystem.MinMaxCurve(0f, 0.02f);
            Debug.LogWarning("[<color=blue>Finished Limpieza</color>]");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (face.IsReady())
            return;

        var cotton = other.GetComponent<CottonBehabiour>();

        if (cotton != null && !isCotton && haveAlcohol)
        {
            cotton.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All,"AddAlcohol");
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All,"cottonColission");
            Debug.LogWarning("Cotton enter");
        }
    }

    // SCNE

    public void resetCotton()
    {
        isCotton = false;
    }
    public void finishCottonTrue()
    {
        finishCotton = true;
    }

    public void resetHaveAlcohol()
    {
        haveAlcohol = false;
    }
    public void finishedLimpiezaTrue()
    {
        finishedLimpieza = true;
    }



    public void addedAlcohol()
    {
        haveAlcohol = true;
    }
    public void cottonColission()
    {
        isCotton = true;
    }



    /////////////////

}