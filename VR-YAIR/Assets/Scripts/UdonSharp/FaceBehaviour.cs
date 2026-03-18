using UdonSharp;
using UnityEngine;
using UnityEngine.ProBuilder;
using VRC.SDK3.Components;
using VRC.SDKBase;

public class FaceBehaviour : UdonSharpBehaviour
{

    [Header("Probeta references")]
    public GameObject probetaShader;
    public GameObject probetaMirror;
    public ProbeBehabiour probeBehaviour;
    [SerializeField] private VRC_Pickup pickup;

    [Header("Face configuration")]
    public string positionFace = null;
    public bool newcalor = false;
    [SerializeField] private bool scaleBorderSend = false;

    [Header("Modules")]
    public PulidoAlumina pulido;
    public LimpiezaScript limpieza;
    public AtaqueQuimico ataque;

    [Header("Particle system")]
    [SerializeField] public ParticleSystem alcoholPS;
    [SerializeField] public ParticleSystem residuosAlumina;
    [SerializeField] public ParticleSystem nitalInProbePS;
    [SerializeField] public ParticleSystem probetaWaterPS;

    [Header("Mirror")]
    public VRCMirrorReflection mirror;
    public VRCMirrorReflection mirrorCanva;

    public LayerMask PCVRMask;
    public LayerMask AndroidMask;


    private void Start()
    {
        if (probetaShader != null)
            probetaShader.SetActive(true);

        if (probetaMirror != null)
            probetaMirror.SetActive(false);

        if (residuosAlumina != null)
            residuosAlumina.Stop();

        if (nitalInProbePS != null)
            nitalInProbePS.Stop();

        if (alcoholPS != null)
            alcoholPS.Stop();

        if (probetaWaterPS != null)
            probetaWaterPS.Stop();

#if UNITY_ANDROID
        mirror.m_ReflectLayers = AndroidMask;
        mirrorCanva.m_ReflectLayers = AndroidMask;
#else
        mirror.m_ReflectLayers = PCVRMask;
        mirrorCanva.m_ReflectLayers = PCVRMask;
#endif
    }

    private void Update()
    {
        if (IsReady())
        {
            residuosAlumina.Stop();
            nitalInProbePS.Stop();
            alcoholPS.Stop();
            probetaWaterPS.Stop();

            FinalState();

            if (newcalor)
            {
                gameObject.GetComponentInParent<BorderColor>().SendCustomEvent("colorGreen");
                scaleBorderSend = false;
            }

            if (!newcalor && !scaleBorderSend)
            {
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ScaleBorder");
                scaleBorderSend = true;
            }
            return;
        }


        ChangeProbetaVisual();


        if (pulido != null)
        {
            pulido.ProcessPulido();
        }

        if (pulido != null && pulido.finishedPulido1 && pulido.finishedPulido2)
        {
            if (!pulido.finishedEnjuagado)
            {
                pulido.ProcessLavado();
            }
        }

        if (pulido != null && pulido.finishedEnjuagado)
        {
            if (probetaWaterPS.isEmitting)
            {
                probetaWaterPS.Stop();
            }

            if (limpieza != null && !limpieza.finishedLimpieza)
                limpieza.ProcessLimpieza();
        }

        if (limpieza != null && limpieza.finishedLimpieza)
        {
            if (alcoholPS.isEmitting)
            {
                alcoholPS.Stop();
            }

            if (ataque != null)
                ataque.ProcessAtaque();
        }

    }


    private void ChangeProbetaVisual()
    {

        if (probeBehaviour.IsLijadoMax())
        { return; }

        if (!pulido.haveAluminaBlanca && !pulido.haveAluminaGris && !ataque.haveNital)
        {
            probetaShader.GetComponent<Renderer>().material.SetFloat("_Reflexion", 0);
        }

        float desgasteProbeta = probetaShader.GetComponent<Renderer>().material.GetFloat("_GranoLija");

        if (desgasteProbeta > 80)
        {
            probetaShader.GetComponent<Renderer>().material.SetInt("_IsFirstSanding", 0);
        }
    }


    private void FinalState()
    {
        probetaShader.SetActive(true);
        probetaMirror.SetActive(false);

        probetaShader.GetComponent<Renderer>().material.SetFloat("_Reflexion", 0.6f);
    }


    public bool IsReady()
    {
        if (pulido == null || limpieza == null || ataque == null)
            return false;

        return pulido.finishedPulido1 && pulido.finishedPulido2 && ataque.finishedAQ && probeBehaviour.IsLijadoMax();
    }

    public void ScaleBorder()
    {
        string dirProbe = probeBehaviour.gameObject.GetComponent<OrientationChecker>().checkOrientation();

        if (dirProbe == null || positionFace == null)
            return;

        if (dirProbe == positionFace)
        {
            probeBehaviour.bodyMaterial.GetComponent<Renderer>().material.SetFloat("_Scale", 0f);
        }

        else if (IsReady())
        {
            probeBehaviour.bodyMaterial.GetComponent<Renderer>().material.SetFloat("_Scale", 0f);
        }
    }
}