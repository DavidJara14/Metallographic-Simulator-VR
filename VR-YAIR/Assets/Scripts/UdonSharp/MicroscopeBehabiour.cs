using System;
using TMPro;
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

    [Obsolete()][SerializeField] private Image[] CompImage;
    [SerializeField] private RawImage[] CompRawImage;
    [SerializeField] private TextMeshProUGUI CompText;
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
        if(PBcomponent.GetComponent<ActivateMirror>().IsReady())
        {
            TryChangeImageNew(PBcomponent.getProbeType(), Augment);
            CompText.text = $"x{Augment}";
        }
    }

    private void TryChangeImageNew(string type, int augment, int index = 0)
    {
        Texture2D rawImagenAUsar = ReferenceGOComponent.placeHolderT2D;
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
                    rawImagenAUsar = Textures[0].Result;
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
        UpdateRawImage(ref rawImagenAUsar);
    }

    private void UpdateRawImage(ref Texture2D imagenAUsar)
    {
        foreach (var comp in CompRawImage)
        {
            if (comp == null)
            {
                continue;
            }

            comp.texture = imagenAUsar;
        }
    }

    private void RemoveImage()
    {
        Texture2D sprite = null;
        UpdateRawImage(ref sprite);
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
            if (PBcomponent.GetComponent<ActivateMirror>().IsReady())
            {
                TryChangeImageNew(PBcomponent.getProbeType(), Augment);
                CompText.text = $"x{Augment}";
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PBcomponent = other.gameObject.GetComponent<ProbeBehabiour>();
        if (PBcomponent != null)
        {
            RemoveImage();
            PBcomponent = null;
        }
    }

}
