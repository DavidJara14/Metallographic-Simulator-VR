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

    private float generalTimer = 0f;
    private float generalTimer2 = 0f;
    private float generalTimer3 = 0f;


    public GameObject bodyMaterial;
    public bool isInPulidora = false;
    private float colorTimer = 0f;
    private bool isClear = false;

    [SerializeField] private VRC_Pickup pickup;

    public bool finishedPulido1 = false;
    public bool finishedPulido2 = false;
    public bool finishedAQ = false;

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

        if(PulidoraScript != null && PulidoraScript.Rotating)
        {
            PañoToObjSize = new Vector3(gameObject.transform.position.x - PulidoraScript.transform.position.x, 0f, gameObject.transform.position.z - PulidoraScript.transform.position.z);
            Up = gameObject.transform.up;
            VectorDeDireccionDePuilidoActual = Vector3.Cross(PañoToObjSize, Up);

            if (pickup.currentPlayer == null && isInPulidora )
            {
                GetComponent<Rigidbody>().AddForce(VectorDeDireccionDePuilidoActual.normalized * 250f);
            }
        }

        if (!haveAluminaBlanca && !haveAluminaGris && !haveNital) // Hasta esta etapa solo se ha lijado 
        {
            if (caraTrabajada == 1)
                probetaShader1.GetComponent<Renderer>().material.SetFloat("_Reflexion", 0);

            else if (caraTrabajada == 2)
                probetaShader2.GetComponent<Renderer>().material.SetFloat("_Reflexion", 0);
        }

        if(isInPulidora)
        {
            if (!probeBehaviour.IsLijadoMax())
            {
                changeColor(false);
                Debug.Log("Termina de lijar");
            }

            if (probeBehaviour.IsLijadoMax())
            {
                if(!haveNital && !haveAluminaGris && !haveAluminaBlanca)
                {
                    Debug.Log("ISLIJADOMAXIMO: " + probeBehaviour.IsLijadoMax());
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
                else if (haveAluminaGris && haveAluminaBlanca && haveNital) // Ataque, TIENE / TUVO NITAL, tuvo alumina blanca y ya tuvo gris 
                {
                    if (calor)
                    {
                        generalTimer3 += Time.deltaTime;
                        Debug.Log("Tiempo de calor: " + generalTimer3);
                        if (generalTimer3 > 10 || finishedAQ)
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
                        }
                        finishedAQ = true;
                    }
                }
            }

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
        if(other.GetComponent<PulidoraScript>() != null)
        {
            rotorPulidora = other.gameObject;
            PulidoraScript = rotorPulidora.GetComponent<PulidoraScript>();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.name == "Colision" && finishedPulido1 && finishedPulido2)
        {
            Debug.Log("ColisionCalor");
            calor = true;
        }

        if (other.gameObject.name == "ColisionPañoGris")
        {
            Debug.Log("ColisionGris");
            haveAluminaGris = true;
        }

        if (other.gameObject.name == "ColisionPañoBlanco" && haveAluminaGris && finishedPulido1)
        {
            Debug.Log("ColisionBlanca");
            haveAluminaBlanca = true;
        }

        if (other.gameObject.name == "RotorPulidora")
        {
            isInPulidora = other.GetComponent<PulidoraScript>().Rotating;
            Debug.Log("Is in pulidora: " + isInPulidora);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Colision")
        {
            calor = false;
        }

        if (other.gameObject.name == "RotorPulidora") 
        {
            isInPulidora = false;
            generalTimer = 0f;
            generalTimer2 = 0f;
        }

        if (other.GetComponent<PulidoraScript>())
        {
            rotorPulidora = null;
            PulidoraScript = null;
        }
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
                Debug.Log("Desgaste set to: " + Desgaste);
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
