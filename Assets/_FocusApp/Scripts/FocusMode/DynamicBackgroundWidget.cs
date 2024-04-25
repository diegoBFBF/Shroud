using DG.Tweening;
using UnityEngine;

public class DynamicBackgroundWidget : FocusWidget{

    public Ease fadeInEase = Ease.OutSine;
    public Ease fadeOutEase = Ease.InSine;

    public Material targetMaterial;
    public float height;

    override
    public void HandleonFadeInStart(float time){
        targetMaterial.DOFloat(height, "_Opacity", time).SetEase(fadeInEase);
    }

    override
    public void HandleonFadeOutStart(float time){
        targetMaterial.DOFloat(0, "_Opacity", time).SetEase(fadeInEase);

    }


}