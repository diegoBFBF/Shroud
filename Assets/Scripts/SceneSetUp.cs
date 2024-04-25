using Meta.XR.MRUtilityKit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

public class SceneSetUp : MonoBehaviour
{
    [SerializeField]
    MRUK mruk;

    [SerializeField]
    Transform workspaceAnchor;


    private void Awake()
    {
        mruk.SceneLoadedEvent.AddListener(OnSceneLoaded);
    }

    private void OnSceneLoaded()
    {
        Debug.Log("SceneSetUp - OnSceneLoaded");
        var room = mruk.GetCurrentRoom();
        var anchor = room.Anchors.Find(t => t.AnchorLabels.Contains(OVRSceneManager.Classification.Table));
        Vector3 newPosition = anchor.transform.position;

        workspaceAnchor.position = (newPosition);

        //workspaceAnchor.rotation = 
    }
}
