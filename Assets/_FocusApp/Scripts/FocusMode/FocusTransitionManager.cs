using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class FocusTransitionManager : MonoBehaviour
{
    public static FocusTransitionManager Instance;

    [HideInInspector] public UnityEvent onFocusStart;
    [HideInInspector] public TimedEvent onFadeInStart;
    [HideInInspector] public UnityEvent onFadeInComplete;
    [HideInInspector] public TimedEvent onFadeOutStart;
    [HideInInspector] public UnityEvent onFadeComplete;
    [HideInInspector] public UnityEvent onFocusOutEnd;

    public float transitionInTime = 1.5f;
    public float transitionOutTime = 1.5f;

    public float countdownTime = 3f;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if(onFadeInStart == null){
            onFadeInStart = new TimedEvent();
        }

        if(onFadeOutStart == null){
            onFadeOutStart = new TimedEvent();
        }
    }

    [ContextMenu("Focus In")]
    public void FocusIn(){
        onFocusStart.Invoke(); 
        Invoke(nameof(StartTransition), countdownTime);
    }

    public void StartTransition(){
        onFadeInStart.Invoke(transitionInTime);
        Invoke(nameof(HandleTransitionEnd), transitionInTime);
    }

    private void HandleTransitionEnd()
    {
        onFadeInComplete.Invoke();
    }

    [ContextMenu("Focus Out")]
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

