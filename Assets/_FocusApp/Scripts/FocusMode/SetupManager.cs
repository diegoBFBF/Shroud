using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupManager : MonoBehaviour
{
    IntentionManager intentionManager;
    TimerSetupWidget timerSetupWidget;

    private void Awake()
    {
        intentionManager = GetComponent<IntentionManager>();
        timerSetupWidget = GetComponent<TimerSetupWidget>();
    }


    private void Start()
    {
        BeginSetup();
    }

    void BeginSetup()
    {
        intentionManager.BeginIntentionSetup();
        intentionManager.OnIntentionSet += OnIntentionSet;
    }

    void OnIntentionSet(bool set)
    {
        timerSetupWidget.BeginTimerSetup();
        intentionManager.OnIntentionSet -= OnIntentionSet;
    }

}
