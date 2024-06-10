
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ActivateMirror : UdonSharpBehaviour
{
    public GameObject probetaShader;
    public GameObject probetaMirror;

    public bool hasAluminaGris = false;
    public bool hasAluminaBlanca = false;

    private void Start()
    {
        probetaShader.SetActive(true);
        probetaMirror.SetActive(false);

    }

    private void Update()
    {
        // Alumina gris -> Para efectos practicos le dara brillo
        // Alumina blanca ->´Para efectos practicos le dara acabado espejo

        if (!hasAluminaBlanca & !hasAluminaGris) // Hasta esta etapa solo se ha lijado
        {
            //probetaShader.SetActive(true);
            //probetaMirror.SetActive(false);
            probetaShader.GetComponent<Renderer>().material.SetFloat("Reflexion", 0);
            Debug.Log("reflexion 0");
        }
        else if (hasAluminaGris & !hasAluminaBlanca) // Primera etapa de pulido, TIENE ALUMINA GRIS
        {
            //probetaShader.SetActive(true);
            //probetaMirror.SetActive(false);
            probetaShader.GetComponent<Renderer>().material.SetFloat("_Reflexion", 1);

            Debug.Log("reflexion 1");

        }
        else if (!hasAluminaGris & hasAluminaBlanca) // Segunda etapa de pulido, TIENE ALUMINA BLANCA
        {
            probetaShader.SetActive(false);
            probetaMirror.SetActive(true);
            Debug.Log("Mirror active");

        }
    }

    private void OnParticleCollision(GameObject other)
    {
        string tipo = other.GetComponentInParent<BotellaLab>().Tipo;

        Debug.Log(tipo);

        if (tipo == "AGris")
        {
            hasAluminaGris = true;
            hasAluminaBlanca = false;

            Debug.Log("Alumina Gris");
        }
        else if (tipo == "ABlanca")
        {
            hasAluminaGris = false;
            hasAluminaBlanca = true;
            Debug.Log("Alumina Blanca");
        }
    }
}



