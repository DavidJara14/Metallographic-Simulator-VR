using UdonSharp;
using UnityEditor;
using UnityEngine;

public class ElementType : UdonSharpBehaviour
{
    public string type;
    public Sprite[] x100;
    public Sprite[] x200;
    public Sprite[] x500;
    public Sprite[] x1000;

    public Sprite[] GetAumentImages(int aumento)
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

}
