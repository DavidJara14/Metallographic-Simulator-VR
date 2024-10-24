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

        if (!haveAluminaBlanca && !haveAluminaGris && !haveNital) // Hasta esta etapa solo se ha lijado 
        {
            if (caraTrabajada == 1)
                probetaShader1.GetComponent<Renderer>().material.SetFloat("_Reflexion", 0);

            else if (caraTrabajada == 2)
                probetaShader2.GetComponent<Renderer>().material.SetFloat("_Reflexion", 0);

        }

        if (isInPulidora && probeBehaviour.IsLijadoMaximo() && !haveNital && !haveAluminaGris && !haveAluminaBlanca)
        {
            Debug.Log("ISLIJADOMAXIMO: " + probeBehaviour.IsLijadoMaximo());
            bodyMaterial.GetComponent<Renderer>().material.SetFloat("_Scale", 1.1f);
            colorTimer += Time.deltaTime;
            if (colorTimer >= 0.1f)
            {
                if (isClear)
                {
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
        
        if (isInPulidora && haveAluminaGris && !haveAluminaBlanca && probeBehaviour.IsLijadoMaximo() && !haveNital) // Primera etapa de pulido, TIENE ALUMINA GRIS 
        {
            generalTimer += Time.deltaTime;
            Debug.Log("Tiempo de AGris: "+generalTimer);
            if(generalTimer > 10)
            {
                if (caraTrabajada == 1)
                    probetaShader1.GetComponent<Renderer>().material.SetFloat("_Reflexion", 1);
                else if (caraTrabajada == 2)
                    probetaShader2.GetComponent<Renderer>().material.SetFloat("_Reflexion", 1);

                bodyMaterial.GetComponent<Renderer>().material.SetFloat("_Scale", 1.1f);
                
                colorTimer += Time.deltaTime;
                if (colorTimer >= 0.1f)
                {
                    if (isClear)
                    {
                        bodyMaterial.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
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
        
        if (isInPulidora && haveAluminaGris && haveAluminaBlanca && probeBehaviour.IsLijadoMaximo() && !haveNital) // Segunda etapa de pulido, TIENE ALUMINA BLANCA y ya tuvo gris, sin nital 
        {
            //generalTimer = 0;
            generalTimer2 += Time.deltaTime;
            Debug.Log("Tiempo de ABlanca: " + generalTimer2);
            if (generalTimer2 > 10)
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
                bodyMaterial.GetComponent<Renderer>().material.SetFloat("_Scale", 1.1f);


                colorTimer += Time.deltaTime;
                if (colorTimer >= 0.1f)
                {
                    if (isClear)
                    {
                        bodyMaterial.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
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
        
        if (haveAluminaGris && haveAluminaBlanca && haveNital && calor) // Segunda etapa de pulido, TIENE / TUVO NITAL, tuvo alumina blanca y ya tuvo gris 
        {
            generalTimer3 += Time.deltaTime;
            Debug.Log("Tiempo de ABlanca: " + generalTimer3);
            if (generalTimer3 > 10)
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
                bodyMaterial.GetComponent<Renderer>().material.SetFloat("_Scale", 1.1f);

                // Debug.Log("Mirror unactive");
                colorTimer += Time.deltaTime;
                if (colorTimer >= 0.1f)
                {
                    if (isClear)
                    {
                        bodyMaterial.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
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

        Desgaste = probetaShader1.GetComponent<Renderer>().material.GetFloat("_GranoLija");
        //Debug.Log("Desgaste en la Probeta = "+Desgaste); 

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
        //Debug.Log("Bool Primer Lijado = " + _IsFirstSanding); 

    }

    private void OnParticleCollision(GameObject other)
    {
        //Debug.Log("Juan"); 
        string tipo = other.GetComponentInParent<BotellaLab>().Tipo;
        //Debug.Log(tipo); 

        /*if (tipo == "AGris") 
        {
haveAluminaGris = true; 
            //Debug.Log("Alumina Gris"); 
        } 
        else if (tipo == "ABlanca") 
        { 
            haveAluminaBlanca = true; 
            //Debug.Log("Alumina Blanca"); 
        } 
        else */
        if (tipo == "Nital")
        {
            haveNital = true;
            //Debug.Log("Nital"); 
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //calor = false; 
        //haveAluminaBlanca = false; 
        //haveAluminaGris = false; 
        /*Debug.Log($"{other.gameObject.name}: Layer: {other.gameObject.layer}"); 
        Debug.Log(other.gameObject.layer == LayerMask.NameToLayer("ProbeLayer")); 
        Debug.Log($"{other.gameObject.layer} == {LayerMask.NameToLayer("ProbeLayer")}"); 
        if (other.gameObject.layer == LayerMask.NameToLayer("ProbeLayer")) 
        { 
            calor = true; 
            Debug.Log("Probeta Caliente"); 
        }*/

        /*
        if (other.gameObject.name == "Colision")
        {
            Debug.Log("Colision");
            calor = true;
        }

        if (other.gameObject.name == "ColisionPañoGris")
        {
            Debug.Log("ColisionGris");
            haveAluminaGris = true;
        }

        if (other.gameObject.name == "ColisionPañoBlanco")
        {
            Debug.Log("ColisionBlanca");
            haveAluminaBlanca = true;
        }*/

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.name == "Colision")
        {
            Debug.Log("Colision");
            calor = true;
        }

        if (other.gameObject.name == "ColisionPañoGris")
        {
            Debug.Log("ColisionGris");
            haveAluminaGris = true;
        }

        if (other.gameObject.name == "ColisionPañoBlanco")
        {
            Debug.Log("ColisionBlanca");
            haveAluminaBlanca = true;
        }

        if (other.gameObject.name == "CollisionRotorPulidora")
        {
            isInPulidora = true;
            Debug.Log("Is in pulidora: " + isInPulidora);    
        }
        /*if(other.GetComponent<PulidoraScript>().colliderRotor.SetActive())
        {
            isInPulidora = true;
            bodyMaterial.GetComponent<Renderer>().material.SetFloat("_Scale", 1.1f);
        }*/
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Colision")
        {
            //Debug.Log("Colision");
            calor = false;
        }

        if (other.gameObject.name == "ColisionPañoGris")
        {
           // Debug.Log("ColisionGris");
            //haveAluminaGris = false;
        }

        if (other.gameObject.name == "ColisionPañoBlanco")
        {
           // Debug.Log("ColisionBlanca");
            //haveAluminaBlanca = false;
        }
        if (other.gameObject.name == "CollisionRotorPulidora")
        {
            isInPulidora = false;
            //haveAluminaGris = false ;
            //haveAluminaBlanca = false ;
            //bodyMaterial.GetComponent<Renderer>().material.SetFloat("_Scale", 1f);
        }

    }

    public bool IsReady()
    {
        return haveAluminaBlanca && haveNital && haveAluminaGris && probeBehaviour.IsLijadoMaximo();
    }

}
