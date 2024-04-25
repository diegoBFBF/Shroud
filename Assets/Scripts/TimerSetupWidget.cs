using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerSetupWidget : MonoBehaviour
{
    [SerializeField]
    GameObject enableContainter;

    [SerializeField]
    TextMeshProUGUI timerText;


    private void Start()
    {
        timerText.text = FocusTransitionManager.Instance.timeSet.ToString();
    }
    
    void BeginTimerSetup()
    {
        EnableVisuals
    }

    void EnableVisuals(bool enable)
    {
        enableContainter.SetActive(enable);
    }

    public void AdjustTime(bool increase)
    {
        if(increase) FocusTransitionManager.Instance.timeLeft += 5;
        else FocusTransitionManager.Instance.timeLeft -= 5;

        FocusTransitionManager.Instance.timeLeft = Mathf.Clamp(FocusTransitionManager.Instance.timeSet, 5, 30);

        timerText.text = FocusTransitionManager.Instance.timeLeft.ToString();
    }
}
