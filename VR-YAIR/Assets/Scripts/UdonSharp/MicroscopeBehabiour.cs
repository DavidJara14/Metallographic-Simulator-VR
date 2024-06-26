using System;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class MicroscopeBehabiour : UdonSharpBehaviour
{

    [SerializeField] private Sprite[] Imagenes;
    [SerializeField] private Image[] CompImage;
    [SerializeField] private GameObject CanvasGO;

    [SerializeField] private bool IsRight;

    void Start()
    {
        PositionImage();
        UpdateImage(0);
    }

    private void PositionImage()
    {
        if(IsRight)
        {
            CanvasGO.transform.localPosition = new Vector3 (0, 0.41f, 0.575f);
        }
        else
        {
            CanvasGO.transform.localPosition = new Vector3(0, 0.41f, -0.575f);
        }
    }

    void UpdateImage(int index)
    {
        foreach(var comp in CompImage)
        {
            if (comp == null)
            {
                continue;
            }

            comp.sprite = Imagenes[index];
        }
    }

}
