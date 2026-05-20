using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

public class SnowParticleFollowPlayer : UdonSharpBehaviour
{

    public VRCObjectPool pool;
    public VRCPlayerApi target;

    private void Update()
    {
        if(target != null)
            gameObject.transform.position = target.GetPosition();
    }

    public override void OnPlayerLeft(VRCPlayerApi player)
    {
        pool.Return(this.gameObject);
    }

}
