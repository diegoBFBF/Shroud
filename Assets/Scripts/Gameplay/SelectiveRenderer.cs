using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectiveRenderer : MonoBehaviour
{
    
    Renderer[] renderers;

    PlayerManager playerOwner;

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
    }

    private void OnEnable()
    {
        if(GameManager.Instance){
            GameManager.Instance.onRendererEvent.AddListener(UpdateStatus);
        }
    }

    //TODO: optimize
    private void UpdateStatus(PlayerManager player)
    {
        foreach(var ren in renderers){
            ren.enabled = playerOwner != player;
        }
    }

    private void OnDisable()
    {
        if(GameManager.Instance){
            GameManager.Instance.onRendererEvent.RemoveListener(UpdateStatus);
        }
    }
}
