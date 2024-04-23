using DilmerGames.Core.Singletons;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkOptions : Singleton<NetworkOptions>
{
    [SerializeField]
    bool joinRandomHost;
    [SerializeField]
    bool createPublic;
    [SerializeField]
    bool createPrivate;
    [SerializeField] 
    bool joinWithCode;

    [SerializeField] string joinCode;

    [SerializeField]
    bool leaveRoom;

    bool alreadyLeaving;

    NetworkSettings ns;

    UnityTransport transport;

    private void Awake()
    {
        ns = GetComponent<NetworkSettings>();
        transport = GetComponent<UnityTransport>();
    }

    private void Start()
    {
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void Update()
    {
        if (createPublic)
        {
            CreatePublicRoom();
            createPublic = false;
        }

        if (createPrivate)
        {
            CreatePrivateRoom();
            createPrivate = false;
        }

        if (joinRandomHost)
        {
            FindRandomHost();
            joinRandomHost = false;
        }

        if (joinWithCode)
        {
            JoinWithCode(joinCode);
            joinWithCode = false;
        }

        if (leaveRoom)
        {
            LeaveRoom();
            leaveRoom = false;
        }
       
    }
   

    #region Join Room
    public async void FindRandomHost()
    {
        //start loading screen
        //delay if needed because could join faster than loading sceen routine

        string joinCode = await LobbyManager.Instance.FindRandomHost();// StartSceneLoadingManager managed in here

        if (joinCode != null)
        {
            NetworkSettings.serverData.joinCode = joinCode;
            StartCoroutine(LoadScene(NewScene.Join));
        }
    }

    

    public async void JoinWithCode(string lobbyCode)//only used in editor //now handled in JoinByCodeManager
    {
        if (string.IsNullOrEmpty(lobbyCode))
        {
            Debug.Log("joinCode empty or null");
            return;
        }
        (string joinCode, bool isPrivate) = await LobbyManager.Instance.FindLobbyWithLobbyCode(lobbyCode);

        if (joinCode != null) 
        {
            NetworkSettings.serverData.isPrivate = isPrivate;
            if(isPrivate) NetworkSettings.serverData.lobbyCode = lobbyCode;
            else NetworkSettings.serverData.joinCode = joinCode;
            StartCoroutine(LoadScene(NewScene.Join));
        }
    }
    public void JoinRoomWithLobby(Lobby newLobby)
    {
        Debug.Log($"joining {newLobby.Name} from server list.");

        NetworkSettings.serverData.joinCode = newLobby.Data["joinCode"].Value;

        StartCoroutine(LoadScene(NewScene.Join));
    }
    
    #endregion

    #region Create Room
    public void CreatePublicRoom()
    {
        CreateHost(false);
    }

    public void CreatePrivateRoom()
    {
        CreateHost(true);
    }


    private void CreateHost(bool isPrivate)
    {
        NetworkSettings.serverData.isHost = true;
        NetworkSettings.serverData.isPrivate = isPrivate;
        StartCoroutine(LoadScene(NewScene.Join));
    }

    #endregion


    #region Leave Room
    bool intentionalLeave;
    public void LeaveRoom()
    {
        if (alreadyLeaving) return;
        alreadyLeaving = true;
        intentionalLeave = true;

        StartCoroutine(LoadScene(NewScene.Leave));
    }

    private void OnSceneUnloaded(Scene prevScene)
    {
        Debug.Log("NetworkOptions.SceneChanged.");
        if (prevScene.name == "BackroomsScene")
        {
            Debug.Log("NetworkOptions.SceneChanged. prevScene = BackroomsScene");
            StopDisconnectListener(false);
            NetworkManager.Singleton.Shutdown();
            Debug.Log("NetworkManager.Shutdown");
            //VivoxManager.Instance.LeaveChannel();
            transport.DisconnectLocalClient();
            //LobbyManager handles this itself
        }

        alreadyLeaving = false;
        intentionalLeave = false;
    }

    public void StartDisconnectListener(bool server)
    {
        if(!server) NetworkManager.Singleton.OnClientStopped += OnClientStopped;
        else NetworkManager.Singleton.OnServerStopped += OnServerStopped;
    }
    public void StopDisconnectListener(bool server)
    {
        if (!server) NetworkManager.Singleton.OnClientStopped -= OnClientStopped;
        else NetworkManager.Singleton.OnServerStopped -= OnServerStopped;
    }

    private void OnClientStopped(bool isHost)
    {
        Debug.Log("OnClientStopped called.");

        if (quiting || alreadyLeaving) return;

        if (!intentionalLeave) NetworkSettings.disconnectError = NetworkSettings.ErrorType.HostDisconnected;

        StartCoroutine(LoadScene(NewScene.Leave));
    }
    private void OnServerStopped(bool b)
    {
        Debug.Log("NetworkManager.OnServerStopped: Quitting application");
        Application.Quit();
    }

    #endregion

    enum NewScene { Join, Leave }
    IEnumerator LoadScene(NewScene newScene)
    {
        //fade 
        switch (newScene)
        {
            case NewScene.Join: SceneManager.LoadScene("MultiplayerScene", LoadSceneMode.Single); break;
            case NewScene.Leave: SceneManager.LoadScene("SampleScene", LoadSceneMode.Single); break;
        }
        yield return null;
    }


    bool quiting = false;
    void OnApplicationQuit()
    {
        quiting = true;
    }

}
