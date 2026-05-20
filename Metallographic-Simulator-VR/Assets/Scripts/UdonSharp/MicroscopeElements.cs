using System;
using System.Xml.Linq;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDK3.Image;
using VRC.Udon;

public class MicroscopeElements : UdonSharpBehaviour
{
    [SerializeField] public ElementType[] elementos;
    //public Sprite placeHolder;
    public Texture2D placeHolderT2D;
    public VRCImageDownloader imageDownloader;

    private void Start()
    {
        imageDownloader = new VRCImageDownloader();
        foreach (var elemento in elementos)
        {
            elemento.Setup(ref imageDownloader);
        }
    }

    private void OnDestroy()
    {
        imageDownloader.Dispose();
    }
}

