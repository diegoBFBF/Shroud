using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spaceship : MonoBehaviour
{
    OwnedByPlayer onwer;
    Health health;
    [SerializeField]
    GameObject hitFx;
    [SerializeField]
    GameObject dieFx;

    private void Awake()
    {
        onwer = GetComponent<OwnedByPlayer>();
        health = GetComponent<Health>();
        health.OnHit.AddListener(HandleHit);
    }

    private void HandleHit(){
        //KYLE TODO: Network instantiate hit FX
    }

    private void HanldeDestroyed(){
        GameManager.Instance.HandleShipDestroyed(this);
        //KYLE TODO: Network instantiate death FX
        //KYLE TODO: Network destroy This object
        Destroy(gameObject);//This
    }
}
