
using UdonSharp;
using UnityEngine;
using UnityEngine.UIElements;
using VRC.SDKBase;
using VRC.Udon;

public class UpdatePreview : UdonSharpBehaviour
{
    [Header("Faces Probeta")]
    public GameObject probetaShaderParent_inf;
    public GameObject probetaMirrorParent_inf;
    public GameObject probetaBorderParent_inf;

    public GameObject probetaShaderParent_sup;
    public GameObject probetaMirrorParent_sup;
    public GameObject probetaBorderParent_sup;

    [Header("Faces Canva")]
    public GameObject probetaShaderChildren;
    public GameObject probetaMirrorChildren;
    [SerializeField] private GameObject probetaShaderActual;

    [SerializeField] public BorderColor borderColor;
    [SerializeField] private bool supIsActive = false;


    [Header("RenderInfo")]
    [SerializeField] private float reflexion = 0;
    [SerializeField] private float granoLija = 0;
    [SerializeField] private int isFirstSanding = 0;
    [SerializeField] private float angleRotation = 0;

    [SerializeField] private float timer = 0f;
    [SerializeField] private bool switchFlag = false;

    private void Start()
    {
        activeSelf(probetaShaderParent_inf, probetaMirrorParent_inf);
        probetaShaderActual = probetaShaderParent_inf;
    }
    private void Update()
    {
        updateShader(probetaShaderChildren);

        if (supIsActive)
        {
            activeSelf(probetaShaderParent_sup, probetaMirrorParent_sup);
        }
        if (!supIsActive)
        {
            activeSelf(probetaShaderParent_inf, probetaMirrorParent_inf);
        }


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
            scaleAllMaterial();

            if (probetaMirrorChildren.activeSelf)
            {
                if (supIsActive) { probetaShaderParent_sup.SetActive(false); }
                if (!supIsActive) { probetaShaderParent_inf.SetActive(false); }
            }
        }
    }

    public void switchFaces_Off()
    {
        forceActiveShader(probetaShaderParent_inf, probetaMirrorParent_inf);
        scaleShader(probetaBorderParent_sup, 0f, 0);
        borderColor.bodyMaterial = probetaBorderParent_inf;
        probetaShaderActual = probetaShaderParent_inf;
        supIsActive = false;
        switchFlag = true;
        timer = 0f;
        Debug.LogWarning("[<color=blue>Change to face inferior</color>]");
    }

    public void switchFaces_On()
    {
        forceActiveShader(probetaShaderParent_sup, probetaMirrorParent_sup);
        scaleShader(probetaBorderParent_inf, 0f, 0);
        borderColor.bodyMaterial = probetaBorderParent_sup;
        probetaShaderActual = probetaShaderParent_sup;
        supIsActive = true;
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

    private void activeSelf(GameObject shader, GameObject mirror)
    {
        bool thisBool = false;
        thisBool = shader.activeSelf;
        probetaMirrorChildren.SetActive(mirror.activeSelf);
        if (probetaMirrorChildren.activeSelf)
            thisBool = false;
        probetaShaderChildren.SetActive(thisBool);
    }

    private void updateShader(GameObject shader)
    {
        getDataFaces(probetaShaderActual);
        shader.GetComponent<Renderer>().material.SetFloat("_Reflexion", reflexion);
        shader.GetComponent<Renderer>().material.SetFloat("_GranoLija", granoLija);
        shader.GetComponent<Renderer>().material.SetInt("_IsFirstSanding", isFirstSanding);
        shader.GetComponent<Renderer>().material.SetFloat("_AngleRotation", angleRotation);
    }

    private void scaleShader(GameObject shadertoScale, float scale, int posArray)
    {
        shadertoScale.GetComponent<Renderer>().materials[posArray].SetFloat("_Scale", scale);
    }

    private void forceActiveShader(GameObject shaderToActivate, GameObject mirrorToCheck)
    {
        if(!mirrorToCheck.activeSelf)
            return;
        else
            shaderToActivate.SetActive(true);
    }

    public void scaleAllMaterial()
    {
        scaleShader(probetaBorderParent_inf, 0f, 0);
        scaleShader(probetaBorderParent_sup, 0f, 0);
    }
}
