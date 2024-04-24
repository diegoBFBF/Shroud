using UnityEngine;
using DG.Tweening;

public class VisualBlockerWidget : FocusWidget{

    public ObjectAnchors upMovementPath;
    public ObjectAnchors downMovementPath;
    
    [SerializeField]
    Ease easeIn;
    [SerializeField]
    Ease easeOut;

    override
    public void HandleonFadeInStart(float moveTime){
        transform.position = upMovementPath.startPosition;
        transform.DOMove(upMovementPath.endposition,moveTime).SetEase(easeIn);
    }

    override
    public void HandleonFadeOutStart(float moveTime){
        // transform.position = downMovementPath.startPosition;
        transform.DOMove(downMovementPath.endposition,moveTime).SetEase(easeOut);
    }
    
    
}
