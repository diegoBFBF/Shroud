using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public NetworkPlayer networkPlayer;

    public PlayerManagers managers;

    private void Awake()
    {
        networkPlayer = GetComponent<NetworkPlayer>();
        managers = GetComponent<PlayerManagers>();
    }
}
