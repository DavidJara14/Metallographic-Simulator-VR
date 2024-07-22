using System;
using UdonSharp;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Image;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;

public class ElementType : UdonSharpBehaviour
{
    public string type;
    IUdonEventReceiver _udonEventReceiver;

    [Header("Predecapted")]
    [Obsolete] public Sprite[] x100;
    [Obsolete] public Sprite[] x200;
    [Obsolete] public Sprite[] x500;
    [Obsolete] public Sprite[] x1000;

    [Header("Links")]
    public VRCUrl[] LinkX100;
    public VRCUrl[] LinkX200;
    public VRCUrl[] LinkX500;
    public VRCUrl[] LinkX1000;

    [Header("Textures")]
    public IVRCImageDownload[] TextureX100;
    public IVRCImageDownload[] TextureX200;
    public IVRCImageDownload[] TextureX500;
    public IVRCImageDownload[] TextureX1000;

    public void Setup(ref VRCImageDownloader imageDownloader)
    {
        _udonEventReceiver = (IUdonEventReceiver)this;

        TextureX100 = new IVRCImageDownload[LinkX100.Length];
        TextureX200 = new IVRCImageDownload[LinkX200.Length];
        TextureX500 = new IVRCImageDownload[LinkX500.Length];
        TextureX1000 = new IVRCImageDownload[LinkX1000.Length];

        for (int i = 0; i < LinkX100.Length; i++)
        {
            TextureX100[i] = imageDownloader.DownloadImage(LinkX100[i], default, _udonEventReceiver);
        }
        for(int i = 0; i < LinkX200.Length; i++)
        {
            TextureX200[i] = imageDownloader.DownloadImage(LinkX200[i], default, _udonEventReceiver);
        }
        for (int i = 0; i < LinkX100.Length; i++)
        {
            TextureX500[i] = imageDownloader.DownloadImage(LinkX500[i], default, _udonEventReceiver);
        }
        for (int i = 0; i < LinkX200.Length; i++)
        {
            TextureX1000[i] = imageDownloader.DownloadImage(LinkX1000[i], default, _udonEventReceiver);
        }
}

    [Obsolete] public Sprite[] GetAumentImages(int aumento)
    {
        Sprite[] images = null;
        
        switch (aumento)
        {
            case 100:
                images = x100;
                break;
            case 200:
                images = x200;
                break;
            case 500:
                images = x500;
                break;
            case 1000:
                images = x1000;
                break;
            default:
                Debug.LogWarning($"Aumento x{aumento} no encontrado");
                break;
        }

        return images;
    }

    public IVRCImageDownload[] GetAumentTextures(int aumento)
    {
        IVRCImageDownload[] links = null;

        switch (aumento)
        {
            case 100:
                links = TextureX100;
                break; 
            case 200:
                links = TextureX200;
                break;
            case 500:
                links = TextureX500;
                break;
            case 1000:
                links = TextureX1000;
                break;
            default:
                Debug.LogWarning($"Aumento x{aumento} no encontrado");
                break;
        }
        return links;
    }

    public override void OnImageLoadSuccess(IVRCImageDownload result)
    {
        Debug.Log($"Image loaded: {result.SizeInMemoryBytes} bytes.");
    }

    public override void OnImageLoadError(IVRCImageDownload result)
    {
        Debug.Log($"Image not loaded: {result.Error.ToString()}: {result.ErrorMessage}.");
    }

}
