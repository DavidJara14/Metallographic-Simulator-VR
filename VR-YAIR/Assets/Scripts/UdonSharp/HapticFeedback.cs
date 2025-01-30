
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class HapticFeedback : UdonSharpBehaviour
{
    [SerializeField] private VRC_Pickup pickup;

    [SerializeField] private float hapticDuration = 0.05f;
    [SerializeField] private float hapticAmplitudeDesbaste = 0.5f;
    [SerializeField] private float hapticFrequencyDesbaste = 200f;
    [SerializeField] private float hapticAmplitudePulido = 0.2f;
    [SerializeField] private float hapticFrequencyPulido = 50f;


    [SerializeField] private float Desgaste = 0f;

    private void Update()
    {
        Desgaste = gameObject.GetComponent<ProbeBehabiour>().Desgaste;
    }

    public void hapticFeedbackDesbaste()
    {
        Networking.LocalPlayer.PlayHapticEventInHand(pickup.currentHand, hapticDuration, hapticAmplitudeDesbaste * (800f / Desgaste)*5, hapticFrequencyDesbaste);
        Debug.Log("Haptic Feedback desbaste!!!!!!!!!!!!");
    }

    public void hapticFeedbackPulido()
    {
        Networking.LocalPlayer.PlayHapticEventInHand(pickup.currentHand, hapticDuration, hapticAmplitudePulido, hapticFrequencyPulido);
        Debug.Log("Haptic Feedback Pulido!!!!!!!!!!!!");
    }

    public void hapticFeedbackCotton()
    {
        Networking.LocalPlayer.PlayHapticEventInHand(pickup.currentHand, hapticDuration, 0.15f, 25f);
        Debug.Log("Haptic Feedback Cotton!!!!!!!!!!!!");
    }
}