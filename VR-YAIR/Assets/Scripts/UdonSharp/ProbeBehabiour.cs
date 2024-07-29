using System;
using UdonSharp;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDKBase;
using VRC.Udon;


public class ProbeBehabiour : UdonSharpBehaviour
{

    private float UpdateMatTimer = 0f;

    const float DesgasteMin = 0f;
    const float DesgasteMax = 1f;
    [SerializeField][Range(DesgasteMin, DesgasteMax)] private float Desgaste; //0 a Lija

    [SerializeField] public Vector3 LijaToObjSize;
    [SerializeField] public Vector3 Up;
    [SerializeField] public Vector3 VectorDeDireccionDeDesgasteActual;

    Material EsteMaterial;
    [SerializeField] ParticleSystem EsteParticleSystem;
    [SerializeField] GameObject LijaRotationActivaGO;
    [SerializeField] LijaRotation LijaRotationActiva;
    [SerializeField] ActivateMirror Mirror;

    public GameObject probetaShader1;
    public GameObject probetaShader2;


    public string ProbeType = "";

    [Header("Audio Config")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _audioClip;

    private void Start()
    {
        _audioSource.clip = _audioClip;
    }

    private void Update()
    { 
        if(LijaRotationActiva != null)
        { 
            if(LijaRotationActiva.Rotating == true) 
            {
                LijaToObjSize = new Vector3(gameObject.transform.position.x - LijaRotationActiva.Lija.transform.position.x, 0f, gameObject.transform.position.z - LijaRotationActiva.Lija.transform.position.z);
                Up = gameObject.transform.up;
                VectorDeDireccionDeDesgasteActual = Vector3.Cross(LijaToObjSize, Up);
            }
        }
        else if(LijaRotationActivaGO != null)
        {
            LijaRotationActiva = LijaRotationActivaGO.GetComponent<LijaRotation>();
        }
        if(/*First != "" && */LijaRotationActiva != null)
        {
            if(LijaRotationActiva.Rotating)
            {
                EsteParticleSystem.Play();
                EsteParticleSystem.gameObject.transform.rotation = Quaternion.FromToRotation(EsteParticleSystem.transform.rotation.eulerAngles, VectorDeDireccionDeDesgasteActual);
                if (!_audioSource.isPlaying)
                {
                    _audioSource.Play();
                }
            }
        }
        else if(EsteParticleSystem.isEmitting)
        {
            EsteParticleSystem.Stop();
            _audioSource.Stop();
        }
        if (UpdateMatTimer >= .1)
        {
            UpdateMaterial();
        }
        UpdateMatTimer += Time.deltaTime;
    }

    void UpdateMaterial()
    {
        if (LijaRotationActiva == null)
            return;
        Debug.Log(LijaRotationActiva.name);
        if (LijaRotationActiva.Lija == null)
            return;
        Desgaste = LijaRotationActiva.Lija.GetComponent<LijaCircularBehabiour>().TamañoDeGrano;
        //EsteMaterial.SetFloat("_GranoLija", Desgaste);
        if (Mirror.caraTrabajada == 1)
        {
            EsteMaterial = Mirror.probetaShader1.GetComponent<Renderer>().material;
            probetaShader1.GetComponent<Renderer>().material.SetFloat("_GranoLija", Desgaste);
            probetaShader1.GetComponent<Renderer>().material.SetFloat("_AngleRotation", Quaternion.AngleAxis(Vector3.Angle(gameObject.transform.up, VectorDeDireccionDeDesgasteActual), gameObject.transform.forward).eulerAngles.z);

        }
        else if (Mirror.caraTrabajada == 2)
        {
            EsteMaterial = Mirror.probetaShader2.GetComponent<Renderer>().material;
            probetaShader2.GetComponent<Renderer>().material.SetFloat("_GranoLija", Desgaste);
            probetaShader2.GetComponent<Renderer>().material.SetFloat("_AngleRotation", Quaternion.AngleAxis(Vector3.Angle(gameObject.transform.up, VectorDeDireccionDeDesgasteActual), gameObject.transform.forward).eulerAngles.z);
        }

        

        Debug.Log("TamanioLija "+Desgaste);
        //EsteMaterial.SetFloat("_AngleRotation", Quaternion.AngleAxis(Vector3.Angle(gameObject.transform.up, VectorDeDireccionDeDesgasteActual),gameObject.transform.forward).eulerAngles.z);
    }

    public string getProbeType()
    {
        return ProbeType;
    }

    //public void SetVar(string name, bool value)
    //{
    //    switch (name)
    //    {
    //        case "UpTouched":
    //            //UpTouched = value;
    //            break;
    //        case "DownTouched":
    //            //DownTouched = value;
    //            break;
    //        case "LeftTouched":
    //            //LeftTouched = value;
    //            break;
    //        case "RightTouched":
    //            //RightTouched = value;
    //            break;
    //        default:
    //            Debug.Log(string.Format("error in direction, {0} not identified", First));
    //            break;
    //    }
    //}

    //public void ResetVariables()
    //{
    //    VarCount = 0f;
    //    First = "";
    //    Last = "";
    //}

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other + "in Probebehabiour");
        if (other.GetComponent<LijaRotation>() != null)
        {
            Debug.Log("Lijarotation in Provebehabiour");
            LijaRotationActivaGO = other.gameObject;
            LijaRotationActiva = LijaRotationActivaGO.GetComponent<LijaRotation>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.GetComponent<LijaRotation>())
        {
            //ResetVariables();
            LijaRotationActivaGO = null;
            LijaRotationActiva = null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (LijaRotationActiva != null)
        {
            if (LijaRotationActiva.Lija != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(LijaRotationActiva.Lija.transform.position, LijaRotationActiva.Lija.transform.position + LijaToObjSize);
                Gizmos.color = Color.green;
                Gizmos.DrawLine(gameObject.transform.position, gameObject.transform.position + Up);
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(gameObject.transform.position, gameObject.transform.position + VectorDeDireccionDeDesgasteActual);
            }
        }
        Gizmos.color = Color.white;
        //Gizmos.DrawLine();
    }

}

