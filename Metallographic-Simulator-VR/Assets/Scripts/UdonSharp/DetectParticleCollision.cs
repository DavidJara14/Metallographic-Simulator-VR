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

        string orientation = gameObject.GetComponent<OrientationChecker>().checkOrientation();

        ActivateMirror activateMirrorTarget;

        if (orientation == "Down")
        {
            activateMirrorTarget = activateMirror1;
        }
        else if (orientation == "Up")
        {
            activateMirrorTarget = activateMirror2;
        }
        else
        {
            activateMirrorTarget = null;
        }
        if (activateMirrorTarget == null) return;

        liquidSource(other, activateMirrorTarget);
        botellaLab(other, activateMirrorTarget);
    }

    private void liquidSource(GameObject other, ActivateMirror activateMirrorTarget)
    {
        var liquidSource = other.GetComponentInParent<IsLiquidSource>();
        if (liquidSource != null && activateMirrorTarget.finishedPulido2)
        {
            activateMirrorTarget.waterPS = other.GetComponent<ParticleSystem>();
            Debug.LogWarning($"[<color=blue>waterPS assigned to: </color>]{activateMirrorTarget.waterPS.name}");
        }
    }

    private void botellaLab(GameObject other, ActivateMirror activateMirrorTarget)
    {
        var botellaLab = other.GetComponentInParent<BotellaLab>();
        if (botellaLab != null)
        {
            string tipo = botellaLab.Tipo;
            checkAndSendEvent(activateMirrorTarget, tipo);
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