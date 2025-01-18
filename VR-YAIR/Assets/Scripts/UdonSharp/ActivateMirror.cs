using System.Threading;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ActivateMirror : UdonSharpBehaviour
{
    public GameObject probetaShader1;
    public GameObject probetaMirror1;

    public GameObject probetaShader2;
    public GameObject probetaMirror2;

    [SerializeField] public Vector3 PañoToObjSize;
    [SerializeField] public Vector3 Up;
    [SerializeField] public Vector3 VectorDeDireccionDePuilidoActual;
    const float LAUNCH_FORCE = 250f;
    [SerializeField] private GameObject rotorPulidora;
    [SerializeField] private PulidoraScript PulidoraScript;

    public bool haveAluminaGris = false;
    public bool haveAluminaBlanca = false;
    public bool haveNital = false;


    public int caraTrabajada = 1;

    public ProbeBehabiour probeBehaviour;
    public float Desgaste = 0;
    public bool _IsFirstSanding = true;
    public bool newcalor = false;

    private float generalTimer = 0f;
    private float TimerPistolaDeCalor = 0f;
    public GameObject bodyMaterial;
    public bool isInPulidora = false;

    [SerializeField] private VRC_Pickup pickup;

    [UdonSynced] public bool finishedPulido1 = false;
    [UdonSynced] public bool finishedPulido2 = false;
    [UdonSynced] public bool finishedAQ = false;
    [UdonSynced] private bool ownerSays = false;

    private void Start()
    {
        probetaShader1.SetActive(true);
        probetaMirror1.SetActive(false);

        probetaShader2.SetActive(true);
        probetaMirror2.SetActive(false);
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

        if (finishedPulido1 && finishedPulido2 && haveNital) // Ataque, TIENE / TUVO NITAL, tuvo alumina blanca y ya tuvo gris 
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

    private void AtaqueConNital()
    {
        if (newcalor)
        {
            TimerPistolaDeCalor += Time.deltaTime;
            Debug.Log("Tiempo de calor: " + TimerPistolaDeCalor);
            if (TimerPistolaDeCalor > 10 || finishedAQ)
            {
                if (caraTrabajada == 1)
                {
                    probetaShader1.SetActive(true);
                    probetaMirror1.SetActive(false);
                    probetaShader1.GetComponent<Renderer>().material.SetFloat("_Reflexion", 0.5f);
                }

                else if (caraTrabajada == 2)
                {
                    probetaShader2.SetActive(true);
                    probetaMirror2.SetActive(false);
                    probetaShader2.GetComponent<Renderer>().material.SetFloat("_Reflexion", 0.5f);
                }
                // Debug.Log("Mirror unactive");
                gameObject.GetComponent<BorderColor>().SendCustomEvent("colorGreen");
                finishedAQ = true;
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

    private void OnParticleCollision(GameObject other)
    {
        string tipo = other.GetComponentInParent<BotellaLab>().Tipo;

        if (tipo == "Nital" && finishedPulido1 && finishedPulido2)
        {
            haveNital = true;
        }
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
