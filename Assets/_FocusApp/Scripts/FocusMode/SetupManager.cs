using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupManager : MonoBehaviour
{
    IntentionManager intentionManager;
    TimerSetupWidget timerSetupWidget;

    void BeginSetup()
    {
        intentionManager.BeginIntentionSetup();
        intentionManager.OnIntentionSet += OnIntentionSet;
    }

    void OnIntentionSet(bool set)
    {
        //timerSetupWidget.
    }

}
