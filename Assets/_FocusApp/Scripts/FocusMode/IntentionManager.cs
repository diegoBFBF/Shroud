using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class IntentionManager : MonoBehaviour
{
    public static IntentionManager Instance;

    public bool intentionSet;

    [SerializeField]
    GameObject welcomeText;

    [SerializeField]
    TextMeshProUGUI spokenIntentionText;

    [SerializeField]
    Transform noteIntentionContainer;

    public event Action<bool> OnIntentionSet;

    private void Awake()
    {
        Instance = this;
    }

    public void BeginIntentionSetup()
    {
        intentionSet = false;
        spokenIntentionText.text = "";
        if(noteIntentionContainer.childCount > 0)
        {
            Destroy(noteIntentionContainer.GetChild(0).gameObject);
        }

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
        noteIntention.rotation = Quaternion.LookRotation(noteIntentionContainer.up, Vector3.up);

        welcomeText.SetActive(false);
        noteIntentionContainer.gameObject.SetActive(true);

        intentionSet = true;
        OnIntentionSet?.Invoke(true);
    }
}
