using UnityEngine;
using TMPro;
using UnityEngine.Video;
using DG.Tweening;

public class IntentWidget : FocusWidget {

    TMP_Text intentText;
    ObjectAnchors anchors;
    [SerializeField]
    Ease movementEase;
    float fadeTime;

    private void Awake()
    {
        anchors = GetComponent<ObjectAnchors>();
    }
    
    public void SetIntentText(string te){
        intentText.text = te;
    }

    override
    public void HandleonFadeInStart(float time)
    {
        
        fadeTime = time;
        transform.position = anchors.startPosition;
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
