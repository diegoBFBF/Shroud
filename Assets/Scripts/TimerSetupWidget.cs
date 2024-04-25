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


    private void Awake()
    {
        EnableVisuals(false);
    }

    private void Start()
    {
        timerText.text = FocusTransitionManager.Instance.timeSet.ToString();
    }
    
    public void BeginTimerSetup()
    {
        EnableVisuals(true);
    }

    public void EndTimerSetup()
    {
        FocusTransitionManager.Instance.FocusIn();
    }

    public void EnableVisuals(bool enable)
    {
        enableContainter.SetActive(enable);
    }

    public void AdjustTime(bool increase)
    {
        if(increase) FocusTransitionManager.Instance.timeSet += 5;
        else FocusTransitionManager.Instance.timeSet -= 5;

        FocusTransitionManager.Instance.timeSet = Mathf.Clamp(FocusTransitionManager.Instance.timeSet, 5, 30);

        timerText.text = FocusTransitionManager.Instance.timeSet.ToString();
    }
}
