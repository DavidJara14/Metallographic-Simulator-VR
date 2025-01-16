using System;
using UdonSharp;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDKBase;
using VRC.Udon;


public class ProbeBehabiour : UdonSharpBehaviour
{
    const float DesgasteMin = 0f;
    const float DesgasteMax = 801f;

    const int ParticleRateMin = 10;
    const int ParticleRateMax = 50;

    [SerializeField][Range(DesgasteMin, DesgasteMax)] private float Desgaste; //0 a Lija

    [SerializeField] public Vector3 LijaToObjSize;
    [SerializeField] public Vector3 Up;
    [SerializeField] public Vector3 VectorDeDireccionDeDesgasteActual;

    Material EsteMaterial;
    [SerializeField] ParticleSystem EsteParticleSystem;
    [SerializeField] GameObject LijaRotationActivaGO;
    [SerializeField] LijaRotation LijaRotationActiva;
    [SerializeField] ActivateMirror Mirror;

    [SerializeField] private VRC_Pickup pickup;
    [SerializeField] private InteractProbe interactProbe;
    private DataList TamañosDeLija = new DataList()
    {
        { 120 },
        { 180 },
        { 240 },
        { 360 },
        { 400 },
        { 500 },
        { 600 },
        { 800 },
        { 9000 },
        { 9001 },
    };


    public GameObject probetaShader1;
    public GameObject probetaShader2;
    public float _insideColliderTimer = 0f;
    public bool _isInsideCollider = false;
    public GameObject bodyMaterial;
    public bool canLijar = false;

    private float colorTimer = 0.0f;
    private bool isClear = true;
    private bool isHumedo = true;

    public string ProbeType = "";

    [Header("Audio Config")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _audioClip;
    private bool LastUserWasVR = false;

    private float hapticDuration = 0.05f;
    private float hapticAmplitude = 0.5f;
    private float hapticFrequency = 200f;


    private void Start()
    {
        _audioSource.clip = _audioClip;
    }

    private void Update()
    {

        if (LijaRotationActiva != null)
        { 
            if(LijaRotationActiva.Rotating) 
            {
                LijaToObjSize = new Vector3(gameObject.transform.position.x - LijaRotationActiva.Lija.transform.position.x, 0f, gameObject.transform.position.z - LijaRotationActiva.Lija.transform.position.z);
                Up = gameObject.transform.up;
                VectorDeDireccionDeDesgasteActual = Vector3.Cross(LijaToObjSize, Up);

                if (_isInsideCollider)
                {
                    if (pickup.currentPlayer == null)
                    {
                        GetComponent<Rigidbody>().AddForce(VectorDeDireccionDeDesgasteActual.normalized * 200f);
                        Debug.Log("Probe droped, addForce");
                    }

                    if (pickup.currentPlayer != null && Networking.IsOwner(Networking.LocalPlayer, this.gameObject))
                    {
                        Networking.LocalPlayer.PlayHapticEventInHand(pickup.currentHand, hapticDuration, hapticAmplitude * (800f / Desgaste)*2, hapticFrequency);
                        Debug.Log("Haptic Feedback!!!!!!!!!!!!");
                    }
                }

                SetParticleRateOverTime();

                if (!EsteParticleSystem.isEmitting || !EsteParticleSystem.isPlaying)
                {
                    EsteParticleSystem.Play();
                }
                EsteParticleSystem.gameObject.transform.rotation = Quaternion.FromToRotation(EsteParticleSystem.transform.rotation.eulerAngles, VectorDeDireccionDeDesgasteActual);

                if (!_audioSource.isPlaying)
                {
                    _audioSource.Play();
                }
            }
            else
            {
                EsteParticleSystem.Stop();
                if (_audioSource.isPlaying)
                {
                    _audioSource.Stop();
                }
            }
        }

        else if(LijaRotationActivaGO != null)
        {
            LijaRotationActiva = LijaRotationActivaGO.GetComponent<LijaRotation>();
        }

        else if(EsteParticleSystem.isEmitting)
        {
            Debug.Log("StopEmitting!!!");
            EsteParticleSystem.Stop();
            _audioSource.Stop();
        }

        UpdateMaterial();

        if(!_isInsideCollider)
            bodyMaterial.GetComponent<Renderer>().material.SetFloat("_Scale", 0.01f);
    }

    void UpdateMaterial()
    {
        if (LijaRotationActiva == null)
            return;
        if (LijaRotationActiva.Lija == null)
            return;

        TryChangeDesgaste();

        if (_isInsideCollider && isHumedo && !IsLijadoMaximo() && canLijar)
        {
            if (LijaRotationActiva.Rotating)
            {
                _insideColliderTimer += Time.deltaTime;
                Debug.Log("Time lija: " + _insideColliderTimer);
                if (_insideColliderTimer >= 10f || probetaShader1.GetComponent<Renderer>().material.GetFloat("_GranoLija") == Desgaste || probetaShader2.GetComponent<Renderer>().material.GetFloat("_GranoLija") == Desgaste)
                {
                    if (Mirror.caraTrabajada == 1)
                    {
                        probetaShader1.GetComponent<Renderer>().material.SetFloat("_GranoLija", Desgaste);
                        probetaShader1.GetComponent<Renderer>().material.SetFloat("_AngleRotation", Quaternion.AngleAxis(Vector3.Angle(gameObject.transform.up, VectorDeDireccionDeDesgasteActual), gameObject.transform.forward).eulerAngles.z);

                    }
                    else if (Mirror.caraTrabajada == 2)
                    {
                        probetaShader2.GetComponent<Renderer>().material.SetFloat("_GranoLija", Desgaste);
                        probetaShader2.GetComponent<Renderer>().material.SetFloat("_AngleRotation", Quaternion.AngleAxis(Vector3.Angle(gameObject.transform.up, VectorDeDireccionDeDesgasteActual), gameObject.transform.forward).eulerAngles.z);
                    }

                    changeColor(true);

                }
                else if(_insideColliderTimer<10.0f && _isInsideCollider)
                {
                    bodyMaterial.GetComponent<Renderer>().material.SetFloat("_Scale", 0.01f);
                }

            }
        }
        else if ((!isHumedo || !canLijar) && _isInsideCollider)
        {
            changeColor(false);
        }
        else
        {
            bodyMaterial.GetComponent<Renderer>().material.SetFloat("_Scale", 0.01f);
        }
   }

    private void SetParticleRateOverTime()
    {
        var emisionModule = EsteParticleSystem.emission;
        var waterValue = LijaRotationActiva.Lija.GetComponent<LijaCircularBehabiour>().GetHumedad();
        emisionModule.rateOverTimeMultiplier = Mathf.Clamp(Mathf.Exp(-(waterValue / 3.03f)+7) + 10f, ParticleRateMin, ParticleRateMax);
    }

    private void TryChangeDesgaste()// 120 a 800
    {
        var TamañoDeGranoEnLija = LijaRotationActiva.Lija.GetComponent<LijaCircularBehabiour>().TamañoDeGrano;
        if (LijaRotationActiva == null)
            return;
        if (!LijaRotationActiva.Rotating)
            return;
        if (Desgaste == TamañoDeGranoEnLija)
            return;
        if (Desgaste > TamañoDeGranoEnLija)
        {
            Debug.Log($"Cambio por bajar: {Desgaste} a {TamañoDeGranoEnLija}");
            Desgaste = TamañoDeGranoEnLija;
            canLijar = false;
        }
        else
        {
            var IndexDesgasteEnProbeta = TamañosDeLija.BinarySearch(new DataToken(Desgaste));
            var IndexTamañoDeGranoEnLija = TamañosDeLija.BinarySearch(new DataToken(TamañoDeGranoEnLija));
            if (IndexDesgasteEnProbeta < IndexTamañoDeGranoEnLija
                && (IndexDesgasteEnProbeta + 1 == IndexTamañoDeGranoEnLija || IndexDesgasteEnProbeta + 2 == IndexTamañoDeGranoEnLija))
            {
                Debug.Log($"Cambio por seguir: {Desgaste} a {TamañoDeGranoEnLija}");
                Desgaste = TamañoDeGranoEnLija;
                canLijar = true;
            }
            else
            {
                Debug.Log($"Intento de salto de lija: {Desgaste} a {TamañoDeGranoEnLija}");
                canLijar = false;
            }
        }
        Debug.Log("Puedo lijar?: " + canLijar);
    }

    public string getProbeType()
    {
        return ProbeType;
    }

    public override void OnPickup()
    {
        LastUserWasVR = pickup.currentPlayer.IsUserInVR();
        CheckInteractProve();
    }

    private void CheckInteractProve()
    {
        if (!LastUserWasVR)
        {
            interactProbe.gameObject.SetActive(true);
        }
        else
        {
            interactProbe.gameObject.SetActive(false);
            interactProbe.DisableCanva();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<InteractProbe>() != null)
        {
            return;
        }

        if (other.GetComponent<LijaRotation>() != null)
        {
            Debug.Log("Lijarotation in Provebehabiour");
            LijaRotationActivaGO = other.gameObject;
            LijaRotationActiva = LijaRotationActivaGO.GetComponent<LijaRotation>();
            interactProbe.DisableCanva();
            interactProbe.gameObject.SetActive(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {

        //Debug.Log("OnTriggerExitCall: " + other.name);

        if(other.GetComponent<LijaRotation>())
        {
            LijaRotationActivaGO = null;
            LijaRotationActiva = null;
            CheckInteractProve();
        }

        bodyMaterial.GetComponent<Renderer>().material.SetFloat("_Scale", 0.01f);

        if(other.gameObject.name == "Rotor")
        {
            if (pickup.currentPlayer == null)
            {
                Debug.Log("Stop no current player");
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "StopTimer");
            }
            else if (Networking.IsOwner(Networking.LocalPlayer, this.gameObject))
            {
                Debug.Log("Stop Owner");
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "StopTimer");
            }
            if (!other.GetComponent<LijaRotation>().Rotating)
            {
                _isInsideCollider = false;
                Debug.Log("_isInsideCollider  = " + _isInsideCollider);

            }
        }
    }

    public void StartTimer()
    {
        _isInsideCollider = true;
    }

    public void StopTimer()
    {
        _insideColliderTimer = 0f;
        _isInsideCollider = false;
        Debug.Log("ResetTimer to 0 ");
    }

    private void OnTriggerStay(Collider other)
    {
        //Debug.Log("OnTriggerStayCall: " + other.name);

        if (other.GetComponent<LijaCircularBehabiour>() != null)
        {

            if (other.GetComponent<LijaCircularBehabiour>().GetHumedad() > 0)
            {
                isHumedo = true;
            }
            else
            {
                isHumedo = false;
            }
        }

        if(pickup.currentPlayer != null)
        {
            if(other.gameObject.name == "Rotor" && other.GetComponent<LijaRotation>().Rotating)
            {
                _isInsideCollider = true;
                //Debug.Log("_isInsideCollider  = " + _isInsideCollider);
            }
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
    }

    private bool IsLijadoMaximo()
    { 
        return (int)Desgaste == (int)DesgasteMax;
    }

    public bool IsLijadoMax()
    {
        return (int)Desgaste == 800;
    }

    private void changeColor(bool isGreen)
    {
        bodyMaterial.GetComponent<Renderer>().material.SetFloat("_Scale", 1.1f);
        colorTimer += Time.deltaTime;
        if (colorTimer >= 0.1f)
        {
            if (isClear)
            {
                if(isGreen)
                    bodyMaterial.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
                else
                    bodyMaterial.GetComponent<Renderer>().material.SetColor("_Color", Color.red);

                isClear = false;
                Debug.Log("Desgaste set to: " + Desgaste);
            }
            else
            {
                bodyMaterial.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
                isClear = true;
            }
            colorTimer = 0f;
        }
    }

}

