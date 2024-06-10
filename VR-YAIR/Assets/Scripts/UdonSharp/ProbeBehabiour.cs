using UdonSharp;
using UnityEditor;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;


public class ProbeBehabiour : UdonSharpBehaviour
{

    private float timer = 0f;

    const float DesgasteMin = 0f;
    const float DesgasteMax = 1f;
    [SerializeField][Range(DesgasteMin, DesgasteMax)] private float Desgaste; //0 a 1

    [SerializeField] public Vector3 LijaToObjSize;
    [SerializeField] public Vector3 Down;
    [SerializeField] public Vector3 VectorDeDireccionDeDesgasteActual;

    public GameObject CaraAModificar;
    public Material EsteMaterial;
    [SerializeField] ParticleSystem EsteParticleSystem;
    public GameObject LijaRotationActivaGO;
    public LijaRotation LijaRotationActiva;

    private void Start()
    {
        EsteMaterial = CaraAModificar.GetComponent<MeshRenderer>().material;
    }

    private void Update()
    { 
        if(LijaRotationActiva != null)
        { 
            if(LijaRotationActiva.Rotating == true) 
            {
                LijaToObjSize = new Vector3(gameObject.transform.position.x - LijaRotationActiva.Lija.transform.position.x, 0f, gameObject.transform.position.z - LijaRotationActiva.Lija.transform.position.z);
                Down = -gameObject.transform.up;
                VectorDeDireccionDeDesgasteActual = Vector3.Cross(LijaToObjSize, Down);
            }
        }
        else if(LijaRotationActivaGO != null)
        {
            LijaRotationActiva = LijaRotationActivaGO.GetComponent<LijaRotation>();
        }
        if(LijaRotationActiva != null)
        {
            if (LijaRotationActiva.Rotating)
            {
                EsteParticleSystem.Play();
                EsteParticleSystem.gameObject.transform.LookAt(gameObject.transform.position + VectorDeDireccionDeDesgasteActual);
            }
        }
        else if(EsteParticleSystem.isEmitting)
        {
            EsteParticleSystem.Stop();
        }
        if (timer >= .1)
        {
            UpdateMaterial();
        }
        timer += Time.deltaTime;
    }

    void UpdateMaterial()
    {
        if(LijaRotationActiva != null)
            Desgaste = LijaRotationActiva.Lija.GetComponent<LijaCircularBehabiour>().TamañoDeGrano;
        EsteMaterial.SetFloat("_GranoLija", Desgaste);
        EsteMaterial.SetFloat("_AngleRotation", 
            Quaternion.AngleAxis(
                Vector3.Angle(
                    gameObject.transform.up, 
                    VectorDeDireccionDeDesgasteActual),
                gameObject.transform.forward).eulerAngles.x);
    }

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
                Gizmos.DrawLine(gameObject.transform.position, gameObject.transform.position + Down);
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(gameObject.transform.position, gameObject.transform.position + VectorDeDireccionDeDesgasteActual);
            }
        }
        Gizmos.color = Color.white;
        //Gizmos.DrawLine();
    }

}

