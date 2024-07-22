using System;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.Core;
using VRC.SDK3.Data;
using VRC.SDK3.Image;
using VRC.SDKBase;
using VRC.Udon;
using static UnityEngine.Rendering.HableCurve;

public class MicroscopeBehabiour : UdonSharpBehaviour
{

    [SerializeField] private Image[] CompImage;
    [SerializeField] private MicroscopeElements ReferenceGOComponent;
    [SerializeField] private GameObject CanvasGO;
    private ProbeBehabiour PBcomponent;

    [SerializeField] private bool IsRight;
    [SerializeField] private int Augment = 100; 
    int count = 0;

    private DataDictionary Aumentos = new DataDictionary()
    {
        {0, 100},
        {1, 200},
        {2, 500},
        {3, 1000}
    };


    void Start()
    {
        Augment = Aumentos[count].Int;
        if (ReferenceGOComponent == null)
            Debug.LogError($"No Reference gameobject reference found, add a GameObject with a MicroscopeElements component to your scene or assign it");
        PositionImageViewer();
        //TryChangeImage("Acero 1018", Augment, 50);
        //TryChangeImageNew("Acero 1018", Augment, 50);
        CanvasGO.SetActive(false);
    }
    private void PositionImageViewer()
    {
        if (IsRight)
        {
            CanvasGO.transform.localPosition = new Vector3(0, 0.41f, 0.575f);
        }
        else
        {
            CanvasGO.transform.localPosition = new Vector3(0, 0.41f, -0.575f);
        }
    }

    public void OnAugmentChange(int count)
    {
        Augment = Aumentos[count].Int;
        if (PBcomponent == null)
            return;
        TryChangeImageNew(PBcomponent.getProbeType(), Augment);
        //TryChangeImage(PBcomponent.getProbeType(), Augment);
    }

    [Obsolete] void TryChangeImage(string type, int augment, int index = 0)
    {
        Sprite imagenAUsar = ReferenceGOComponent.placeHolder;
        bool Success = false;

        for (int i = 0; i < ReferenceGOComponent.elementos.Length; i++)
        {
            ElementType elementType = ReferenceGOComponent.elementos[i];
            if (type == elementType.type)
            {
                Success = true;
                Sprite[] imagenes = ReferenceGOComponent.elementos[i].GetAumentImages(augment);
                imagenAUsar = imagenes[UnityEngine.Random.Range(0, imagenes.Length)];
                break;
            }
        }

        if (!Success)
        {
            Debug.LogWarning($"{this}:no image detected with type {type}");
        }

        UpdateImage(ref imagenAUsar);
    }

    private void TryChangeImageNew(string type, int augment, int index = 0)
    {
        Sprite imagenAUsar = ReferenceGOComponent.placeHolder;
        bool ValidProveType = false;

        for (int i = 0; i < ReferenceGOComponent.elementos.Length; i++)
        {
            ElementType elementType = ReferenceGOComponent.elementos[i];
            if (type == elementType.type)
            {
                ValidProveType = true;
                IVRCImageDownload[] Textures = ReferenceGOComponent.elementos[i].GetAumentTextures(augment);
                if (Textures[0].State == VRCImageDownloadState.Complete)
                {
                    Texture2D texture = Textures[0].Result;
                    imagenAUsar = Sprite.Create(texture,new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                }
                else
                {
                    Debug.LogWarning($"{this}:image not finished download, {Textures[0].State}");
                }
                break;
            }
        }

        if (!ValidProveType)
        {
            Debug.LogWarning($"{this}:no image detected with type {type}");
        }

        UpdateImage(ref imagenAUsar);
    }

    private void UpdateImage(ref Sprite imagenAUsar)
    {
        foreach (var comp in CompImage)
        {
            if (comp == null)
            {
                continue;
            }

            comp.sprite = imagenAUsar;
        }
    }

    public void Canvas_On()
    {
        CanvasGO.SetActive(true);
    }

    public void Canvas_Off()
    {
        CanvasGO.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        PBcomponent = other.gameObject.GetComponent<ProbeBehabiour>();
        if (PBcomponent != null)
        {
            TryChangeImageNew(PBcomponent.getProbeType(), Augment);
            //TryChangeImage(PBcomponent.getProbeType(), Augment);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PBcomponent = other.gameObject.GetComponent<ProbeBehabiour>();
        if (PBcomponent != null)
        {
        }
    }

}
