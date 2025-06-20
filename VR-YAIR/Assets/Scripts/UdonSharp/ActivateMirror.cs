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
    public string positionFace = null; // Up -> Cara inferior, Down -> Cara superior
    public float desgasteProbeta = 0;
    public bool newcalor = false;
    [SerializeField] private bool scaleBorderSend = false;

    [Header("Pulidora")]
    [SerializeField] private GameObject rotorPulidora;
    [SerializeField] private PulidoraScript PulidoraScript;

    [Header("Pulido variables")] 
    [SerializeField] const float timePulido = 3f;
    public bool haveAluminaGris = false; // SCNE
    public bool haveAluminaBlanca = false; // SCNE
    public bool isInPulidora = false; // SCNE
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
            FinalState(); // Asegura que al final la probeta tenga las mismas condiciones visuales
            if (newcalor)
            {
                gameObject.GetComponentInParent<BorderColor>().SendCustomEvent("colorGreen");
                scaleBorderSend = false;
            }
            if (!newcalor && !scaleBorderSend)
            {
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ScaleBorder");
                scaleBorderSend = true;
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

        if (finishedPulido1 && finishedPulido2)
        {
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
    public void ResetTimer() // SCNE
    {
        Debug.Log("ResetTimersPulidora");
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

        if (finishedWater)
        {
            residuosAlumina.Stop();
            if (probetaWaterPS != null && !probetaWaterPS.isEmitting && !finishedEnjuagado)
            {
                probetaWaterPS.Play();
                Debug.LogWarning("[<color=blue>probetawaterPS play: </color>]");
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ResetTimer");
            }

            if (newcalor)
            {
                probetaWaterPS.Stop();
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "enjuagadoFinished");
                Debug.LogWarning("[<color=blue>FinishedEnjuagado: </color>]" + finishedEnjuagado);
            }
            return;
        }

        if (waterPS != null) // Comprueba si existe una fuente emisora de agua
        {
            //Debug.LogWarning("[<color=blue>waterPS is: </color>]" + waterPS);
            
            if (!waterPS.isEmitting) // remueve la referencia emisora de agua si no esta emitiendo
            {
                waterPS = null;
                residuosAlumina.Stop();
            }
            
            else if (residuosAlumina != null) 
            {
                if (!residuosAlumina.isEmitting)
                {
                    residuosAlumina.Play();
                    //Debug.LogWarning("[<color=gray>residuosAluminaPS play: </color>]");
                }
                if (generalTimer(1.5f, "time agua: "))
                {
                    SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "waterFinished");
                    Debug.LogWarning("[<color=blue>waterFinished </color>]" + finishedWater);
                }
                Debug.Log("Chorro de agua fria");
            }

            else
            {
                residuosAlumina.Stop();
            }
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
        if (Networking.IsOwner(Networking.LocalPlayer, probeBehaviour.gameObject) && other.gameObject.name == "RotorPulidora") 
        {
            rotorPulidora = other.GetComponentInParent<PulidoraScript>().gameObject;
            PulidoraScript = rotorPulidora.GetComponent<PulidoraScript>();
            if (PulidoraScript.GrisLoaded)
            {
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "HaveAlumGrisTrue");
            }
            if (finishedPulido1)
            {
                if (PulidoraScript.BlancaLoaded)
                {
                    SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "HaveAlumBlanTrue");
                }
            }
            if (PulidoraScript.Rotating)                                            
            {
                //IsInPulidoraTrue();
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "IsInPulidoraTrue");
            }
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

    public void HaveAlumBlanTrue() // SCNE
    {
        haveAluminaBlanca = true;
    }

    public void HaveAlumGrisTrue() // SCNE
    {
        haveAluminaGris = true;
    }

    public void IsInPulidoraTrue() //SCNE
    {
        isInPulidora = true;
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
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ResetTimer");
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ResetVars");
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ScaleBorder");
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "IsInPulidoraFalse");
            }
        }

        if (Networking.IsOwner(Networking.LocalPlayer, probeBehaviour.gameObject))
        {
            //Debug.LogWarning("This player is owner, objeto: " + other.gameObject.name);
            if (other.gameObject.name == "RotorPulidora")
            {
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ResetTimer");
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ResetVars");
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "IsInPulidoraFalse");
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ScaleBorder");
            }
        }
    }

    public void IsInPulidoraFalse() // SCNE
    {
        isInPulidora = false;
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
        string dirProbe = probeBehaviour.gameObject.GetComponent<OrientationChecker>().checkOrientation();
        if (dirProbe == null || positionFace == null) {return; }
        if(dirProbe == positionFace)
        {
            Debug.LogWarning("ScaleBorder by: " + gameObject.name + " Orientation Probe: " + dirProbe);
            probeBehaviour.bodyMaterial.GetComponent<Renderer>().material.SetFloat("_Scale", 0f);
        }
        else if (IsReady())
        {
            Debug.LogWarning("ScaleBorder by: " + gameObject.name + " Orientation Probe: " + dirProbe + "IsReady: " + IsReady());
            probeBehaviour.bodyMaterial.GetComponent<Renderer>().material.SetFloat("_Scale", 0f);
        }
    }

}
