using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class IntentionManager : MonoBehaviour
{
    bool intentionSet;

    [SerializeField]
    GameObject welcomeText;

    [SerializeField]
    TextMeshProUGUI spokenIntentionText;

    [SerializeField]
    Transform noteIntentionContainer;

    public event Action<bool> OnIntentionSet;
    
    public void BeginIntentionSetup()
    {
        welcomeText.SetActive(true);
        spokenIntentionText.gameObject.SetActive(false);
        noteIntentionContainer.gameObject.SetActive(false);
    }

    public void SetSpokenIntention(string spokenIntention)
    {
        spokenIntentionText.text = spokenIntention;

        welcomeText.SetActive(false);
        spokenIntentionText.gameObject.SetActive(true);

        intentionSet = true;
        OnIntentionSet?.Invoke(true);
    }

    public void SetNoteIntention(Transform noteIntention)
    {
        noteIntention.SetParent(noteIntentionContainer);
        noteIntention.localPosition = Vector3.zero;
        noteIntention.localRotation = Quaternion.identity;

        welcomeText.SetActive(false);
        noteIntentionContainer.gameObject.SetActive(true);

        intentionSet = true;
        OnIntentionSet?.Invoke(true);
    }
}
