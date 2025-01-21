using System.Runtime.Remoting.Messaging;
using System.Threading;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ActivateMirror : UdonSharpBehaviour
{

    [Header("Probeta references")]
    public GameObject probetaShader1;
    public GameObject probetaMirror1;
    public GameObject probetaShader2;
    public GameObject probetaMirror2;
    public GameObject bodyMaterial;
    public ProbeBehabiour probeBehaviour;
    [SerializeField] private VRC_Pickup pickup;
    public int caraTrabajada = 1;

    [Header("Other variables")]
    public float Desgaste = 0;
    public bool _IsFirstSanding = true;
    public bool newcalor = false;

    [Header("Vectores")]
    [SerializeField] public Vector3 PañoToObjSize;
    [SerializeField] public Vector3 Up;
    [SerializeField] public Vector3 VectorDeDireccionDePuilidoActual;
    

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
    //[SerializeField] GameObject cottonGO = null;
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
    [SerializeField]  ParticleSystem residuosAlumina;
    [SerializeField] ParticleSystem nitalInProbePS;
    [SerializeField] ParticleSystem probetaWaterPS;

    [Header("Placers")]
    [SerializeField] public GameObject placersWater = null;
    [SerializeField] public GameObject placersNital = null;
    [SerializeField] public GameObject placersMicro = null;

    private float generalTimer = 0f;
    private float TimerPistolaDeCalor = 0f;
    [UdonSynced] private bool ownerSays = false;
    
    const float LAUNCH_FORCE = 250f;

    private void Start()
    {
        probetaShader1.SetActive(true);
        probetaMirror1.SetActive(false);

        probetaShader2.SetActive(true);
        probetaMirror2.SetActive(false);

        if(residuosAlumina != null)
            residuosAlumina.Stop();

        if(nitalInProbePS != null)
            nitalInProbePS.Stop();

        if (alcoholPS != null)
            alcoholPS.Stop();

        if(probetaWaterPS != null)
            probetaWaterPS.Stop();
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
            //placersWater.SetActive(true);
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

    private void RetroalimentacionYProbetaVoladora()
    {
        if (PulidoraScript != null && PulidoraScript.Rotating)
        {
            PañoToObjSize = new Vector3(gameObject.transform.position.x - PulidoraScript.transform.position.x, 0f, gameObject.transform.position.z - PulidoraScript.transform.position.z);
            Up = gameObject.transform.up;
            VectorDeDireccionDePuilidoActual = Vector3.Cross(PañoToObjSize, Up);
            if (isInPulidora)
            {
                if (pickup.currentPlayer == null)
                    GetComponent<Rigidbody>().AddForce(VectorDeDireccionDePuilidoActual.normalized * LAUNCH_FORCE);

                if (pickup.currentPlayer != null && Networking.IsOwner(Networking.LocalPlayer, this.gameObject))
                    gameObject.GetComponent<HapticFeedback>().SendCustomEvent("hapticFeedbackPulido");
            }
        }
    }

    private void ChangeProbetaVisual()
    {
        if (!haveAluminaBlanca && !haveAluminaGris && !haveNital) // Hasta esta etapa solo se ha lijado 
        {
            if (caraTrabajada == 1)
                probetaShader1.GetComponent<Renderer>().material.SetFloat("_Reflexion", 0);

            else if (caraTrabajada == 2)
                probetaShader2.GetComponent<Renderer>().material.SetFloat("_Reflexion", 0);
        }


        Desgaste = probetaShader1.GetComponent<Renderer>().material.GetFloat("_GranoLija");

        if (Desgaste > 80)
        {
            _IsFirstSanding = false;
            if (caraTrabajada == 1)
            {
                probetaShader1.GetComponent<Renderer>().material.SetInt("_IsFirstSanding", 0);
            }
            else if (caraTrabajada == 2)
            {
                probetaShader2.GetComponent<Renderer>().material.SetInt("_IsFirstSanding", 0);
            }
        }
    }

    void Pulido()
    {

        if (!probeBehaviour.IsLijadoMax())
        {
            gameObject.GetComponent<BorderColor>().SendCustomEvent("colorRed");
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
                gameObject.GetComponent<BorderColor>().SendCustomEvent("colorRed");
            }

            else if (haveAluminaGris && !haveAluminaBlanca && !haveNital) // Primera etapa de pulido, TIENE ALUMINA GRIS 
            {
                generalTimer += Time.deltaTime;
                Debug.Log("Tiempo de AGris: " + generalTimer);
                if (generalTimer > 10 || finishedPulido1)
                {
                    if (caraTrabajada == 1)
                        probetaShader1.GetComponent<Renderer>().material.SetFloat("_Reflexion", 1);
                    else if (caraTrabajada == 2)
                        probetaShader2.GetComponent<Renderer>().material.SetFloat("_Reflexion", 1);
                    gameObject.GetComponent<BorderColor>().SendCustomEvent("colorGreen");
                    
                    finishedPulido1 = true;
                }
            }

            else if (haveAluminaGris && haveAluminaBlanca && !haveNital) // Segunda etapa de pulido, TIENE ALUMINA BLANCA y ya tuvo gris, sin nital 
            {
                generalTimer += Time.deltaTime;
                Debug.Log("Tiempo de ABlanca: " + generalTimer);
                if (generalTimer > 10 || finishedPulido2)
                {
                    if (caraTrabajada == 1)
                    {
                        probetaShader1.SetActive(false);
                        probetaMirror1.SetActive(true);
                    }

                    else if (caraTrabajada == 2)
                    {
                        probetaShader2.SetActive(false);
                        probetaMirror2.SetActive(true);
                    }
                    //Debug.Log("Mirror active"); 
                    gameObject.GetComponent<BorderColor>().SendCustomEvent("colorGreen");
                    finishedPulido2 = true;
                }
            }
        }

    }

    private void ChorroDeAguaYSecado()
    {
        Debug.LogWarning("Enter in ChorroDeAguaYSecado");
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
                if(generalTimer > 3f)
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
        Debug.LogWarning("Enter in lIMPIEZA");

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
                gameObject.GetComponent<HapticFeedback>().SendCustomEvent("hapticFeedbackCotton");
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
            //SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "resetHaveAlcoholAndFinishedLimpiezaTrue");
            finishedLimpieza = true;
            resetHaveAlcoholAndFinishedLimpiezaTrue();
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
            if (TimerPistolaDeCalor > 5f || finishedAQ)
            {
                if (caraTrabajada == 1)
                {
                    probetaShader1.SetActive(true);
                    probetaMirror1.SetActive(false);
                    probetaShader1.GetComponent<Renderer>().material.SetFloat("_Reflexion", 0.6f);
                }

                else if (caraTrabajada == 2)
                {
                    probetaShader2.SetActive(true);
                    probetaMirror2.SetActive(false);
                    probetaShader2.GetComponent<Renderer>().material.SetFloat("_Reflexion", 0.6f);
                }
                // Debug.Log("Mirror unactive");
                gameObject.GetComponent<BorderColor>().SendCustomEvent("colorGreen");
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
        //Debug.LogWarning("[<color=green>OnParticleCollision, GO name: </color>]" + other.gameObject.name);
        if (other.GetComponentInParent<IsLiquidSource>() != null)
        {
            waterPS = other.GetComponent<ParticleSystem>();
            Debug.LogWarning("[<color=blue>waterPS, assigned: </color>]" + waterPS.name);
        }


        if (other.GetComponentInParent<BotellaLab>() != null)
        {
            string tipo = other.GetComponentInParent<BotellaLab>().Tipo;
            if (tipo == "Nital" && finishedPulido1 && finishedPulido2 && !haveNital)
            {
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "addedNital");
            }
            if (tipo == "Alcohol" && finishedEnjuagado && !haveAlcohol)
            {
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "addedAlcohol");
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
        if (other.gameObject.name == "TriggerPulidora")
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
        }

        if (other.gameObject.GetComponent<CottonBehabiour>() != null && !isCotton)
        {

            //cottonGO = other.gameObject;
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "cottonColission");
            Debug.LogWarning("Cotton enter");
        }
    }
                                                

    private void OnTriggerExit(Collider other)
    {
        if(pickup.currentPlayer == null)
        {
            Debug.Log("No current player");
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ResetTimersYBoolxd");
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ResetVars");
        }

        if (Networking.IsOwner(Networking.LocalPlayer, this.gameObject))
        {
            Debug.LogWarning("This player is owner, objeto: " + other.gameObject.name);
            if (other.gameObject.name == "TriggerPulidora")
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
