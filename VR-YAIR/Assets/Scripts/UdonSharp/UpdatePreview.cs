
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class UpdatePreview : UdonSharpBehaviour
{
    [Header("Faces Probeta")]
    public GameObject probetaShaderParent_inf;
    public GameObject probetaMirrorParent_inf;
    public GameObject probetaShaderParent_sup;
    public GameObject probetaMirrorParent_sup;
    
    [Header("Faces Canva")]
    public GameObject probetaShaderChildren;
    public GameObject probetaMirrorChildren;


    [SerializeField] public BorderColor borderColor;


    [Header("RenderInfo")]

    [SerializeField] private float reflexion = 0;
    [SerializeField] private float granoLija = 0;
    [SerializeField] private int isFirstSanding = 0;
    [SerializeField] private float angleRotation = 0;

    [SerializeField] private float timer = 0f;
    [SerializeField] private bool switchFlag = false;

    private void Start()
    {
        probetaShaderChildren.SetActive(probetaShaderParent_inf.activeSelf);
        probetaMirrorChildren.SetActive(probetaMirrorParent_inf.activeSelf);
    }
    private void Update()
    {
        probetaShaderChildren.GetComponent<Renderer>().material.SetFloat("_Reflexion", reflexion);
        probetaShaderChildren.GetComponent<Renderer>().material.SetFloat("_GranoLija", granoLija);
        probetaShaderChildren.GetComponent<Renderer>().material.SetInt("_IsFirstSanding", isFirstSanding);
        probetaShaderChildren.GetComponent<Renderer>().material.SetFloat("_AngleRotation", angleRotation);

        if (switchFlag && timer<1.5f)
        {
            borderColor.SendCustomEvent("colorYellow");
            timer += Time.deltaTime;
        }

        if(timer > 1f)
        {
            switchFlag = false;
            timer = 0f;
        }

        if(!switchFlag)
        {
            probetaShaderParent_inf.GetComponent<Renderer>().materials[1].SetFloat("_Scale", 0f);
            probetaShaderParent_sup.GetComponent<Renderer>().materials[1].SetFloat("_Scale", 0f);
        }
    }

    public void switchFaces_Off()
    {
        probetaShaderChildren.SetActive(probetaShaderParent_inf.activeSelf);
        probetaMirrorChildren.SetActive(probetaMirrorParent_inf.activeSelf);
        probetaShaderParent_sup.GetComponent<Renderer>().materials[1].SetFloat("_Scale", 0f);
        borderColor.bodyMaterial = probetaShaderParent_inf;
        getDataFaces(probetaShaderParent_inf);
        switchFlag = true;
        timer = 0f;
        Debug.LogWarning("[<color=blue>Change to face inferior</color>]");
    }

    public void switchFaces_On()
    {
        probetaShaderChildren.SetActive(probetaShaderParent_sup.activeSelf);
        probetaMirrorChildren.SetActive(probetaMirrorParent_sup.activeSelf);
        probetaShaderParent_inf.GetComponent<Renderer>().materials[1].SetFloat("_Scale", 0f);
        borderColor.bodyMaterial = probetaShaderParent_sup;
        getDataFaces(probetaShaderParent_sup);
        switchFlag = true;
        timer = 0f;
        Debug.LogWarning("[<color=blue>Change to face superior</color>]");
    }

    private void getDataFaces(GameObject thisFace)
    {
        reflexion = thisFace.GetComponent<Renderer>().material.GetFloat("_Reflexion");
        granoLija = thisFace.GetComponent<Renderer>().material.GetFloat("_GranoLija");
        isFirstSanding = thisFace.GetComponent<Renderer>().material.GetInt("_IsFirstSanding");
        angleRotation = thisFace.GetComponent<Renderer>().material.GetFloat("_AngleRotation");
    }
}
