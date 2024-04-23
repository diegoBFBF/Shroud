using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    public string playerID;
    public string displayName;

    PlayerManager playerManager;
    PlayerManagers playerManagers;

    [SerializeField]
    GameObject playerSettingsPrefab;

    private void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
        playerManagers = GetComponent<PlayerManagers>();
        if (PlayerLists.Instance) PlayerLists.Instance.AddPlayer(playerManager);
    }


    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            LocalPlayerManager.Instance.player = playerManager;

            displayName = NetworkSettings.Instance.data.displayName;
            playerID = AuthenticationService.Instance.PlayerId;
            UpdateDisplayNameServerRpc(displayName, playerID);

            InitializeLocalPlayer();
            //VivoxManager.Instance.headPos = mainCamera.transform;

        }
        else
        {
            DisableLocalComponents();
            GetDisplayNameServerRpc();
        }
    }

    void InitializeLocalPlayer()
    {
        playerManagers.trackersManager.InitializeLocalPlayer();
    }

    void DisableLocalComponents()
    {
        
    }

    #region DisplayName & PlayerID RPCs
    [ServerRpc]
    public void UpdateDisplayNameServerRpc(string newName, string newID)
    {
        UpdateDisplayNameClientRpc(newName, newID);
        displayName = newName;
        playerID = newID;
        //Debug.Log("update name server");
    }


    [ClientRpc]
    void UpdateDisplayNameClientRpc(string newName, string newID)
    {
        displayName = newName;
        playerID = newID;
        //Debug.Log("update name client");
    }

    [ServerRpc(RequireOwnership = false)]
    void GetDisplayNameServerRpc()
    {
        UpdateDisplayNameClientRpc(displayName, playerID);
    }
    #endregion

}
