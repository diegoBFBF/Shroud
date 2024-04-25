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

    [SerializeField]
    Transform centerEye;

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

        Vector3 tableDimensions = anchor.transform.GetChild(0).GetComponent<BoxCollider>().size;

        Vector3 direction = centerEye.position - newPosition;
        direction = new Vector3(direction.x, 0, direction.z).normalized;

        direction = direction * 100;
        direction = new Vector3(Mathf.Clamp(direction.x, 0, tableDimensions.x), 0, Mathf.Clamp(direction.z, 0, tableDimensions.z));

        newPosition = newPosition + direction;

        workspaceAnchor.position = (newPosition);

        workspaceAnchor.rotation = Quaternion.LookRotation(centerEye.forward, Vector3.up);
    }
}
