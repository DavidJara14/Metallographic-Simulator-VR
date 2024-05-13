using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class MicroscopeInteraction : UdonSharpBehaviour
{
/*    public Material copyMaterial; // El material que usaremos para copiar la textura

    public Texture2D sampleImage; // La imagen de la muestra
    public GameObject playerCamera; // La cámara del jugador
    public Material copyMaterial; // El material que usaremos para copiar la textura

    private bool isLookingThroughMicroscope = false;
    private RenderTexture sampleRenderTexture;

    void Start()
    {
        // Creamos una nueva RenderTexture
        sampleRenderTexture = new RenderTexture(sampleImage.width, sampleImage.height, 0);

        // Copiamos la textura en la RenderTexture usando el material
        Graphics.Blit(sampleImage, sampleRenderTexture, copyMaterial);
    }

    public override void Interact()
    {
        // Cuando el usuario interactúa con el microscopio, cambiamos la vista
        isLookingThroughMicroscope = !isLookingThroughMicroscope;

        if (isLookingThroughMicroscope)
        {
            // Cambiamos la textura de la cámara para mostrar la imagen de la muestra
            playerCamera.GetComponent<Camera>().targetTexture = sampleRenderTexture;
        }
        else
        {
            // Volvemos a la vista normal
            playerCamera.GetComponent<Camera>().targetTexture = null;
        }
    }

    private void Update()
    {
        if (isLookingThroughMicroscope)
        {
            // Aquí puedes implementar la lógica para desplazarse por la imagen de la muestra
            // Por ejemplo, podrías cambiar la posición de la textura en función de la entrada del usuario
        }
    }*/
}
