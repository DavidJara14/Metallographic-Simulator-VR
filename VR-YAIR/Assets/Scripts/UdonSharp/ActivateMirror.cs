
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

    public int caraTrabajada = 1;

    public GameObject probeBehaviour;
    public float Desgaste = 0;
    public int _IsFirstSanding = 1;


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
        else if (haveAluminaGris & haveAluminaBlanca) // Segunda etapa de pulido, TIENE ALUMINA BLANCA y ya tuvo gris
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
            Debug.Log("Mirror active");
        }
        Desgaste = probetaShader1.GetComponent<Renderer>().material.GetFloat("_GranoLija");
        Debug.Log("Desgaste en la Probeta = "+Desgaste);

        if (Desgaste == 120) 
        {
            _IsFirstSanding = 0;
            if (caraTrabajada == 1)
            {
                probetaShader1.GetComponent<Renderer>().material.SetInt("_IsFirstSanding",_IsFirstSanding);
            }
            else if (caraTrabajada == 2)
            {
                probetaShader2.GetComponent<Renderer>().material.SetInt("_IsFirstSanding", _IsFirstSanding);
            }
        }
        //Debug.Log("Bool Primer Lijado = " + _IsFirstSanding);

    }

    private void OnParticleCollision(GameObject other)
    {
        Debug.Log("Juan");
        string tipo = other.GetComponentInParent<BotellaLab>().Tipo;

        Debug.Log(tipo);

        if (tipo == "AGris")
        {
            haveAluminaGris = true;
            Debug.Log("Alumina Gris");
        }
        else if (tipo == "ABlanca")
        {
            haveAluminaBlanca = true;
            Debug.Log("Alumina Blanca");
        }
    }
}



