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

    public string lastRecording;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            OpenDictationMic();
        }
        else if (OVRInput.GetUp(OVRInput.Button.One))
        {
            CloseDictationMic();
            Invoke("ResetText", 3);
        }
    }

    public void OpenDictationMic()
    {
        dictationExperience.Activate();
    }

    public void CloseDictationMic()
    {
        dictationExperience.Deactivate();
    }

    public void SetLastRecording(string rec)
    {
        lastRecording = rec;
    }


    void ResetText()
    {
        text.text = "Press A";
    }
}
