using System.Runtime.Remoting.Messaging;
using System.Threading;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

public class ActivateMirror : UdonSharpBehaviour
{

    [Header("Probeta references")]
    public GameObject probetaShader;
    public GameObject probetaMirror;
    public ProbeBehabiour probeBehaviour;
    [SerializeField] private VRC_Pickup pickup;

    [Header("Other variables")]
    public float desgasteProbeta = 0;
    public bool newcalor = false;

    [Header("Pulidora")]
    [SerializeField] private GameObject rotorPulidora;
    [SerializeField] private PulidoraScript PulidoraScript;

    [Header("Pulido variables")]
    [SerializeField] const float timePulido = 3f;
    [UdonSynced] public bool haveAluminaGris = false;
    [UdonSynced] public bool haveAluminaBlanca = false;
    public bool isInPulidora = false;
    [UdonSynced] public bool finishedPulido1 = false;
    [UdonSynced] public bool finishedPulido2 = false;

    [Header("Enjuage variables")]
    public bool finishedEnjuagado = false;
    public bool finishedWater = false;

    [Header("Limpieza variables")]
    public bool isCotton = false;
    public bool haveAlcohol = false;
    [UdonSynced] public bool finishCotton = false;
    [UdonSynced] public bool finishedLimpieza = false;

    [Header("Nital variables")]
    public bool haveNital = false;
    public bool nitalRemoved = false;
    public bool finishedAQ = false;

    [Header("Particle system")]
    [SerializeField] public ParticleSystem waterPS;
    [SerializeField] ParticleSystem alcoholPS;
    [SerializeField] ParticleSystem residuosAlumina;
    [SerializeField] ParticleSystem nitalInProbePS;
    [SerializeField] ParticleSystem probetaWaterPS;

    private float timer = 0f;
    [UdonSynced] private bool ownerSays = false;
    
    const float LAUNCH_FORCE = 250f;

    [Header("Mirror")]
    public VRCMirrorReflection mirror;
    public VRCMirrorReflection mirrorCanva;

    public LayerMask PCVRMask;
    public LayerMask AndroidMask;


    private void Start()
    {
        if(probetaShader != null)
        {
            probetaShader.SetActive(true);
        }

        if(probetaMirror != null)
        {
            probetaMirror.SetActive(false);
        }

        if(residuosAlumina != null)
            residuosAlumina.Stop();

        if(nitalInProbePS != null)
            nitalInProbePS.Stop();

        if (alcoholPS != null)
            alcoholPS.Stop();

        if(probetaWaterPS != null)
            probetaWaterPS.Stop();

#if UNITY_ANDROID
        mirror.m_ReflectLayers = AndroidMask;  
        mirrorCanva.m_ReflectLayers = AndroidMask;
        
#else
        mirror.m_ReflectLayers = PCVRMask;
        mirrorCanva.m_ReflectLayers = PCVRMask;
#endif
    }

    private void Update()
    {
        // Alumina gris -> Para efectos practicos le dara brillo 
        // Alumina blanca ->´Para efectos practicos le dara acabado espejo 

        if (IsReady())
        {
            residuosAlumina.Stop();
            nitalInProbePS.Stop();
            alcoholPS.Stop();
            probetaWaterPS.Stop();
            FinalState();
            if (newcalor)
            {
                gameObject.GetComponentInParent<BorderColor>().SendCustomEvent("colorGreen");
            }
            if (!newcalor)
            {
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ScaleBorder");
            }
            return;
        }

        ChangeProbetaVisual();

        if (isInPulidora)
        {
            if((finishedPulido1 && !haveAluminaBlanca) || finishedPulido2)
            {
                gameObject.GetComponentInParent<BorderColor>().SendCustomEvent("colorGreen");
                return;
            }
            Retroalimentacion();
            Pulido();
        }

        if (finishedPulido1 && finishedPulido2 && !finishedEnjuagado)
        {
            if (waterPS != null && !waterPS.isEmitting)
            {
                waterPS = null;
                residuosAlumina.Stop();
            }
            ChorroDeAguaYSecado();
        }

        if (finishedEnjuagado && !finishedLimpieza)
        {
            limpieza();
        }

        if (finishedLimpieza) // Ataque, TIENE / TUVO NITAL, tuvo alumina blanca y ya tuvo gris 
        {
            AtaqueConNital();
        }

    }

    private void ChangeProbetaVisual()
    {
        if (!haveAluminaBlanca && !haveAluminaGris && !haveNital) // Hasta esta etapa solo se ha lijado 
        {
            probetaShader.GetComponent<Renderer>().material.SetFloat("_Reflexion", 0);
        }

        desgasteProbeta = probetaShader.GetComponent<Renderer>().material.GetFloat("_GranoLija");

        if (desgasteProbeta > 80)
        {
            probetaShader.GetComponent<Renderer>().material.SetInt("_IsFirstSanding", 0);
        }
    }

    private void Retroalimentacion()
    {
        if (PulidoraScript != null && pickup.currentPlayer != null && Networking.IsOwner(Networking.LocalPlayer, probeBehaviour.gameObject))
        {
            gameObject.GetComponentInParent<HapticFeedback>().SendCustomEvent("hapticFeedbackPulido");
        }
    }


    void Pulido()
    {
        if (!probeBehaviour.IsLijadoMax())
        {
            gameObject.GetComponentInParent<BorderColor>().SendCustomEvent("colorRed");
            //Debug.Log("Termina de lijar");
        }

        if (ownerSays)
        {
            Debug.LogWarning("[<color=green>OwnerSay</color>]is in trigger by error, but owner player exit, exit");
            ResetTimerAndBool(); // Sync by owner
            ResetVars();         // Sync by owner
        }

        if (probeBehaviour.IsLijadoMax())
        {
            if (!haveNital && !haveAluminaGris && !haveAluminaBlanca)
            {
                gameObject.GetComponentInParent<BorderColor>().SendCustomEvent("colorRed");
            }

            else if (haveAluminaGris && !haveAluminaBlanca && !haveNital) // Primera etapa de pulido, TIENE ALUMINA GRIS 
            {
                ScaleBorder();
                if (generalTimer(timePulido, "Tiempo de AGris: "))
                {
                    probetaShader.GetComponent<Renderer>().material.SetFloat("_Reflexion", 1);
                    finishedPulido1 = true;
                }
            }

            else if (finishedPulido1 && haveAluminaBlanca && !haveNital) // Segunda etapa de pulido, TIENE ALUMINA BLANCA y ya tuvo gris, sin nital 
            {
                ScaleBorder();
                if (generalTimer(timePulido, "Tiempo de ABlanca: "))
                {
                    probetaShader.SetActive(false);
                    probetaMirror.SetActive(true);
                    //Debug.Log("Mirror active"); 
                    finishedPulido2 = true;
                }
            }
        }

    }
    public void ResetTimerAndBool() // SCNE
    {
        //Debug.Log("ResetTimersPulidora e IsInPulidoraFalse");
        isInPulidora = false; 
        timer = 0f;
    }
    public void ResetVars() // SCNE
    {
        //Debug.Log("ResetRefs");
        rotorPulidora = null;
        PulidoraScript = null;
    }

    private void ChorroDeAguaYSecado()
    {
        //Debug.LogWarning("Enter in ChorroDeAguaYSecado");
        if (waterPS != null)
        {
            //Debug.LogWarning("[<color=blue>waterPS is: </color>]" + waterPS);
            if (waterPS.isEmitting && residuosAlumina != null && !finishedWater)
            {
                if (!residuosAlumina.isEmitting)
                {
                    residuosAlumina.Play();
                    //Debug.LogWarning("[<color=gray>residuosAluminaPS play: </color>]");
                }
                Debug.Log("Chorro de agua fria");
                if(generalTimer(1.5f, "time agua: "))
                {
                    residuosAlumina.Stop();
                    SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "waterFinished");
                    //waterFinished();
                    Debug.LogWarning("[<color=blue>waterFinished </color>]" + finishedWater);

                    if (probetaWaterPS != null)
                    {
                        probetaWaterPS.Play();
                        Debug.LogWarning("[<color=blue>probetawaterPS play: </color>]");
                    }
                    SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ResetTimerAndBool");
                }
            }
            else
            {
                residuosAlumina.Stop();
            }
        }

        if (newcalor && finishedWater)
        {
            probetaWaterPS.Stop();
            //enjuagadoFinished();
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "enjuagadoFinished");
            Debug.LogWarning("[<color=blue>FinishedEnjuagado: </color>]" + finishedEnjuagado);
        }
    }
    public void waterFinished() // SCNE
    {
        finishedWater = true;
    }
    public void enjuagadoFinished() // SCNE
    {
        finishedEnjuagado = true;
    }

    private void limpieza()
    {
        //Debug.LogWarning("Enter in lIMPIEZA");
        var mainalcoholPS = alcoholPS.main;

        if (alcoholPS != null && !alcoholPS.isEmitting && haveAlcohol)
        {
            alcoholPS.Play();
            Debug.LogWarning("[<color=blue>ALCOHOLps play: </color>]");
        }

        if (isCotton && alcoholPS.isEmitting) 
        {
            //Debug.LogWarning("[<color=blue>is cotton: </color>]" + isCotton);

            if (pickup.currentPlayer != null && Networking.IsOwner(Networking.LocalPlayer, probeBehaviour.gameObject))
            {
                gameObject.GetComponentInParent<HapticFeedback>().SendCustomEvent("hapticFeedbackCotton");
            }

            if (haveAlcohol)
            {
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "resetCotton");
                finishCotton = true; // udonsync
                mainalcoholPS.startSize = new ParticleSystem.MinMaxCurve(0f, 0.005f);
                Debug.LogWarning("[<color=blue>Alcohol absorbed </color>]");
            }
        }

        if(newcalor && finishCotton)
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "resetHaveAlcohol");
            finishedLimpieza = true; // udonsync
            alcoholPS.Stop();
            mainalcoholPS.startSize = new ParticleSystem.MinMaxCurve(0f, 0.02f);
            Debug.LogWarning("[<color=blue>FinishedLimpieza: </color>]" + finishedLimpieza);
        }
    }
    public void resetCotton() //SCNE
    {
        isCotton = false; 
    }
    public void resetHaveAlcohol() //SCNE
    {
        haveAlcohol = false; 
    }

    private void AtaqueConNital()
    {
        var mainnitalInProbePS = nitalInProbePS.main;
        if (nitalInProbePS != null && !nitalInProbePS.isEmitting && haveNital)
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "resetHaveAlcohol"); //Parche 
            nitalInProbePS.Play();
            Debug.LogWarning("[<color=blue>nitalInProbePS play: </color>]");
        }

        if (haveAlcohol && nitalInProbePS.isEmitting)
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "nitalRemovedTrue");
            mainnitalInProbePS.startSize = new ParticleSystem.MinMaxCurve(0f, 0.005f);
            Debug.LogWarning("[<color=blue>nitalRemoved: </color>]" + nitalRemoved);
        }

        if (newcalor && nitalRemoved)
        {
            if (generalTimer(1.5f, "Tiempo de calor: "))
            {
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "finishedAtaqueQ");
                nitalInProbePS.Stop();
                mainnitalInProbePS.startSize = new ParticleSystem.MinMaxCurve(0f, 0.02f);
                Debug.LogWarning("[<color=blue>FinishedAQ: </color>]" + finishedAQ);
            }
        }
    }
    private void FinalState() 
    {
        probetaShader.SetActive(true);
        probetaMirror.SetActive(false);
        probetaShader.GetComponent<Renderer>().material.SetFloat("_Reflexion", 0.6f);
    }
    public void finishedAtaqueQ() //SCNE
    {
        finishedAQ = true;
        haveNital = false; 
    }
    public void nitalRemovedTrue() //SCNE
    {
        nitalRemoved = true;
    }
    
    public void addedNital() //SCNE, used in DetectParticleCollision.cs
    {
        haveNital = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "RotorPulidora")
        {
            if(Networking.IsOwner(Networking.LocalPlayer, probeBehaviour.gameObject))
            {
                ownerSays = false;
                Debug.LogWarning("[<color=green>OwnerSay</color>]Owner say in Enter: " + ownerSays.ToString());
            }

            rotorPulidora = other.GetComponentInParent<PulidoraScript>().gameObject;
            PulidoraScript = rotorPulidora.GetComponent<PulidoraScript>();
            haveAluminaGris = PulidoraScript.GrisLoaded;
            if (finishedPulido1)
            {
                haveAluminaBlanca = PulidoraScript.BlancaLoaded;            
            }
            isInPulidora = PulidoraScript.Rotating;
            //Debug.LogWarning("EnterTrigger, variables set");
            //Debug.LogWarning("Face: " + gameObject.name);
        }

        if (IsReady())
        {
            return;
        }

        if (other.gameObject.GetComponent<CottonBehabiour>() != null && !isCotton && haveAlcohol)
        {
            other.GetComponent<CottonBehabiour>().SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "AddAlcohol");
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "cottonColission");
            Debug.LogWarning("Cotton enter");
        }
    }                                           
    public void addedAlcohol() //SCNE
    {
        haveAlcohol = true;
    }
    public void cottonColission() //SCNE
    {
        isCotton = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<ProbetaSnap>() != null)
            return;

        if(pickup.currentPlayer == null)
        {
            Debug.Log("No current player");
            if (isInPulidora)
            {
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ResetTimerAndBool");
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ResetVars");
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ScaleBorder");
            }
        }

        if (Networking.IsOwner(Networking.LocalPlayer, probeBehaviour.gameObject))
        {
            //Debug.LogWarning("This player is owner, objeto: " + other.gameObject.name);
            if (other.gameObject.name == "RotorPulidora")
            {
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ResetTimerAndBool");
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ResetVars");
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ScaleBorder");
                ownerSays = true;
                Debug.LogWarning("[<color=green>OwnerSay</color>]Owner say in Exit: " + ownerSays.ToString());
            }
        }
    }

    private bool generalTimer(float maxTime, string messageDebug)
    {
        timer += Time.deltaTime;
        Debug.Log(messageDebug + timer);
        if (timer > maxTime) { return true; }
        else { return false; }
    }
    public bool IsReady()
    {
        return finishedPulido1 && finishedPulido2 && finishedAQ && probeBehaviour.IsLijadoMax();
    }

    public void ScaleBorder()
    {
        probeBehaviour.bodyMaterial.GetComponent<Renderer>().material.SetFloat("_Scale", 0f);
    }

}
