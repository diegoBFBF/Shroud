using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManagers : MonoBehaviour
{
    [HideInInspector]
    public PlayerTrackersManager trackersManager;

    private void Awake()
    {
        trackersManager = GetComponentInChildren<PlayerTrackersManager>();
    }
}
