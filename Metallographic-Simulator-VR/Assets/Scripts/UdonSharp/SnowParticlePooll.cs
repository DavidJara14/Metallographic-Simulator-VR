
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

public class SnowParticlePooll : UdonSharpBehaviour
{

    public VRCObjectPool pool;

    private void Awake()
    {
        if(pool == null)
            pool = GetComponent<VRCObjectPool>();
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        GameObject go = pool.TryToSpawn();
        if(go != null)
        {
            go.GetComponent<SnowParticleFollowPlayer>().target = player;
        }
        else
        {
            Debug.Log("Mas");
        }
    }

}
