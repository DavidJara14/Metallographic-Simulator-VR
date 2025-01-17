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
    public bool calor = false;
    public bool newcalor = false;

    private float generalTimer = 0f;
    private float generalTimer2 = 0f;
    private float TimerPistolaDeCalor = 0f;
    //private int count = 0;

    public GameObject bodyMaterial;
    public bool isInPulidora = false;
    private float colorTimer = 0f;
    private bool isClear = false;

    [SerializeField] private VRC_Pickup pickup;

    [UdonSynced] public bool finishedPulido1 = false;
    [UdonSynced] public bool finishedPulido2 = false;
    [UdonSynced] public bool finishedAQ = false;

    private float hapticDuration = 0.05f;
    private float hapticAmplitude = 0.2f;
    private float hapticFrequency = 50f;

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
                {
                    GetComponent<Rigidbody>().AddForce(VectorDeDireccionDePuilidoActual.normalized * LAUNCH_FORCE);
                }

                if (pickup.currentPlayer != null && Networking.IsOwner(Networking.LocalPlayer, this.gameObject))
                {
                    Networking.LocalPlayer.PlayHapticEventInHand(pickup.currentHand, hapticDuration, hapticAmplitude, hapticFrequency);
                    Debug.Log("Haptic Feedback!!!!!!!!!!!!");
                }
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
        if (calor)
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
                changeColor(true);
                finishedAQ = true;
            }
        }
    }

    void Pulido()
    {

        if (!probeBehaviour.IsLijadoMax())
        {
            changeColor(false);
            //Debug.Log("Termina de lijar");
        }

        if (ownerSays)
        {
            Debug.LogError("is in trigger by error, but owner player exit, exit");
            SetCalorFalse();
            ResetTimersYBoolxd();
            ResetVars();
            //ownerSays = false;
            Debug.Log("now, owner say: " + ownerSays + " Return");
        }

        if (probeBehaviour.IsLijadoMax())
        {
            if (!haveNital && !haveAluminaGris && !haveAluminaBlanca)
            {
                //Debug.Log("ISLIJADOMAXIMO: " + probeBehaviour.IsLijadoMax());
                changeColor(false);
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

                    changeColor(true);
                    finishedPulido1 = true;
                }
            }
            else if (haveAluminaGris && haveAluminaBlanca && !haveNital) // Segunda etapa de pulido, TIENE ALUMINA BLANCA y ya tuvo gris, sin nital 
            {
                generalTimer2 += Time.deltaTime;
                Debug.Log("Tiempo de ABlanca: " + generalTimer2);
                if (generalTimer2 > 10 || finishedPulido2)
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
                    changeColor(true);
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
                Debug.LogError("Owner say in Enter: " + ownerSays.ToString());
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


        if (other.gameObject.name == "Colision" && finishedPulido1 && finishedPulido2)
        {
            //SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "CollisionCalor");
            CollisionCalor();
        }
    }
                                                
    public void CollisionCalor()
    {
        Debug.Log("ColisionCalor");
        if(haveNital)
            calor = true;
    }

    private void OnTriggerExit(Collider other)
    {
        /*if(!Networking.IsOwner(Networking.LocalPlayer, this.gameObject))
        {
            Debug.LogWarning("Player no owner, return");
            return;
            if(other.gameObject.name == "TriggerPulidora" )
            {
                ResetTimersYBoolxd();
                ResetVars();
            }
        }*/

        if(pickup.currentPlayer == null)
        {
            Debug.Log("No current player");
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "SetCalorFalse");
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ResetTimersYBoolxd");
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ResetVars");
        }

        if (other.gameObject.name == "Colision")
        {
            //SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "SetCalorFalse");
            SetCalorFalse();
        }

        if (Networking.IsOwner(Networking.LocalPlayer, this.gameObject))
        {
            Debug.LogWarning("This player is owner, objeto: " + other.gameObject.name);



            if (other.gameObject.name == "TriggerPulidora")
            {
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ResetTimersYBoolxd");
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ResetVars");
                ownerSays = true;
                Debug.LogError("Owner say in Exit: " + ownerSays.ToString());

                //ResetTimersYBoolxd();
            }
        }
    }

    public void ResetVars()
    {
        Debug.Log("ResetRefs");
        rotorPulidora = null;
        PulidoraScript = null;
    }

    public void ResetTimersYBoolxd()
    {
        Debug.Log("ResetTimersPulidora e IsInPulidoraFalse");
        isInPulidora = false;
        generalTimer = 0f;
        generalTimer2 = 0f;
    }

    public void SetCalorFalse()
    {
        Debug.Log("SecCalor");
        calor = false;
    }

    public bool IsReady()
    {
        return finishedPulido1 && finishedPulido2 && finishedAQ && probeBehaviour.IsLijadoMax();
    }

    private void changeColor(bool isGreen)
    {
        bodyMaterial.GetComponent<Renderer>().material.SetFloat("_Scale", 1.1f);
        colorTimer += Time.deltaTime;
        if (colorTimer >= 0.1f)
        {
            if (isClear)
            {
                if (isGreen)
                    bodyMaterial.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
                else
                    bodyMaterial.GetComponent<Renderer>().material.SetColor("_Color", Color.red);

                isClear = false;
            }
            else
            {
                bodyMaterial.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
                isClear = true;
            }
            colorTimer = 0f;
        }
    }

}
