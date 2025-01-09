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
    //  private Color currentColor = Color.clear;
    public bool rotorIsRotating = false;
    public bool dropProbeta = false;
    public float elapsedtime = 0f;

    [UdonSynced] private float colorTimer = 0.0f;
    private bool isClear = true;
    private bool isHumedo = true;

    public string ProbeType = "";

    [Header("Audio Config")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _audioClip;
    private bool LastUserWasVR = false;

    private void Start()
    {
        _audioSource.clip = _audioClip;
        //bodyMaterial.GetComponent<Renderer>().material.SetColor("_Color", Color.clear);
    }

    private void Update()
    {

        if (LijaRotationActiva != null)
        { 
            if(LijaRotationActiva.Rotating == true) 
            {
                LijaToObjSize = new Vector3(gameObject.transform.position.x - LijaRotationActiva.Lija.transform.position.x, 0f, gameObject.transform.position.z - LijaRotationActiva.Lija.transform.position.z);
                Up = gameObject.transform.up;
                VectorDeDireccionDeDesgasteActual = Vector3.Cross(LijaToObjSize, Up);
                
                if (pickup.currentPlayer == null && _isInsideCollider)
                {
                    GetComponent<Rigidbody>().AddForce(VectorDeDireccionDeDesgasteActual.normalized * 200f);
                    Debug.Log("Probe droped, addForce");
                    
                }
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
                SetParticleRateOverTime();

                if(!EsteParticleSystem.isEmitting || !EsteParticleSystem.isPlaying)
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
        else if(EsteParticleSystem.isEmitting)
        {
            Debug.Log("StopEmitting!!!");
            EsteParticleSystem.Stop();
            _audioSource.Stop();
        }

        UpdateMaterial();
        if (UpdateMatTimer >= .1)
        {
        }
        UpdateMatTimer += Time.deltaTime;
        if(!_isInsideCollider)
            bodyMaterial.GetComponent<Renderer>().material.SetFloat("_Scale", 0.01f);
    }
    /*
    public override void OnPickupUseDown()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "RotateProbeta");
    }

    public void RotateProbeta()
    {
        gameObject.transform.Rotate(90.0f, 0.0f, 0.0f, Space.World);
    }*/

    void UpdateMaterial()
    {
        if (LijaRotationActiva == null)
            return;
        //Debug.Log(LijaRotationActiva.name);
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
                    //EsteMaterial.SetFloat("_GranoLija", Desgaste);

                    if (Mirror.caraTrabajada == 1)
                    {
                        //EsteMaterial = Mirror.probetaShader1.GetComponent<Renderer>().material;
                        probetaShader1.GetComponent<Renderer>().material.SetFloat("_GranoLija", Desgaste);
                        probetaShader1.GetComponent<Renderer>().material.SetFloat("_AngleRotation", Quaternion.AngleAxis(Vector3.Angle(gameObject.transform.up, VectorDeDireccionDeDesgasteActual), gameObject.transform.forward).eulerAngles.z);

                    }
                    else if (Mirror.caraTrabajada == 2)
                    {
                        //EsteMaterial = Mirror.probetaShader2.GetComponent<Renderer>().material;
                        probetaShader2.GetComponent<Renderer>().material.SetFloat("_GranoLija", Desgaste);
                        probetaShader2.GetComponent<Renderer>().material.SetFloat("_AngleRotation", Quaternion.AngleAxis(Vector3.Angle(gameObject.transform.up, VectorDeDireccionDeDesgasteActual), gameObject.transform.forward).eulerAngles.z);
                    }

                    //              float hue = Mathf.Repeat(Time.time , 1.0f); 
                    //            Color newColor = Color.HSVToRGB(hue, 1.0f, 1.0f); 

                    //          currentColor = Color.Lerp(currentColor, newColor, Time.deltaTime);

                    bodyMaterial.GetComponent<Renderer>().material.SetFloat("_Scale", 1.1f);


                    colorTimer += Time.deltaTime;

                    if (colorTimer >= 0.1F)
                    {
                        if (isClear)
                        {
                            bodyMaterial.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
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
                else if(_insideColliderTimer<10.0f && _isInsideCollider)
                {
                    bodyMaterial.GetComponent<Renderer>().material.SetFloat("_Scale", 0.01f);
                }

            }
        }
        else if ((!isHumedo || !canLijar) && _isInsideCollider)
        {
            bodyMaterial.GetComponent<Renderer>().material.SetFloat("_Scale", 1.1f);
            colorTimer += Time.deltaTime;

            if (colorTimer >= 0.1F)
            {
                if (isClear)
                {
                    bodyMaterial.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
                    isClear = false;
                }
                else
                {
                    bodyMaterial.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
                    isClear = true;
                }
                colorTimer = 0f;
            }
            //Debug.Log("Ponme agua");
        }
        else
        {
            //StopTimer();
            bodyMaterial.GetComponent<Renderer>().material.SetFloat("_Scale", 0.01f);
            //bodyMaterial.GetComponent<Renderer>().material.SetColor("_Color", Color.clear);
        }

        //Debug.Log("TamanioLija " + Desgaste);
        //Debug.Log("Desgaste en la Probeta (ProbeBehaviour) = " + probetaShader1.GetComponent<Renderer>().material.GetFloat("_GranoLija"));

        //EsteMaterial.SetFloat("_AngleRotation", Quaternion.AngleAxis(Vector3.Angle(gameObject.transform.up, VectorDeDireccionDeDesgasteActual),gameObject.transform.forward).eulerAngles.z);
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

    public override void OnPickup()
    {
        LastUserWasVR = pickup.currentPlayer.IsUserInVR();
        CheckInteractProve();
        dropProbeta = false;
        Debug.Log("Probeta drop is: " + dropProbeta);

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

        if(other.GetComponent<InteractProbe>() != null)
        {
            return;
        }

        //Debug.Log(other + "in Probebehabiour");
        if (other.GetComponent<LijaRotation>() != null)
        {
            Debug.Log("Lijarotation in Provebehabiour");
            LijaRotationActivaGO = other.gameObject;
            LijaRotationActiva = LijaRotationActivaGO.GetComponent<LijaRotation>();
            interactProbe.DisableCanva();
            interactProbe.gameObject.SetActive(false);
        }

        /*if (other.gameObject.name == "colliderRotor" other.GetComponent<colliderRotorBehabiour>())
        {
             _isInsideCollider = other.GetComponent<LijaRotation>().GetComponentInChildren<BoxCollider>().enabled;
            _isInsideCollider = other.GetComponent<colliderRotorBehabiour>().colliderRotor();
            Debug.Log("Detecte collider rotor :"+_isInsideCollider);
        }*/

        //      if (other.GetComponent<LijaCircularBehabiour>() != null /*&& other.GetComponent<LijaRotation>().Lija != null*/)
        //{
        //        _isInsideCollider = true;
        //      if (other.GetComponent<LijaCircularBehabiour>().GetHumedad() > 0)
        //    {
        //      isHumedo = true;
        //                bodyMaterial.GetComponent<Renderer>().material.SetFloat("_Scale", 0.01f);
        //                bodyMaterial.GetComponent<Renderer>().material.SetColor("_Color", Color.clear);
        //}
        // else
        // {
        //    isHumedo = false;
        //                bodyMaterial.GetComponent<Renderer>().material.SetFloat("_Scale", 1.1f);
        //}
        //  Debug.Log("Probeta Enter, humedo is: "+isHumedo);
        //}
        //rotorRotating = other.GetComponent<LijaRotation>().Rotating;
    }

    private void OnTriggerExit(Collider other)
    {

        Debug.Log("OnTriggerExitCall: " + other.name);

        if(other.GetComponent<LijaRotation>())
        {
            //ResetVariables();
            LijaRotationActivaGO = null;
            LijaRotationActiva = null;
            CheckInteractProve();
        }

        if (other.GetComponent<LijaCircularBehabiour>() != null)
        {
            //_isInsideCollider = false;
            //Debug.Log("Inside lija: "+_isInsideCollider);
//            bodyMaterial.GetComponent<Renderer>().material.SetFloat("_Scale", 0.01f);
//            bodyMaterial.GetComponent<Renderer>().material.SetColor("_Color", Color.clear);
        }

        /*if (other.gameObject.name == "colliderRotor")
        {
            _isInsideCollider = false;
            Debug.Log("Inside lija: " + _isInsideCollider);
        }*/
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
                //StopTimer();
            }
            if (!other.GetComponent<LijaRotation>().Rotating)
            {
                rotorIsRotating = false;
                _isInsideCollider = false;
                //Debug.Log("Change _isInsideCollider to: " + _isInsideCollider + "Rontating change to: " + other.GetComponent<LijaRotation>().Rotating);
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
        //Debug.Log("collider: "+other.gameObject.name);

        /*else if(other.gameObject.name == "colliderRotor")
        {
            _isInsideCollider = false;
            Debug.Log("Detecte collider rotor false");
        }*/

        if (other.GetComponent<LijaCircularBehabiour>() != null)
        {

            if (other.GetComponent<LijaCircularBehabiour>().GetHumedad() > 0)
            {
                isHumedo = true;
                //                bodyMaterial.GetComponent<Renderer>().material.SetFloat("_Scale", 0.01f);
                //                bodyMaterial.GetComponent<Renderer>().material.SetColor("_Color", Color.clear);
            }
            else
            {
                isHumedo = false;
                //                bodyMaterial.GetComponent<Renderer>().material.SetFloat("_Scale", 1.1f);
            }
            //Debug.Log("Probeta Enter, humedo is: " + isHumedo);
        }

        //if(/*other.gameObject.name == "colliderRotor"*/ other.GetComponent<colliderRotorBehabiour>())
        //{
        //    _isInsideCollider = other.GetComponent<LijaRotation>().GetComponentInChildren<BoxCollider>().enabled;
          //  _isInsideCollider = other.GetComponent<colliderRotorBehabiour>().colliderRotor();
            //Debug.Log("Detecte collider rotor :"+_isInsideCollider);
        //}

        if(other.gameObject.name == "Rotor" && other.GetComponent<LijaRotation>().Rotating)
        {
            rotorIsRotating = other.GetComponent<LijaRotation>().Rotating;
            _isInsideCollider = true;
            //Debug.Log("Change _isInsideCollider to: " + _isInsideCollider + "Rotating = " + rotorIsRotating);
        }

        if(other.GetComponent<LijaRotation>() != null)
            rotorIsRotating = other.GetComponent<LijaRotation>().Rotating;
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

    private bool IsLijadoMaximo()
    { 
        return (int)Desgaste == (int)DesgasteMax;
    }

    public bool IsLijadoMax()
    {
        return (int)Desgaste == 800;
    }

    public override void OnDrop()
    {
        dropProbeta = true;
        Debug.Log("Probeta drop is: " + dropProbeta);

    }

}

