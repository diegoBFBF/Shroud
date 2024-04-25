using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteToolManager : MonoBehaviour
{
    enum ToolType { None, Pen, Mic }

    [SerializeField]
    ToolType currentTool;

    [SerializeField]
    GameObject controller;
    [SerializeField]
    GameObject pen;
    [SerializeField]
    GameObject mic;

    [SerializeField]
    Transform notePadTransform;
    [SerializeField]
    Transform headTransform;

    [SerializeField]
    float activeDistance = 0.3f;

    private void Start()
    {
        InvokeRepeating("CheckDistance", 1, 0.5f);
    }

    void CheckDistance()
    {
        float headDistance = Vector3.Distance(transform.position, headTransform.position);
        float noteDistance = Vector3.Distance(transform.position, notePadTransform.position);
        //Debug.Log($"Checking Distance - head={headDistance} - note={noteDistance}");

        if(headDistance < activeDistance && noteDistance < activeDistance)
        {
            if(headDistance < noteDistance)
            {
                SwitchTool(ToolType.Mic);
            }
            else
            {
                SwitchTool(ToolType.Pen);
            }
        }
        else if(headDistance < activeDistance)
        {
            SwitchTool(ToolType.Mic);
        }
        else if(noteDistance < activeDistance)
        {
            SwitchTool(ToolType.Pen);
        }
        else
        {
            SwitchTool(ToolType.None);
        }
    }

    void SwitchTool(ToolType newTool)
    {
        if (newTool == currentTool) return;
        currentTool = newTool;

        controller.SetActive(false);
        pen.SetActive(false);
        mic.SetActive(false);
        switch (currentTool)
        {
            case ToolType.None: controller.SetActive(true); break;
            case ToolType.Pen: pen.SetActive(true); break;
            case ToolType.Mic: mic.SetActive(true); break;
        }
    }
}
