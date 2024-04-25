using Oculus.Interaction;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabableSnapToTable : MonoBehaviour
{
    [SerializeField]
    GrabInteractable grab;

    bool holding;

    private void Start()
    {
        grab.WhenStateChanged += StateChanged;
    }

    private void StateChanged(InteractableStateChangeArgs args)
    {
        if(args.PreviousState == InteractableState.Select && args.NewState == InteractableState.Normal)
        {
            RaycastHit hit;
            if(Physics.Raycast(transform.position, -transform.up, out hit, 0.5f))
            {
                if(Vector3.Angle(Vector3.up, hit.normal) < 10)
                {
                    transform.position = hit.point;
                    //transform.rotation = ;
                }
            }
        }
    }
}
