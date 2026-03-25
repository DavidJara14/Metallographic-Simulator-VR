using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.SDK3.Components;

public class PulidoAlumina : UdonSharpBehaviour
{

    [Header("References")]
    [SerializeField] private FaceBehaviour face;
    [SerializeField] private ProbeBehabiour probeBehaviour;
    [SerializeField] private VRC_Pickup pickup;

    [Header("Pulidora")]
    [SerializeField] private GameObject rotorPulidora;
    [SerializeField] private PulidoraScript pulidoraScript;

    [Header("Pulido variables")]
    const float timePulido = 3f;

    public bool haveAluminaGris = false;   //SCNE
    public bool haveAluminaBlanca = false; //SCNE

    public bool isInPulidoraGris = false;  //SCNE
    public bool isInPulidoraBlanca = false;//SCNE

    public bool finishedPulido1 = false;   //SCNE
    public bool finishedPulido2 = false;   //SCNE

    [Header("Lavado Alumina")]
    public bool finishedWater = false;     //SCNE
    public bool finishedEnjuagado = false; //SCNE

    public ParticleSystem waterPS;

    private float timer = 0f;

    private void Start()
    {
        face = GetComponent<FaceBehaviour>();
        probeBehaviour = GetComponentInParent<ProbeBehabiour>();
        pickup = GetComponentInParent<VRC_Pickup>();
    }


    public void ProcessPulido()
    {
        if (isInPulidoraGris || isInPulidoraBlanca)
        {
            Retroalimentacion();
            Pulido();
        }
    }

    public void ProcessLavado()
    {
        if (face.residuosAlumina == null)
            return;

        if (finishedWater)
        {
            face.residuosAlumina.Stop();

            if (face.probetaWaterPS != null && !face.probetaWaterPS.isEmitting && !finishedEnjuagado)
            {
                face.probetaWaterPS.Play();
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All,"ResetTimer");
            }

            if (face.newcalor)
            {
                face.probetaWaterPS.Stop();

                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All,"enjuagadoFinished");
            }

            return;

        }


        if (waterPS != null)
        {
            if (!waterPS.isEmitting)
            {
                waterPS = null;
                face.residuosAlumina.Stop();
            }

            else
            {
                if (!face.residuosAlumina.isEmitting)
                {
                    face.residuosAlumina.Play();
                }

                if (generalTimer(1.5f))
                {
                    SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All,"waterFinished");

                }
            }
        }
    }


    private void Retroalimentacion()
    {
        if (pulidoraScript != null && pickup.currentPlayer != null && Networking.IsOwner(Networking.LocalPlayer, probeBehaviour.gameObject))
        {
            gameObject.GetComponentInParent<HapticFeedback>().SendCustomEvent("hapticFeedbackPulido");
        }
    }

    private void Pulido()
    {

        if (!probeBehaviour.IsLijadoMax())
        {
            gameObject.GetComponentInParent<BorderColor>().SendCustomEvent("colorRed");
            return;
        }

        if ((finishedPulido1 && isInPulidoraGris) || (finishedPulido2 && isInPulidoraBlanca))
        {
            gameObject.GetComponentInParent<BorderColor>().SendCustomEvent("colorGreen");
            return;
        }

        bool probeNoHaveAlumina = !haveAluminaGris && !haveAluminaBlanca;
        bool pulidoCorrecto = (isInPulidoraGris && haveAluminaGris) || (isInPulidoraBlanca && haveAluminaBlanca);

        if (probeNoHaveAlumina || !pulidoCorrecto)
        {
            gameObject.GetComponentInParent<BorderColor>().SendCustomEvent("colorRed");
            return;
        }

        if (pulidoCorrecto && !finishedPulido1)
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "CallScaleBorder");
            if (generalTimer(timePulido))
            {
                face.probetaShader.GetComponent<Renderer>().material.SetFloat("_Reflexion", 1);
                finishedPulido1 = true;
            }
        }

        else if (pulidoCorrecto && finishedPulido1)
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "CallScaleBorder");
            if (generalTimer(timePulido))
            {
                face.probetaShader.SetActive(false);
                face.probetaMirror.SetActive(true);
                finishedPulido2 = true;
            }
        }
    }

    public bool generalTimer(float maxTime)
    {
        timer += Time.deltaTime;

        if (timer > maxTime)
        { return true; }

        return false;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!Networking.IsOwner(Networking.LocalPlayer, probeBehaviour.gameObject))
            return;

        if (other.gameObject.name == "RotorPulidora")
        {
            rotorPulidora = other.GetComponentInParent<PulidoraScript>().gameObject;
            pulidoraScript = rotorPulidora.GetComponent<PulidoraScript>();

            if (pulidoraScript.GrisLoaded)
            {
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All,"HaveAlumGrisTrue");
            }

            if (finishedPulido1)
            {
                if (pulidoraScript.BlancaLoaded)
                {
                    SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All,"HaveAlumBlanTrue");
                }
            }

            if (pulidoraScript.Rotating)
            {
                if (pulidoraScript.IsForGris)
                {
                    SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All,"IsInPulidoraGrisTrue");
                }

                if (pulidoraScript.IsForBlanca)
                {
                    SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All,"IsInPulidoraBlancaTrue");

                }

            }

        }

    }


    private void OnTriggerExit(Collider other)
    {
        if (pickup.currentPlayer == null)
        {
            if (isInPulidoraGris || isInPulidoraBlanca)
            {
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ResetTimer");
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ResetVars");
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "CallScaleBorder");
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "IsInPulidoraGrisFalse");
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "IsInPulidoraBlancaFalse");
            }

        }

        if (Networking.IsOwner(Networking.LocalPlayer, probeBehaviour.gameObject))
        {
            Debug.LogWarning("This player is owner, objeto: " + other.gameObject.name);
            if (other.gameObject.name == "RotorPulidora")
            {
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ResetTimer");
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ResetVars");
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "CallScaleBorder");
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "IsInPulidoraGrisFalse");
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "IsInPulidoraBlancaFalse");
            }
        }

    }


    // SCNE

    public void IsInPulidoraGrisTrue()
    {
        isInPulidoraGris = true;
    }
    public void IsInPulidoraGrisFalse()
    {
        isInPulidoraGris = false;
    }
    public void IsInPulidoraBlancaTrue()
    {
        isInPulidoraBlanca = true;
    }
    public void IsInPulidoraBlancaFalse()
    {
        isInPulidoraBlanca = false;
    }

    public void ResetVars()
    {
        rotorPulidora = null;
        pulidoraScript = null;
    }


    public void HaveAlumBlanTrue()
    {
        haveAluminaBlanca = true;
    }
    public void HaveAlumGrisTrue()
    {
        haveAluminaGris = true;
    }


    public void ResetTimer()
    {
        timer = 0f;
    }

    public void waterFinished()
    {
        finishedWater = true;
    }
    public void enjuagadoFinished()
    {
        finishedEnjuagado = true;
    }

    
    public void CallScaleBorder()
    {
        face.ScaleBorder();
    }

    //////////////////////////////////

}