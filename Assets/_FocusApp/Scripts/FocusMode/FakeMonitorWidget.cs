using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Video;

public class FakeMonitorWidget : FocusWidget
{
    
    public GameObject clutterMonitor;
    public Material clutterMonitorMaterial;
    public GameObject declutterMonitor;
    public Material declutterMonitorMaterial;

    override
    public void Start(){
        base.Start();
        clutterMonitor.SetActive(true);
        declutterMonitor.SetActive(false);
    }

    override
    public void HandleonFadeInStart(float time){
        clutterMonitor.SetActive(true);
        clutterMonitorMaterial.DOFade(0,time/2);
        declutterMonitorMaterial.SetFloat("_Opacity",1);
        declutterMonitorMaterial.DOFade(1,time/2);
    }



    override
    public void HandleonFadeOutStart(float time){
        declutterMonitorMaterial.DOFade(0,time/2);
        clutterMonitorMaterial.DOFade(1,time/2);
    }

    
}
