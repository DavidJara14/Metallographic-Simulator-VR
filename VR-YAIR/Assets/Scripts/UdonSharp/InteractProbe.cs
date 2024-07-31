
using System;
using UdonSharp;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using VRC.Core;
using VRC.SDK3.Data;
using VRC.SDK3.Image;
using VRC.SDKBase;
using VRC.Udon;
using static UnityEngine.Rendering.HableCurve;

public class InteractProbe : UdonSharpBehaviour
{
    public bool activate = false;
    public Camera miCamera;
    public Image miImage;
    public Texture2D miTexture;
    public Sprite miSprite;
    public GameObject canva;

    public Camera targetCamera; // Asigna la cámara que deseas capturar
    public RenderTexture renderTexture; // Crea una RenderTexture para almacenar la imagen

    public VRC_Pickup pickup;
    public bool isUserVR;

    private void Start()
    {
        // Configura la RenderTexture con las dimensiones de la cámara
        //renderTexture = new RenderTexture(targetCamera.pixelWidth, targetCamera.pixelHeight, 24);
        //targetCamera.targetTexture = renderTexture;
        canva.SetActive(false);
        isUserVR = false;
    }
    /*private void Update()
    {
        //miTexture = RTImage(miCamera);
        miTexture = Capture();
        miSprite = Sprite.Create(miTexture, new Rect(0, 0, miTexture.width, miTexture.height), Vector2.zero);
        miImage.sprite = miSprite;
    }*/

    private void Update()
    {
       // isUserVR = pickup.currentPlayer.IsUserInVR();
    }


    public override void Interact()
    {
        activate = !activate;
        if (activate)
        {
            canva.SetActive(true);
        }
        else if (!activate) 
        {
            canva.SetActive(false);
        }
    }

    public void DisableCanva()
    {
        canva.SetActive(false);
    }

    /*Texture2D RTImage(Camera camera)
    {
        // The Render Texture in RenderTexture.active is the one
        // that will be read by ReadPixels.
        var currentRT = RenderTexture.active;
        RenderTexture.active = camera.targetTexture;

        // Render the camera's view.
        camera.Render();

        // Make a new texture and read the active Render Texture into it.
        Texture2D image = new Texture2D(camera.targetTexture.width, camera.targetTexture.height);
        image.ReadPixels(new Rect(0, 0, camera.targetTexture.width, camera.targetTexture.height), 0, 0);
        image.Apply();

        // Replace the original active Render Texture.
        RenderTexture.active = currentRT;
        return image;
    }*/

    // Llama a esta función para capturar la imagen
    /*public Texture2D Capture()
    {
        // Renderiza la cámara en la RenderTexture
        targetCamera.Render();

        // Lee los píxeles de la RenderTexture
        Texture2D screenshot = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        RenderTexture.active = renderTexture;
        screenshot.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        screenshot.Apply();
        return screenshot;

        /*
        // Guarda la imagen (puedes ajustar la ruta y el formato según tus necesidades)
        byte[] bytes = screenshot.EncodeToPNG();
        System.IO.File.WriteAllBytes("CapturedImage.png", bytes);

        // Limpia la RenderTexture
        RenderTexture.active = null;
        targetCamera.targetTexture = null;*/
    //}
}





