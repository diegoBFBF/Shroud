using UnityEngine;

public abstract class FocusWidget : MonoBehaviour{


    protected void Start()
    {
        FocusTransitionManager.Instance.onFocusStart.AddListener(HandleonFocusStart);
        FocusTransitionManager.Instance.onFadeInStart.AddListener(HandleonFadeInStart);
        FocusTransitionManager.Instance.onFadeInComplete.AddListener(HandleonFadeInComplete);
        FocusTransitionManager.Instance.onFadeOutStart.AddListener(HandleonFadeOutStart);
        FocusTransitionManager.Instance.onFadeComplete.AddListener(HandleonFadeComplete);
        FocusTransitionManager.Instance.onFocusOutEnd.AddListener(HandleonFocusOutEnd);
        
    }

    virtual
    public void HandleonFocusOutEnd()
    {

    }

    virtual
    public void HandleonFadeComplete()
    {
    }

    virtual
    public void HandleonFadeOutStart(float fadeTime)
    {
    }

    virtual
    public void HandleonFadeInComplete()
    {
    }

    virtual
    public void HandleonFadeInStart(float fadeTime)
    {
    }

    virtual
    public void HandleonFocusStart()
    {
    }

    private void OnDisable()
    {
        FocusTransitionManager.Instance.onFocusStart.RemoveListener(HandleonFocusStart);
        FocusTransitionManager.Instance.onFadeInStart.RemoveListener(HandleonFadeInStart);
        FocusTransitionManager.Instance.onFadeInComplete.RemoveListener(HandleonFadeInComplete);
        FocusTransitionManager.Instance.onFadeOutStart.RemoveListener(HandleonFadeOutStart);
        FocusTransitionManager.Instance.onFadeComplete.RemoveListener(HandleonFadeComplete);
        FocusTransitionManager.Instance.onFocusOutEnd.RemoveListener(HandleonFocusOutEnd);
        
    }
}
