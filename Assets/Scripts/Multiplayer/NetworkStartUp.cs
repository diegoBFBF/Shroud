using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkStartUp : MonoBehaviour
{
    void Start()
    {
        if (NetworkSettings.serverData.isHost)
        {
            SetUpHost();
        }
        else
        {
            JoinRoom();
        }
    }


    async void SetUpHost()
    {
        bool setupRelay = await RelayManager.Instance.SetupRelay();
        if (!setupRelay)
        {
            NetworkSettings.disconnectError = NetworkSettings.ErrorType.HostError;
            SceneManager.LoadScene("StartingScene", LoadSceneMode.Single);
            return;
        }

        bool createdHostLobby = await LobbyManager.Instance.CreateHostLobby(NetworkSettings.serverData.isPrivate, NetworkSettings.maxHostConnections);
        if (!createdHostLobby)
        {
            NetworkSettings.disconnectError = NetworkSettings.ErrorType.HostError;
            SceneManager.LoadScene("StartingScene", LoadSceneMode.Single);
            return;
        }

        Debug.Log("NetworkManager: StartHost");
        NetworkManager.Singleton.StartHost();
        NetworkOptions.Instance.StartDisconnectListener(false);
        //VivoxManager.Instance.JoinChannel();
    }

    async void JoinRoom()
    {
        //join lobby
        bool joinedLobby = false;
        
        if (NetworkSettings.serverData.isPrivate)
        {
            if (NetworkSettings.serverData.lobbyCode == null)
            {
                Debug.Log("failed to join room: LobbyCode not set on NetworkSettings - Returning to starting scene");
                NetworkSettings.disconnectError = NetworkSettings.ErrorType.JoinError;
                SceneManager.LoadScene("StartingScene", LoadSceneMode.Single);
                return;
            }
            joinedLobby = await LobbyManager.Instance.JoinLobbyByCode(NetworkSettings.serverData.lobbyCode);
        }
        else
        {
            if (NetworkSettings.serverData.joinCode == null)
            {
                Debug.Log("failed to join room: JoinCode not set on NetworkSettings - Returning to starting scene");
                NetworkSettings.disconnectError = NetworkSettings.ErrorType.JoinError;
                SceneManager.LoadScene("StartingScene", LoadSceneMode.Single);
                return;
            }
            //check if public lobby exists
            string lobbyId = await LobbyManager.Instance.FindLobbyWithJoinCodeData(NetworkSettings.serverData.joinCode);
            if (lobbyId == null)
            {
                Debug.Log("failed to find lobby - Returning to starting scene");
                NetworkSettings.disconnectError = NetworkSettings.ErrorType.JoinError;
                SceneManager.LoadScene("StartingScene", LoadSceneMode.Single);
                return;
            }
            joinedLobby = await LobbyManager.Instance.JoinLobbyById(lobbyId);
        }
        
        if (!joinedLobby)
        {
            Debug.Log("failed to join lobby - Returning to starting scene");
            NetworkSettings.disconnectError = NetworkSettings.ErrorType.JoinError;
            SceneManager.LoadScene("StartingScene", LoadSceneMode.Single);
            return;
        }
        //after join lobby

        //set transport
        bool joinedRelay = await RelayManager.Instance.JoinRelay(LobbyManager.Instance.currentLobby.Data["joinCode"].Value);
        if (!joinedRelay)
        {
            Debug.Log("failed to join Relay - Returning to starting scene");
            NetworkSettings.disconnectError = NetworkSettings.ErrorType.JoinError;
            SceneManager.LoadScene("StartingScene", LoadSceneMode.Single);
            return;
        }

        //after transport set
        Debug.Log("NetworkManager: StartClient");
        NetworkManager.Singleton.StartClient();
        NetworkOptions.Instance.StartDisconnectListener(false);
        //VivoxManager.Instance.JoinChannel();
    }
}