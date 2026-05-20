
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Cheto : UdonSharpBehaviour
{

    [SerializeField] private bool HideOnUse;
    [SerializeField, UdonSynced] private bool Used;
    [SerializeField, UdonSynced] private bool Finished;
    [SerializeField] private float timer = 3f;

    [SerializeField] private Vector3 _ref;
    [SerializeField] private VRC_Pickup _pickup;
    [SerializeField] private GameObject _visual;
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private AudioSource _audioSource;

    private void Start()
    {
        _ref = transform.position;
        if((Used || Finished) && HideOnUse)
        {
            gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if(Used && !Finished && timer > 0f)
        {
            timer -= Time.deltaTime;
            if(timer <= 0f)
            {
                Finished = true;
                gameObject.SetActive(false);
                RequestSerialization();
            }
        }
    }

    public override void OnDrop()
    {
        _particleSystem.Emit(3);
        _audioSource.Play();
        if(HideOnUse)
        {
            _visual.SetActive(false);
            Used = true;
            _pickup.pickupable = false;
            RequestSerialization();
        }
        else
            gameObject.transform.position = _ref;
    }

    public override void OnDeserialization()
    {
        if ((Used || Finished) && HideOnUse)
        {
            gameObject.SetActive(false);
        }
    }
}
