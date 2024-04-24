using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(OwnedByPlayer))]
public class SelectiveRenderer : MonoBehaviour
{
    
    Renderer[] renderers;

    OwnedByPlayer onwer;

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
        onwer = GetComponent<OwnedByPlayer>();
    }

    private void OnEnable()
    {
        if(GameManager.Instance){
            GameManager.Instance.onRendererEvent.AddListener(UpdateStatus);
        }
    }

    //TODO: optimize
    private void UpdateStatus(int playerId)
    {
        foreach(var ren in renderers){
            ren.enabled = onwer.PlayerId != playerId;
        }
    }

    private void OnDisable()
    {
        if(GameManager.Instance){
            GameManager.Instance.onRendererEvent.RemoveListener(UpdateStatus);
        }
    }
}
