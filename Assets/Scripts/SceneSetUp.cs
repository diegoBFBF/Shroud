using Meta.XR.MRUtilityKit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Android;

public class SceneSetUp : MonoBehaviour
{
    [SerializeField]
    MRUK mruk;

    [SerializeField]
    Transform workspaceAnchor;

    [SerializeField]
    Transform centerEye;

    bool workspaceSet;

    float tableInset = 0.4f;

    private void Awake()
    {
        Permission.RequestUserPermission(OVRPermissionsRequester.RecordAudioPermission);
        mruk.SceneLoadedEvent.AddListener(OnSceneLoaded);
    }

    private void Start()
    {
        Invoke("NoRoomSetUp", 5);
    }

    private void Update()
    {
        if(OVRInput.GetDown(OVRInput.Button.Two))
        {
            SetWorkSpacePosition();
        }
    }

    private void OnSceneLoaded()
    {
        Debug.Log("SceneSetUp - OnSceneLoaded");
        SetWorkSpacePosition();
    }

    void SetWorkSpacePosition()
    {
        var room = mruk.GetCurrentRoom();

        if (room == null) { NoRoomSetUp(); return; }

        List<MRUKAnchor> tableAnchors = room.Anchors.Where(x => x.AnchorLabels.Contains(OVRSceneManager.Classification.Table)).ToList();

        if (tableAnchors.Count <= 0) { NoRoomSetUp(); return; }

        //find closest table
        MRUKAnchor closestAnchor = null;
        foreach (var tableAnchor in tableAnchors)
        {
            if (closestAnchor == null) { closestAnchor = tableAnchor; continue; }

            if (Vector3.Distance(closestAnchor.transform.position, centerEye.position) > Vector3.Distance(tableAnchor.transform.position, centerEye.position))
            {
                closestAnchor = tableAnchor; continue;
            }
        }

        //position
        Vector3 tableDimensions = closestAnchor.transform.GetChild(0).GetComponent<BoxCollider>().size;
        tableDimensions /= 2;

        Vector3 newPosition = closestAnchor.transform.position;

        if (tableDimensions.x > tableInset * 2 && tableDimensions.z > tableInset * 2)
        {
            Vector3 direction = centerEye.position - newPosition;

            direction = new Vector3(direction.x, 0, direction.z).normalized;

            direction = direction * 100;

            float x = direction.x < 0 ? Mathf.Clamp(direction.x, -tableDimensions.x, 0) : Mathf.Clamp(direction.x, 0, tableDimensions.x);
            float z = direction.z < 0 ? Mathf.Clamp(direction.z, -tableDimensions.z, 0) : Mathf.Clamp(direction.z, 0, tableDimensions.z);
            direction = new Vector3(x, 0, z);

            Debug.Log($"SceneSetup - Direction = {direction}");

            newPosition = newPosition + direction - (direction * tableInset);
        }

        workspaceAnchor.position = (newPosition);

        //rotation
        Vector3 workspaceDirection = centerEye.forward;
        workspaceDirection = new Vector3(workspaceDirection.x, 0, workspaceDirection.z).normalized;
        workspaceAnchor.rotation = Quaternion.LookRotation(workspaceDirection, Vector3.up);

        workspaceSet = true;
    }

    public void NoRoomSetUp()
    {
        if(workspaceSet) { return; }

        Vector3 workspaceDirection = centerEye.forward;
        workspaceDirection = new Vector3(workspaceDirection.x, 0, workspaceDirection.z).normalized;

        //position
        Vector3 newPosition = centerEye.position + workspaceDirection;
        newPosition = new Vector3(newPosition.x, 1, newPosition.z);

        //rotation
       
        workspaceAnchor.rotation = Quaternion.LookRotation(workspaceDirection, Vector3.up);

        workspaceSet = true;
    }
}
