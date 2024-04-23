using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTrackersManager : MonoBehaviour
{
    [SerializeField]
    public Transform headTracker;
    [SerializeField]
    public Transform lHandTracker;
    [SerializeField]
    public Transform rHandTracker;

    Transform rigHead;
    Transform rigLHand;
    Transform rigRHand;

    public void InitializeLocalPlayer()
    {
        OVRCameraRig rig = FindObjectOfType<OVRCameraRig>();
        rigHead = rig.centerEyeAnchor;
        rigLHand = rig.leftHandAnchor;
        rigRHand = rig.rightHandAnchor;
        StartCoroutine(TrackRigRoutine());
    }

    IEnumerator TrackRigRoutine()
    {
        bool tracking = true;
        while (tracking)
        {
            headTracker.SetPositionAndRotation(rigHead.position, rigHead.rotation);
            lHandTracker.SetPositionAndRotation(rigLHand.position, rigLHand.rotation);
            rHandTracker.SetPositionAndRotation(rigRHand.position, rigRHand.rotation);
            yield return null;
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
