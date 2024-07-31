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

    public GameObject probeBehaviour;
    public float Desgaste = 0;
    public bool _IsFirstSanding = true;
    public bool calor = false;

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

        if (!haveAluminaBlanca & !haveAluminaGris) // Hasta esta etapa solo se ha lijado 
        {
            if (caraTrabajada == 1)
            {
                probetaShader1.GetComponent<Renderer>().material.SetFloat("_Reflexion", 0);
            }

            else if (caraTrabajada == 2)
            {
                probetaShader2.GetComponent<Renderer>().material.SetFloat("_Reflexion", 0);
            }
        }
        else if (haveAluminaGris & !haveAluminaBlanca) // Primera etapa de pulido, TIENE ALUMINA GRIS 
        {
            if (caraTrabajada == 1)
            {
                probetaShader1.GetComponent<Renderer>().material.SetFloat("_Reflexion", 1);
            }

            else if (caraTrabajada == 2)
            {
                probetaShader2.GetComponent<Renderer>().material.SetFloat("_Reflexion", 1);
            }
        }
        else if (haveAluminaGris & haveAluminaBlanca & !haveNital) // Segunda etapa de pulido, TIENE ALUMINA BLANCA y ya tuvo gris, sin nital 
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
        }
        else if (haveAluminaGris & haveAluminaBlanca & haveNital & calor) // Segunda etapa de pulido, TIENE / TUVO NITAL, tuvo alumina blanca y ya tuvo gris 
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

    }

    public bool IsReady()
    {
        return haveAluminaBlanca && haveNital && haveAluminaGris;
    }

}
