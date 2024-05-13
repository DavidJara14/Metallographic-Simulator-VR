
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ActivateMirror : UdonSharpBehaviour
{
    public GameObject probetaShader;
    public GameObject probetaMirror;
    public int vectorval; 

    public bool hasAluminaGris = false;
    public bool hasAluminaBlanca = false;

    private void Start()
    {
        
    }

    private void Update()
    {

        if (hasAluminaBlanca & !hasAluminaGris)
        {
            probetaShader.SetActive(false);
            probetaMirror.SetActive(true);
        }
        else if (hasAluminaGris & !hasAluminaBlanca)
        {
            probetaShader.SetActive(true);
            probetaMirror.SetActive(false);
        }
    }
    private void OnParticleCollision(GameObject other)
    {
        vectorval= other.GetComponent<ParticleSystem>().customData.GetVectorComponentCount(((int)ParticleSystemCustomData.Custom1));
        //vectorval = ((int)other.GetComponent<ParticleSystemCustomData>());
        Debug.Log(vectorval);

        if (vectorval == 1)
        {
            Debug.Log("Alumina Gris");
        }
        else if (vectorval == 2)
        {
            Debug.Log("Alumina Blanca");
        }
    }   
}


/*public void ToggleObject()
    {
        if (objectToToggle != null)
        {
            // Comprueba si el objeto está activo en la escena
            bool isActive = objectToToggle.activeSelf;

            // Cambia el estado del objeto
            objectToToggle.SetActive(!isActive);
        }
    }*/
