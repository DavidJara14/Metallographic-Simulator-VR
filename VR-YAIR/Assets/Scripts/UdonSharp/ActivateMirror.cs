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
    [SerializeField] GameObject cottonGO = null;
    [UdonSynced] public bool isCotton = false;
    [UdonSynced] public bool haveAlcohol = false;
    [UdonSynced] public bool finishCotton = false;
    [UdonSynced] public bool finishedLimpieza = false;

    [Header("Nital variables")]
    [UdonSynced] public bool haveNital = false;
    [UdonSynced] public bool nitalRemoved = false;
    [UdonSynced] public bool finishedAQ = false;

    [Header("Particle system")]
    [SerializeField] ParticleSystem waterPS;
    [SerializeField] ParticleSystem alcoholPS;
    [SerializeField]  ParticleSystem residuosAlumina;
    [SerializeField] ParticleSystem nitalInProbePS;
    [SerializeField] ParticleSystem probetaWaterPS;

    [Header("Placers")]
    [SerializeField] public GameObject placersWater = null;
    [SerializeField] public GameObject placersNital = null;

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

        placersWater.SetActive(false);
        placersNital.SetActive(false);

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
            placersWater.SetActive(true);
            ChorroDeAguaYSecado();
        }

        if (finishedEnjuagado && !finishedLimpieza)
        {
            limpieza();
            /*if (ownerSays)
            {
                Debug.LogWarning("[<color=green>OwnerSay</color>]is in trigger by error, but owner player exit, exit");
                ResetVarCotton();
            }*/
        }

        if (finishedLimpieza) // Ataque, TIENE / TUVO NITAL, tuvo alumina blanca y ya tuvo gris 
        {
            AtaqueConNital();
        }

        if (newcalor)
            Debug.LogWarning("NewCalor Active");

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
        if (waterPS != null)
        {
            if(waterPS.isEmitting && residuosAlumina != null && !finishedWater)
            {
                if (!residuosAlumina.isEmitting)
                {
                    residuosAlumina.Play();
                }
                Debug.Log("Chorro de agua fria");
                generalTimer += Time.deltaTime;
                Debug.Log("time agua: " + generalTimer);
                if(generalTimer > 3f)
                {
                    residuosAlumina.Stop();
                    finishedWater = true;
                    generalTimer = 0;
                    placersNital.SetActive(true);

                    if(probetaWaterPS != null)
                        probetaWaterPS.Play();
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
            finishedEnjuagado = true;
//            probetaMirror1.GetComponent<VRC_MirrorReflection>().
            Debug.LogWarning("[<color=blue>FinishedEnjuagado: </color>]" + finishedEnjuagado);
        }
    }

    private void limpieza()
    {
        var mainalcoholPS = alcoholPS.main;

        if (alcoholPS != null && !alcoholPS.isEmitting && haveAlcohol)
        {
            alcoholPS.Play();
        }

        if (/*cottonGO != null*/ isCotton && !finishCotton) 
        {
            if (pickup.currentPlayer != null && Networking.IsOwner(Networking.LocalPlayer, this.gameObject))
            {
                gameObject.GetComponent<HapticFeedback>().SendCustomEvent("hapticFeedbackCotton");
            }

            if (haveAlcohol || cottonGO.GetComponent<CottonBehabiour>().haveAlcohol)
            {
                //generalTimer += Time.deltaTime;
                //Debug.Log("time limpieza: " + generalTimer);
                //if(generalTimer > 3f)
                //{
                haveAlcohol = false;
                generalTimer = 0;
                finishCotton = true;

                mainalcoholPS.startSize = new ParticleSystem.MinMaxCurve(0f, 0.005f);
                isCotton = false;
                //}
            }
        }

        if(newcalor && finishCotton)
        {
            finishedLimpieza = true;
            Debug.LogWarning("[<color=blue>FinishedLimpieza: </color>]" + finishedLimpieza);
            alcoholPS.Stop();
            mainalcoholPS.startSize = new ParticleSystem.MinMaxCurve(0f, 0.02f);
        }
    }

    private void AtaqueConNital()
    {
        var mainnitalInProbePS = nitalInProbePS.main;
        if (nitalInProbePS != null && !nitalInProbePS.isEmitting && haveNital)
        {
            nitalInProbePS.Play();
        }

        if (haveAlcohol && nitalInProbePS.isEmitting)
        {
            mainnitalInProbePS.startSize = new ParticleSystem.MinMaxCurve(0f, 0.005f);
            haveNital = false;
            nitalRemoved = true;
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
                finishedAQ = true;
                haveNital = false;
                nitalInProbePS.Stop();
                mainnitalInProbePS.startSize = new ParticleSystem.MinMaxCurve(0f, 0.02f);
                Debug.LogWarning("[<color=blue>FinishedAQ: </color>]" + finishedAQ);
            }
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.GetComponentInParent<IsLiquidSource>() != null)
        {
            waterPS = other.GetComponent<ParticleSystem>();
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
            if (Networking.IsOwner(Networking.LocalPlayer, this.gameObject))
            {
                //ownerSays = false;
                //Debug.LogWarning("[<color=green>OwnerSay</color>]Owner say in Enter: " + ownerSays.ToString());
            }
            cottonGO = other.gameObject;
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "cottonColission");
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

        if(other.gameObject.GetComponent<CottonBehabiour>() != null && isCotton)
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ResetVarCotton");
            //ownerSays = true;
            //Debug.LogWarning("[<color=blue>OwnerSay</color>]Owner say in Exit: " + ownerSays.ToString());
        }
    }

    public void cottonColission()
    {
        isCotton = true;
    }

    public void ResetVarCotton()
    {
        cottonGO = null;
        isCotton = false;
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
        return finishedPulido1 && finishedPulido2 && finishedAQ && probeBehaviour.IsLijadoMax();
    }

}
