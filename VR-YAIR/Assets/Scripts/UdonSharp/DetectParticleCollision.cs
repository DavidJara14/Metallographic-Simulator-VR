using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class DetectParticleCollision : UdonSharpBehaviour
{
    public ActivateMirror activateMirror1;
    public ActivateMirror activateMirror2;

    private void OnParticleCollision(GameObject other)
    {
        Debug.LogWarning($"[<color=green>OnParticleCollision, GO name: </color>]{other.gameObject.name}");

        if (activateMirror1.IsReady() && activateMirror2.IsReady()) return;

        liquidSource(other);
        botellaLab(other);
    }

    private void liquidSource(GameObject other)
    {
        var liquidSource = other.GetComponentInParent<IsLiquidSource>();
        if (liquidSource != null)
        {
            assignWaterPS(activateMirror1, other);
            assignWaterPS(activateMirror2, other);
        }
    }

    private void assignWaterPS(ActivateMirror activateMirror, GameObject other)
    {
        if (activateMirror.finishedPulido2)
        {
            activateMirror.waterPS = other.GetComponent<ParticleSystem>();
            Debug.LogWarning($"[<color=blue>waterPS, assigned: </color>]{activateMirror.waterPS.name}");
        }
    }

    private void botellaLab(GameObject other)
    {
        var botellaLab = other.GetComponentInParent<BotellaLab>();
        if (botellaLab != null)
        {
            string tipo = botellaLab.Tipo;
            checkAndSendEvent(activateMirror1, tipo);
            checkAndSendEvent(activateMirror2, tipo);
        }
    }

    private void checkAndSendEvent(ActivateMirror activateMirror, string tipo)
    {
        if (tipo == "Nital" && activateMirror.finishedPulido1 && activateMirror.finishedPulido2 && !activateMirror.haveNital && activateMirror.finishedLimpieza)
        {
            activateMirror.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "addedNital");
        }
        if (tipo == "Alcohol" && activateMirror.finishedEnjuagado && !activateMirror.haveAlcohol)
        {
            activateMirror.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "addedAlcohol");
            Debug.LogWarning($"[<color=red>Status haveAlcohol in particle collision: </color>]{activateMirror.haveAlcohol}");
        }
    }
}