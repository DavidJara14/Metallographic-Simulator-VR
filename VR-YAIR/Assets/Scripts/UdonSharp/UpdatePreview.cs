
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class UpdatePreview : UdonSharpBehaviour
{
    public GameObject probetaShaderParent;
    public GameObject probetaMirrorParent;

    public GameObject probetaShaderChildren;
    public GameObject probetaMirrorChildren;

    private void Start()
    {
            
    }

    private void Update()
    {
        probetaShaderChildren.SetActive(probetaShaderParent.activeSelf);
        probetaMirrorChildren.SetActive(probetaMirrorParent.activeSelf);

        probetaShaderChildren.GetComponent<Renderer>().material.SetFloat("_Reflexion", probetaShaderParent.GetComponent<Renderer>().material.GetFloat("_Reflexion"));
        probetaShaderChildren.GetComponent<Renderer>().material.SetFloat("_GranoLija", probetaShaderParent.GetComponent<Renderer>().material.GetFloat("_GranoLija"));
        probetaShaderChildren.GetComponent<Renderer>().material.SetInt("_IsFirstSanding", probetaShaderParent.GetComponent<Renderer>().material.GetInt("_IsFirstSanding"));
        probetaShaderChildren.GetComponent<Renderer>().material.SetFloat("_AngleRotation", probetaShaderParent.GetComponent<Renderer>().material.GetFloat("_AngleRotation"));
    }
}
