using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenWidget : FocusWidget
{
    [SerializeField]
    GameObject screen;

    private void Awake()
    {
        screen.SetActive(false);
    }

    public override void HandleonFadeInComplete()
    {
        screen.SetActive(true);
    }

    public override void HandleonFocusOutEnd()
    {
        screen.SetActive(false);
    }
}
