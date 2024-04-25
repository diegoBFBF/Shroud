using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClockWidget : FocusWidget
{
    [SerializeField]
    TextMeshProUGUI timerText;

    [SerializeField]
    Image fillImage;

    [SerializeField]
    GameObject visualContainer;


    private void Awake()
    {
        EnableVisuals(false);
    }

    private void Update()
    {
        float seconds = FocusTransitionManager.Instance.timeLeft;
        TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
        string timer = timeSpan.ToString("mm\\:ss");
        timerText.text = timer;

        fillImage.fillAmount = Mathf.Lerp(1,0, FocusTransitionManager.Instance.timeLeft / FocusTransitionManager.Instance.timeSet);
    }

    public override void HandleonFadeInComplete()
    
    {
        visualContainer.SetActive(true);
    }
    public override void HandleonFocusOutEnd()
    
    {
        visualContainer.SetActive(false);
    }

    void EnableVisuals(bool enable)
    {
        visualContainer.SetActive(enable);
    }
}
