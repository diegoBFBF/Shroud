using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class FocusTransitionManager : MonoBehaviour
{
    public static FocusTransitionManager Instance;

    public UnityEvent onFocusStart;
    public TimedEvent onFadeInStart;
    public UnityEvent onFadeInComplete;
    public TimedEvent onFadeOutStart;
    public UnityEvent onFadeComplete;
    public UnityEvent onFocusOutEnd;

    public float transitionInTime = 1.5f;
    public float transitionOutTime = 1.5f;

    void Start()
    {
        if(onFadeInStart == null){
            onFadeInStart = new TimedEvent();
        }

        if(onFadeOutStart == null){
            onFadeOutStart = new TimedEvent();
        }
    }

    public void FocusIn(){
        onFocusStart.Invoke();   
    }

    public void StartTransition(){
        onFadeInStart.Invoke(transitionInTime);
        Invoke(nameof(HandleTransitionEnd), transitionInTime);
    }

    private void HandleTransitionEnd()
    {
        onFadeInComplete.Invoke();
    }

    public void FocusOut(){
        onFadeOutStart.Invoke(transitionOutTime);
        Invoke(nameof(HandleTransitionOutEnd), transitionOutTime);
    }

    private void HandleTransitionOutEnd()
    {
        onFadeComplete.Invoke();
        onFocusOutEnd.Invoke();
    }
}

[System.Serializable]
public class TimedEvent : UnityEvent<float>{}

public class FocusManager{

    public float targetTime;

    public void StateIntent(string intent){

    }



}

