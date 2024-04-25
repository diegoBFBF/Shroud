using UnityEngine;
using TMPro;
using UnityEngine.Video;
using DG.Tweening;

public class IntentWidget : FocusWidget {

    [SerializeField]
    TMP_Text intentText;
    [SerializeField]
    ObjectAnchors anchors;
    [SerializeField]
    Ease movementEase;
    float fadeTime;

    private void Awake()
    {
        
    }
    
    public void SetIntentText(string te){
        intentText.text = te;
    }

    override
    public void HandleonFadeInStart(float time)
    {
        transform.position = anchors.startPosition;
        fadeTime = time;
        intentText.DOFade(time,1);

    }

    override
    public void HandleonFadeInComplete()
    {
        transform.DOMove(anchors.endposition,fadeTime).SetEase(movementEase);
    }

    override
    public void HandleonFadeOutStart(float time){
        intentText.DOFade(time,0);
    }

}
