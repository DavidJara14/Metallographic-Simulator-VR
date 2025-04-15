
using UdonSharp;
using UnityEngine;
using UnityEngine.Device;
using VRC.SDKBase;
using VRC.Udon;

public class BorderColor : UdonSharpBehaviour
{
    public GameObject bodyMaterial = null;
    [SerializeField] private float colorTimer = 0f;
    [SerializeField] private bool isClear = false;
    //[SerializeField] private float Desgaste = 0f;

    //private void Update()
    //{
    //    Desgaste = gameObject.GetComponent<ProbeBehabiour>().Desgaste;
    //}

    public void colorGreen()
    {
        bodyMaterial.GetComponent<Renderer>().material.SetFloat("_Scale", 1.1f);
        colorTimer += Time.deltaTime;
        if (colorTimer >= 0.1f)
        {
            if (isClear)
            {
                bodyMaterial.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
                isClear = false;
                //Debug.Log("Desgaste set to: " + Desgaste);
            }
            else
            {
                bodyMaterial.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
                isClear = true;
            }
            colorTimer = 0f;
        }
    }

    public void colorRed()
    {
        bodyMaterial.GetComponent<Renderer>().material.SetFloat("_Scale", 1.1f);
        colorTimer += Time.deltaTime;
        if (colorTimer >= 0.1f)
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
    }

    public void colorYellow()
    {
        bodyMaterial.GetComponent<Renderer>().materials[1].SetFloat("_Scale", 1.04f);
        colorTimer += Time.deltaTime;
        if (colorTimer >= 0.1f)
        {
            if (isClear)
            {
                bodyMaterial.GetComponent<Renderer>().materials[1].SetColor("_Color", Color.yellow);
                isClear = false;
            }
            else
            {
                bodyMaterial.GetComponent<Renderer>().materials[1].SetColor("_Color", Color.white);
                isClear = true;
            }
            colorTimer = 0f;
        }
    }

}
