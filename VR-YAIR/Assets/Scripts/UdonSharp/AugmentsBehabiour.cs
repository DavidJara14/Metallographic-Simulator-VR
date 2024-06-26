using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class AugmentsBehabiour : UdonSharpBehaviour
{
    int count = 0;
    MicroscopeBehabiour microscopeBehabiour;

    private void Start()
    {
        microscopeBehabiour = gameObject.GetComponentInParent<MicroscopeBehabiour>();
    }

    public override void Interact()
    {
        count++;
        if(count == 4)
            count = 0;
        microscopeBehabiour.OnAugmentChange(count);
    }
}
