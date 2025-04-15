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
    public float Desgaste = 0;
    public bool _IsFirstSanding = true;
    public bool newcalor = false;

    [Header("Pulidora")]
    [SerializeField] private GameObject rotorPulidora;
    [SerializeField] private PulidoraScript PulidoraScript;

    [Header("Pulido variables")]
    public bool haveAluminaGris = false;
    public bool haveAluminaBlanca = false;
    public bool isInPulidora = false;
    [UdonSynced] public bool finishedPulido1 = false;
    [UdonSynced] public bool finishedPulido2 = false;

    [Header("Enjuage variables")]
    [UdonSynced] public bool finishedEnjuagado = false;
    [UdonSynced] public bool finishedWater = false;

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
    [SerializeField] ParticleSystem waterPS;
    [SerializeField] ParticleSystem alcoholPS;
    [SerializeField] ParticleSystem residuosAlumina;
    [SerializeField] ParticleSystem nitalInProbePS;
    [SerializeField] ParticleSystem probetaWaterPS;

    private float generalTimer = 0f;
    private float TimerPistolaDeCalor = 0f;
    [UdonSynced] private bool ownerSays = false;
    
    const float LAUNCH_FORCE = 250f;

    [Header("Mirror")]
    public VRCMirrorReflection mirror;
    public VRCMirrorReflection mirrorCanva;

    public LayerMask PCVRMask;
    public LayerMask AndroidMask;

    [Header("TimerDebug")]
    public float elapsedTime = 0.0f;


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

        RetroalimentacionYProbetaVoladora();

        ChangeProbetaVisual();

        if (isInPulidora)
        {
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

        elapsedTime += Time.deltaTime;
        if (elapsedTime > 1.0f) 
        {

            Debug.Log("[<color=green>BoolsEnjuague</color>]");
            Debug.Log($"finishedEnjuagado -> {finishedEnjuagado}");
            Debug.Log($"finishedWater -> {finishedWater}");

            Debug.Log("[<color=cyan>BoolsLimpieza</color>]");
            Debug.Log($"isCotton -> {isCotton}");
            Debug.Log($"haveAlcohol -> {haveAlcohol}");
            Debug.Log($"finishCotton -> {finishCotton}");
            Debug.Log($"finishedLimpieza -> {finishedLimpieza}");

            Debug.Log("[<color=orange>BoolsNital</color>]");
            Debug.Log($"haveNital -> {haveNital}");
            Debug.Log($"nitalRemoved -> {nitalRemoved}");
            Debug.Log($"finishedAQ -> {finishedAQ}");

            elapsedTime = 0;
        }

        if (IsReady())
        {
            residuosAlumina.Stop();
            nitalInProbePS.Stop();
            alcoholPS.Stop();
            probetaWaterPS.Stop();
        }
    }

    private void RetroalimentacionYProbetaVoladora()
    {
        if (PulidoraScript != null && PulidoraScript.Rotating)
        {
            if (isInPulidora)
            {
                if (pickup.currentPlayer != null && Networking.IsOwner(Networking.LocalPlayer, this.gameObject))
                    gameObject.GetComponentInParent<HapticFeedback>().SendCustomEvent("hapticFeedbackPulido");
            }
        }
    }

    private void ChangeProbetaVisual()
    {
        if (!haveAluminaBlanca && !haveAluminaGris && !haveNital) // Hasta esta etapa solo se ha lijado 
        {
            probetaShader.GetComponent<Renderer>().material.SetFloat("_Reflexion", 0);
        }


        Desgaste = probetaShader.GetComponent<Renderer>().material.GetFloat("_GranoLija");

        if (Desgaste > 80)
        {
            _IsFirstSanding = false;
            probetaShader.GetComponent<Renderer>().material.SetInt("_IsFirstSanding", 0);
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
            ResetTimersYBoolxd();
            ResetVars();
        }

        if (probeBehaviour.IsLijadoMax())
        {
            if (!haveNital && !haveAluminaGris && !haveAluminaBlanca)
            {
                gameObject.GetComponentInParent<BorderColor>().SendCustomEvent("colorRed");
            }

            else if (haveAluminaGris && !haveAluminaBlanca && !haveNital) // Primera etapa de pulido, TIENE ALUMINA GRIS 
            {
                generalTimer += Time.deltaTime;
                Debug.Log("Tiempo de AGris: " + generalTimer);
                if (generalTimer > 10 || finishedPulido1)
                {
                    probetaShader.GetComponent<Renderer>().material.SetFloat("_Reflexion", 1);
                    gameObject.GetComponentInParent<BorderColor>().SendCustomEvent("colorGreen");
                    
                    finishedPulido1 = true;
                }
            }

            else if (finishedPulido1 && haveAluminaBlanca && !haveNital) // Segunda etapa de pulido, TIENE ALUMINA BLANCA y ya tuvo gris, sin nital 
            {
                generalTimer += Time.deltaTime;
                Debug.Log("Tiempo de ABlanca: " + generalTimer);
                if (generalTimer > 10 || finishedPulido2)
                {

                    probetaShader.SetActive(false);
                    probetaMirror.SetActive(true);

                    //Debug.Log("Mirror active"); 
                    gameObject.GetComponentInParent<BorderColor>().SendCustomEvent("colorGreen");
                    finishedPulido2 = true;
                }
            }
        }

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
                generalTimer += Time.deltaTime;
                Debug.Log("time agua: " + generalTimer);
                if(generalTimer > 1.5f)
                {
                    residuosAlumina.Stop();
                    //SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "waterFinished");
                    waterFinished();
                    generalTimer = 0;
                    //placersNital.SetActive(true);

                    if (probetaWaterPS != null)
                    {
                        probetaWaterPS.Play();
                        Debug.LogWarning("[<color=blue>probetawaterPS play: </color>]");
                    }
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
            //SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "enjuagadoFinished");
            enjuagadoFinished();
            Debug.LogWarning("[<color=blue>FinishedEnjuagado: </color>]" + finishedEnjuagado);
        }
    }

    public void enjuagadoFinished()
    {
        finishedEnjuagado = true;
    }

    public void waterFinished()
    {
        finishedWater = true;
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

            if (pickup.currentPlayer != null && Networking.IsOwner(Networking.LocalPlayer, this.gameObject))
            {
                gameObject.GetComponentInParent<HapticFeedback>().SendCustomEvent("hapticFeedbackCotton");
            }

            if (haveAlcohol/* || cottonGO.GetComponent<CottonBehabiour>().haveAlcohol*/)
            {
                Debug.LogWarning("[<color=blue>Alcohol absorbed </color>]");

                //haveAlcohol = false;
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "resetCotton");
                finishCotton = true;

                //resetCotton();
                mainalcoholPS.startSize = new ParticleSystem.MinMaxCurve(0f, 0.005f);
            }
        }

        if(newcalor && finishCotton)
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "resetHaveAlcoholAndFinishedLimpiezaTrue");
            finishedLimpieza = true;
            //resetHaveAlcoholAndFinishedLimpiezaTrue();
            Debug.LogWarning("[<color=blue>FinishedLimpieza: </color>]" + finishedLimpieza);
            alcoholPS.Stop();
            mainalcoholPS.startSize = new ParticleSystem.MinMaxCurve(0f, 0.02f);
        }
    }

    public void resetHaveAlcoholAndFinishedLimpiezaTrue()
    {
        haveAlcohol = false; //SCNE
        //finishedLimpieza = true;
    }

    public void resetCotton()
    {
        //finishCotton = true;
        isCotton = false; //SCNE
    }

    private void AtaqueConNital()
    {
        var mainnitalInProbePS = nitalInProbePS.main;
        if (nitalInProbePS != null && !nitalInProbePS.isEmitting && haveNital)
        {
            nitalInProbePS.Play();
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "resetHaveAlcoholAndFinishedLimpiezaTrue"); //Parche 
            Debug.LogWarning("[<color=blue>nitalInProbePS play: </color>]");
        }

        if (haveAlcohol && nitalInProbePS.isEmitting)
        {
            mainnitalInProbePS.startSize = new ParticleSystem.MinMaxCurve(0f, 0.005f);
            //SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "resetHaveNitalAndNitalRemovedTrue");
            resetHaveNitalAndNitalRemovedTrue();
            Debug.LogWarning("[<color=blue>nitalRemoved: </color>]" + nitalRemoved);

        }

        if (newcalor && nitalRemoved)
        {
            TimerPistolaDeCalor += Time.deltaTime;
            Debug.Log("Tiempo de calor: " + TimerPistolaDeCalor);
            if (TimerPistolaDeCalor > 1.5f || finishedAQ)
            {
                probetaShader.SetActive(true);
                probetaMirror.SetActive(false);
                probetaShader.GetComponent<Renderer>().material.SetFloat("_Reflexion", 0.6f);
               
                // Debug.Log("Mirror unactive");
                gameObject.GetComponentInParent<BorderColor>().SendCustomEvent("colorGreen");
                if(!finishedAQ)
                    SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "finishedAtaqueQ");
                //finishedAtaqueQ();
                nitalInProbePS.Stop();
                mainnitalInProbePS.startSize = new ParticleSystem.MinMaxCurve(0f, 0.02f);
                Debug.LogWarning("[<color=blue>FinishedAQ: </color>]" + finishedAQ);
            }
        }
    }

    public void finishedAtaqueQ()
    {
        finishedAQ = true;
        haveNital = false; //SCNE
    }

    public void resetHaveNitalAndNitalRemovedTrue()
    {
        //haveNital = false; //SCNE
        nitalRemoved = true;
    }

    private void OnParticleCollision(GameObject other)
    {
        Debug.LogWarning("[<color=green>OnParticleCollision, GO name: </color>]" + other.gameObject.name);

        if (IsReady())
        {
            return;
        }

        if (other.GetComponentInParent<IsLiquidSource>() != null)
        {
            if (finishedPulido2)
            {
                waterPS = other.GetComponent<ParticleSystem>();
                Debug.LogWarning("[<color=blue>waterPS, assigned: </color>]" + waterPS.name);
            }
        }

        if (other.GetComponentInParent<BotellaLab>() != null)
        {
            string tipo = other.GetComponentInParent<BotellaLab>().Tipo;
            if (tipo == "Nital" && finishedPulido1 && finishedPulido2 && !haveNital && finishedLimpieza)
            {
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "addedNital");
            }
            if (tipo == "Alcohol" && finishedEnjuagado && !haveAlcohol)
            {
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "addedAlcohol");
                Debug.LogWarning("[<color=red>Status haveAlcohol in particle collision: </color>]" + haveAlcohol);
            }
        }
    }

    public void addedAlcohol()
    {
        haveAlcohol = true;
    }

    public void addedNital()
    {
        haveNital = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "RotorPulidora")
        {
            if(Networking.IsOwner(Networking.LocalPlayer, this.gameObject))
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
            Debug.LogWarning("EnterTrigger, variables set");
            Debug.LogWarning("Face: " + gameObject.name);
        }

        if (IsReady())
        {
            return;
        }

        if (other.gameObject.GetComponent<CottonBehabiour>() != null && !isCotton && haveAlcohol)
        {

            //cottonGO = other.gameObject;
            other.GetComponent<CottonBehabiour>().SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "AddAlcohol");
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "cottonColission");
            Debug.LogWarning("Cotton enter");
        }
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
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ResetTimersYBoolxd");
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ResetVars");
            }
        }

        if (Networking.IsOwner(Networking.LocalPlayer, this.gameObject))
        {
            Debug.LogWarning("This player is owner, objeto: " + other.gameObject.name);
            if (other.gameObject.name == "RotorPulidora")
            {
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ResetTimersYBoolxd");
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ResetVars");
                ownerSays = true;
                Debug.LogWarning("[<color=green>OwnerSay</color>]Owner say in Exit: " + ownerSays.ToString());
            }

        }
    }

    public void cottonColission()
    {
        isCotton = true;
    }

    public void ResetVars()
    {
        //Debug.Log("ResetRefs");
        rotorPulidora = null;
        PulidoraScript = null;
    }

    public void ResetTimersYBoolxd()
    {
        //Debug.Log("ResetTimersPulidora e IsInPulidoraFalse");
        isInPulidora = false;
        generalTimer = 0f;
    }

    public bool IsReady()
    {
        //placersMicro.SetActive(true);
        return finishedPulido1 && finishedPulido2 && finishedAQ && probeBehaviour.IsLijadoMax();
    }

}
