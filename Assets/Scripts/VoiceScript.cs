using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Voice;
using Oculus.Voice.Dictation;
using TMPro;

public class VoiceScript : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI text;

    public AppDictationExperience dictationExperience;

    public AppVoiceExperience voiceExperience;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            dictationExperience.Activate();
        }
        else if (OVRInput.GetUp(OVRInput.Button.One))
        {
            dictationExperience.Deactivate();
            Invoke("ResetText", 3);
        }
    }


    void ResetText()
    {
        text.text = "Press A";
    }
}
