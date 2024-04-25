using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupManager : FocusWidget
{
    IntentionManager intentionManager;
    TimerSetupWidget timerSetupWidget;

    private void Awake()
    {
        intentionManager = GetComponent<IntentionManager>();
        timerSetupWidget = GetComponent<TimerSetupWidget>();
    }


    public override void Start()
    {
        base.Start();
        BeginSetup();
    }

    void BeginSetup()
    {
        Debug.Log("Begin Intention Setup");
        intentionManager.BeginIntentionSetup();
        intentionManager.OnIntentionSet += OnIntentionSet;
    }

    void OnIntentionSet(bool set)
    {
        timerSetupWidget.BeginTimerSetup();
        intentionManager.OnIntentionSet -= OnIntentionSet;
    }

    public override void HandleonFocusOutEnd()
    {
        BeginSetup();
    }
}
