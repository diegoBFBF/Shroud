using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

public class SceneSetUp : MonoBehaviour
{
    [SerializeField]
    OVRSceneManager sceneManager;

    [SerializeField]
    NoteTaker noteTaker;


    private void Awake()
    {
        sceneManager.SceneModelLoadedSuccessfully += OnSceneLoaded;
    }

    private async void OnSceneLoaded()
    {
        Debug.Log("SceneSetUp - OnSceneLoaded");
        //OVRSceneRoom room = FindObjectOfType<OVRSceneRoom>();

        //Set Note Pad Position
        var anchors = new List<OVRAnchor>();
        await OVRAnchor.FetchAnchorsAsync<OVRRoomLayout>(anchors);

        // no rooms - call Space Setup or check Scene permission
        if (anchors.Count == 0)
            return;

        // access anchor data by retrieving the components
        var room = anchors.First();

        // access the ceiling, floor and walls with the OVRRoomLayout component
        var roomLayout = room.GetComponent<OVRRoomLayout>();
        if (roomLayout.TryGetRoomLayout(out Guid ceiling, out Guid floor, out Guid[] walls))
        {
            // use these guids to fetch the OVRAnchor object directly
            await OVRAnchor.FetchAnchorsAsync(walls, anchors);
        }

        // access the list of children with the OVRAnchorContainer component
        if (!room.TryGetComponent(out OVRAnchorContainer container))
            return;

        // use the component helper function to access all child anchors
        await container.FetchChildrenAsync(anchors);

        foreach (var anchor in anchors)
        {
            // check that this anchor is the table
            if (!anchor.TryGetComponent(out OVRSemanticLabels labels) ||
                !labels.Labels.Contains(OVRSceneManager.Classification.Table))
            {
                continue;
            }

            Debug.Log("SceneSetUp - Found Table" + anchor.Uuid);

            // get the table dimensions
            anchor.TryGetComponent(out OVRBounded3D bounded3D);
            var size = bounded3D.BoundingBox.size;

            Vector3 newPosition = bounded3D.BoundingBox.center;

            //newPosition += Vector3.up * size.y;

            noteTaker.SetPosition(newPosition);
            break;
        }
    }
}
