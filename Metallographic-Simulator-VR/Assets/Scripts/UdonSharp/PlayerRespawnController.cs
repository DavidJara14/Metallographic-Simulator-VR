using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

public class PlayerRespawnController : UdonSharpBehaviour
{

    [SerializeField] private Transform playerRespawnPositionTransform;
    [SerializeField] private ParticleSystem playerRespawnFogParticleSystem;

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if(player.IsValid() && playerRespawnPositionTransform != null && playerRespawnFogParticleSystem != null)
        {
            playerRespawnFogParticleSystem.Play();
            player.TeleportTo(playerRespawnPositionTransform.position, 
                player.GetRotation());
            player.SetVelocity(new Vector3(0f, player.GetVelocity().y, 0f));
        }
    }

}
