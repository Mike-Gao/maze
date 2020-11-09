using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    Pickup pickup;
    // Start is called before the first frame update
    void Start()
    {
        pickup = GetComponent<Pickup>();
        DebugUtility.HandleErrorIfNullGetComponent<Pickup, AmmoPickup>(pickup, this, gameObject);
        pickup.onPick += OnPick;

    }

    void OnPick(PlayerCharacterController p)
    {
        Ammo PlayerAmmo = p.GetComponent<Ammo>();
        if (PlayerAmmo)
        {
            PlayerAmmo.AddAmmo();
            pickup.PlayPickupFeedback();
            Destroy(gameObject);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
