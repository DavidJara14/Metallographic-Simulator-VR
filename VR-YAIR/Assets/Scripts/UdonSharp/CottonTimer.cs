
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class CottonTimer : UdonSharpBehaviour
{
    [SerializeField] private CottonBehabiour cottonBehabiour;
    [SerializeField] private float Timer = 0f;

    void Start()
    {
        cottonBehabiour = GetComponent<CottonBehabiour>();
        this.enabled = false;
    }

    private void Update()
    {
        Timer += Time.deltaTime;
        if (Timer >= 1f)
        {
            Timer = 1f;
            this.enabled = false;
        }
        cottonBehabiour.ChangeAlcohol(Timer);
    }

    private void OnEnable()
    {
        Timer = 0f;
    }

}
