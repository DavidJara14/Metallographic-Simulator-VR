using UdonSharp;
using UnityEditor;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;


public class ProbeBehabiour : UdonSharpBehaviour
{

    //public float VarCount = 0f;
    private float UpdateMatTimer = 0f;
    //public string First = "";
    //public string Last = "";

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
            }
        }
        else if(EsteParticleSystem.isEmitting)
        {
            EsteParticleSystem.Stop();
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
        if (Mirror.caraTrabajada == 1)
        {
            EsteMaterial = Mirror.probetaShader1.GetComponent<Renderer>().material;
        }
        else if (Mirror.caraTrabajada == 2)
        {
            EsteMaterial = Mirror.probetaShader2.GetComponent<Renderer>().material;
        }

        Desgaste = LijaRotationActiva.Lija.GetComponent<LijaCircularBehabiour>().TamañoDeGrano;
        EsteMaterial.SetFloat("_GranoLija", Desgaste);
        EsteMaterial.SetVector("_AngleRotation", Quaternion.AngleAxis(Vector3.Angle(gameObject.transform.up, VectorDeDireccionDeDesgasteActual),gameObject.transform.forward).ToVector4());
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

