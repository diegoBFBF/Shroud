using DilmerGames.Core.Singletons;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class LobbyManager : Singleton<LobbyManager>
{
    [SerializeField]
    bool deleteLobby;
    [SerializeField]
    string deleteLobbyID;

    ConcurrentQueue<string> createdLobbyIds = new ConcurrentQueue<string>();
    ConcurrentQueue<string> joinedLobbyIds = new ConcurrentQueue<string>();

    public Lobby currentLobby;//doesnt update//only use for data set on start

    bool alreadyLeaving;

    IEnumerator heartbeat;

    // Start is called before the first frame update
    void Start()
    {
        SceneManager.sceneUnloaded += SceneUnloaded;
    }

    // Update is called once per frame
    void Update()
    {
        if (deleteLobby)
        {
            deleteLobby = false;
            if (!string.IsNullOrWhiteSpace(deleteLobbyID))
            {
                Lobbies.Instance.DeleteLobbyAsync(deleteLobbyID);
            }
        }
    }

    #region Create
    
    public async Task<bool> CreateHostLobby(bool isPrivate, int maxPlayers)
    {
        string lobbyName = NetworkSettings.GetRandomServerName();

        CreateLobbyOptions options = new CreateLobbyOptions();
        options.IsPrivate = isPrivate;
        options.Data = GenerateLobbyData();

        try
        {
            currentLobby = await Lobbies.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);
            createdLobbyIds.Enqueue(currentLobby.Id);
            Debug.Log($"created host lobby {currentLobby.Name}. Max player count = {maxPlayers}");
        }
        catch (LobbyServiceException e)
        {
            Debug.Log("LobbyManager.CreateLobby.CreateLobbyAsync.LobbyServiceException: " + e.Message);
            return false;
        }

        StartCoroutine(heartbeat = HeartbeatLobbyCoroutine(currentLobby.Id, 15));
        InvokeRepeating("CheckLobby", 10f, 10f);
        return true;
    }


    Dictionary<string, DataObject> GenerateLobbyData()
    {
        return new Dictionary<string, DataObject>()
        {
            {
                "joinCode", new DataObject(
                    visibility: DataObject.VisibilityOptions.Public,
                    value: RelayManager.Instance.joinCode,
                    index: DataObject.IndexOptions.S1)
            },
            {
                "gameVersion", new DataObject(
                    visibility: DataObject.VisibilityOptions.Public,
                    value: NetworkSettings.Instance.data.versionNumber,
                    index: DataObject.IndexOptions.S2)
            },
        };
    }

    #endregion

    #region Find
    
    public async Task<string> FindRandomHost()//returns joinCode
    {
        var queryOptions = new QueryLobbiesOptions
        {
            SampleResults = false, // Paging cannot use randomized results
            Filters = new List<QueryFilter>
            {
                     new QueryFilter(
                    field: QueryFilter.FieldOptions.AvailableSlots,
                    op: QueryFilter.OpOptions.GT,
                    value: "0"),

                    new QueryFilter(
                    field: QueryFilter.FieldOptions.S2,
                    op: QueryFilter.OpOptions.EQ,
                    value: NetworkSettings.Instance.data.versionNumber),

            },
            Order = new List<QueryOrder>
            {
                new QueryOrder(false, QueryOrder.FieldOptions.Created),
            }
        };

        QueryResponse response;
        try
        {
            response = await Lobbies.Instance.QueryLobbiesAsync(queryOptions);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log("LobbyManager.FindRandomServer.QueryLobbiesAsync.LobbyServiceException: " + e.Message);
            StartMenuOptions.Instance.StartErrorRoutine(StartMenuOptions.ErrorType.Error);
            return null;
        }

        if (response.Results.Count <= 0)
        {
            Debug.Log("LobbyManager.FindRandomHost found no lobbies");
            StartMenuOptions.Instance.StartErrorRoutine(StartMenuOptions.ErrorType.NoLobbies);

            return null;
        }

        string joinLobby = response.Results[0].Data["joinCode"].Value;
        return joinLobby;
    }
    public async Task<string> FindLobbyWithJoinCodeData(string joinCode)//returns lobbyId
    {
        var queryOptions = new QueryLobbiesOptions
        {
            SampleResults = false, // Paging cannot use randomized results
            Filters = new List<QueryFilter>
            {
                    new QueryFilter(
                    field: QueryFilter.FieldOptions.S1,
                    op: QueryFilter.OpOptions.EQ,
                    value: joinCode),
            },
            Order = new List<QueryOrder>
            {
                new QueryOrder(false, QueryOrder.FieldOptions.Created),
            }
        };

        QueryResponse response;
        try
        {
            response = await Lobbies.Instance.QueryLobbiesAsync(queryOptions);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log("LobbyManager.FindLobbyWithJoinCodeData.QueryLobbiesAsync.LobbyServiceException: " + e.Message);
            return null;
        }
        if (response.Results.Count <= 0)
        {
            Debug.Log($"LobbyManager.FindLobbyWithJoinCodeData: Room of {joinCode} not found");
            return null;
        }
        Debug.Log($"LobbyManager.FindLobbyWithJoinCodeData found lobby {response.Results[0].LobbyCode}");
        return response.Results[0].Id;
    }

    public async Task<(string, bool)> FindLobbyWithLobbyCode(string lobbyCode)//returns joinCode, isPrivate
    {
        bool joinedLobby = await JoinLobbyByCode(lobbyCode);
        if (!joinedLobby)
        {
            return (null, false);
        }

        string joinCode = currentLobby.Data["joinCode"].Value;
        bool isPrivate = currentLobby.IsPrivate;

        if (currentLobby.Data["gameVersion"].Value != NetworkSettings.Instance.data.versionNumber)
        {
            joinCode = null;
            return (null, false);
        }

        bool leftLobby = await RemoveLocalPlayerFromCurrentLobby();
        if (!leftLobby)
        {
            //server should remove player from lobby if this fails
            return (null, false);
        }

        return (joinCode, isPrivate);
    }
    #endregion

    #region Get List

    public async Task<List<Lobby>> GetHostLobbies(int page, int count)
    {
        QueryLobbiesOptions options = new QueryLobbiesOptions();
        options.Count = count;

        options.Skip = page * count;

        // Filter for open lobbies only
        options.Filters = new List<QueryFilter>()
        {
            new QueryFilter(
                    field: QueryFilter.FieldOptions.AvailableSlots,
                    op: QueryFilter.OpOptions.LT,
                    value: $"{NetworkSettings.maxHostConnections}"),
             
        };

        // Order by newest lobbies first
        options.Order = new List<QueryOrder>()
        {
            new QueryOrder(false, QueryOrder.FieldOptions.S3),
                new QueryOrder(
                    asc: false,
                    field: QueryOrder.FieldOptions.Created)
        };

        QueryResponse lobbies = null;
        try
        {
            lobbies = await Lobbies.Instance.QueryLobbiesAsync(options);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }

        if (lobbies == null) return null;

        return lobbies.Results;
    }
    #endregion

    #region Join

    public async Task<bool> JoinLobbyById(string id)
    {
        // check version number before this!
        try
        {
            currentLobby = await Lobbies.Instance.JoinLobbyByIdAsync(id);
            joinedLobbyIds.Enqueue(currentLobby.Id);
            Debug.Log("Joined lobby: " + currentLobby.Id);
            return true;
        }
        catch(LobbyServiceException e)
        {
            switch (e.Reason)
            {
                case LobbyExceptionReason.LobbyFull: break;
                default: break;
            }
            Debug.Log("LobbyServiceException: " + e.Message);
            return false;
        }
    }

    public async Task<bool> JoinLobbyByCode(string lobbyCode)
    {
        try
        {
            currentLobby = await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyCode);//join lobby first bc private lobbies have protected data
        }
        catch (LobbyServiceException e)
        {
            switch (e.Reason)
            {
                case LobbyExceptionReason.LobbyFull: break;
                default: break;
            }
            Debug.Log("LobbyServiceException: " + e.Message);
            return false;
        }


        if (currentLobby.Data["gameVersion"].Value != NetworkSettings.Instance.data.versionNumber)
        {
            try
            {
                await Lobbies.Instance.RemovePlayerAsync(currentLobby.Id, AuthenticationService.Instance.PlayerId);
                Debug.Log("removed player bc outdated version");
            }
            catch(LobbyServiceException e)
            {
                Debug.Log("LobbyManager.JoinLobbyByCode.RemovePlayerAsync: " + e);//Hopefully server removes player
            }
            
            currentLobby = null;
            return false;
        }

        joinedLobbyIds.Enqueue(currentLobby.Id);
        return true;
    }

    #endregion

    IEnumerator HeartbeatLobbyCoroutine(string lobbyId, float waitTimeSeconds)
    {
        var delay = new WaitForSecondsRealtime(waitTimeSeconds);
        while (currentLobby != null)
        {
            Lobbies.Instance.SendHeartbeatPingAsync(lobbyId);
            yield return delay;
        }
    }

    async Task<bool> CheckLobbyGameVersion(string lobbyName)
    {
        var queryOptions = new QueryLobbiesOptions
        {
            SampleResults = false, // Paging cannot use randomized results
            Filters = new List<QueryFilter>
            {
                new QueryFilter(
                field: QueryFilter.FieldOptions.Name,
                op: QueryFilter.OpOptions.EQ,
                value: lobbyName),

                new QueryFilter(
                field: QueryFilter.FieldOptions.S2,
                op: QueryFilter.OpOptions.EQ,
                value: NetworkSettings.Instance.data.versionNumber),
            },
            Order = new List<QueryOrder>
            {
                new QueryOrder(true, QueryOrder.FieldOptions.Created),
            }
        };
        QueryResponse response = await Lobbies.Instance.QueryLobbiesAsync(queryOptions);

        if (response.Results.Count > 0) { Debug.Log("Found lobby with compatible game version"); return true; }

        Debug.Log($"Failed to join lobby {lobbyName}, incompatible game version");
        return false;

        //old method
        //lobby = await Lobbies.Instance.JoinLobbyByCodeAsync(MainMenuInputFields.Instance.joinCode.text);
        //Debug.Log($"comparing {lobby.Data["gameVersion"].Value} and  {versionNumber}");
        //if (!String.Equals(checkLobby.Data["gameVersion"].Value, NetworkSettings.Instance.data.versionNumber))
        //{
        //    StartCoroutine(MainMenuOptions.Instance.DisplayError(MainMenuOptions.ErrorType.IncompatibleVersion));
        //    string playerId = AuthenticationService.Instance.PlayerId;
        //    await Lobbies.Instance.RemovePlayerAsync(lobby.Id, playerId);
        //    lobby = null;
        //    Debug.Log("removed player bc outdated version");
        //    return;
        //}
    }

    [SerializeField]
    List<string> strikedIDs = new List<string>();
    int nullLobbyStrikes;
    async void CheckLobby() //invoke repeating//only on server
    {
        if(PlayerLists.Instance == null)
        {
            nullLobbyStrikes++;
            if (nullLobbyStrikes >= 3)
            {
                Debug.Log($"LobbyManager.CheckLobby couldnt find PlayerLists.Instance {nullLobbyStrikes} times. Leaving Room.");
                NetworkOptions.Instance.LeaveRoom();
            }
            return;
        }

        if (this.currentLobby == null)
        {
            nullLobbyStrikes++;
            if (nullLobbyStrikes >= 3)
            {
                Debug.Log($"LobbyManager.CheckLobby couldnt find PlayerLists.Instance {nullLobbyStrikes} times. Leaving Room.");
                NetworkOptions.Instance.LeaveRoom();
            }
            return;
        }

        Lobby currentLobby;
        try
        {
            currentLobby = await Lobbies.Instance.GetLobbyAsync(this.currentLobby.Id);
        }
        catch
        {
            nullLobbyStrikes++;
            if (nullLobbyStrikes >= 3)
            {
                Debug.Log($"LobbyManager.CheckLobby couldnt find PlayerLists.Instance {nullLobbyStrikes} times. Leaving Room.");
                NetworkOptions.Instance.LeaveRoom();
            }
            return;
        }

        if (PlayerLists.Instance.players.Count <= 0)
        {
            //host should never have 0 because they should always be in the lobby.
            nullLobbyStrikes++;
            if (nullLobbyStrikes >= 3)
            {
                Debug.Log($"LobbyManager.CheckLobby couldnt find PlayerLists.Instance {nullLobbyStrikes} times. Leaving Room.");
                NetworkOptions.Instance.LeaveRoom();
            }
        }
        else
        {
            nullLobbyStrikes = 0;
        }

        List<string> lobbyPlayerIDs = currentLobby.Players.Select(p => p.Id).ToList();

        HashSet<string> playerIDs = new HashSet<string>(PlayerLists.Instance.players.Select(p => p.networkPlayer.playerID));

        Debug.Log($"Host/Server checking lobby of {lobbyPlayerIDs.Count}, with {playerIDs.Count} players...");

        for (int i = strikedIDs.Count - 1; i >= 0; i--)
        {
            if (!playerIDs.Contains(strikedIDs[i]) && !lobbyPlayerIDs.Contains(strikedIDs[i]))//if playerID doesnt exist in both lists, remove from strikedID list
            {
                strikedIDs.RemoveAt(i);
                continue;
            }
            else if (playerIDs.Contains(strikedIDs[i]) && lobbyPlayerIDs.Contains(strikedIDs[i]))//if playerID exists in both lists, remove from strikedID list
            {
                strikedIDs.RemoveAt(i);
                continue;
            }
            else if (!playerIDs.Contains(strikedIDs[i]) && lobbyPlayerIDs.Contains(strikedIDs[i]))//if playerID doesnt exist in player list, remove player
            {
                try
                {
                    await Lobbies.Instance.RemovePlayerAsync(this.currentLobby.Id, strikedIDs[i]);
                    Debug.Log($"Host/Server removed {strikedIDs[i]} from lobby");
                    strikedIDs.RemoveAt(i);
                    continue;
                }
                catch (LobbyServiceException e)
                {
                    Debug.Log($"Host/Server failed to remove {strikedIDs[i]} from lobby.\n{e.Message}");
                }
            }
            else if (playerIDs.Contains(strikedIDs[i]) && !lobbyPlayerIDs.Contains(strikedIDs[i]))//if playerID doesnt exist in lobby list, add player?
            {
                //client rpc call?
            }
        }

        foreach (string id in lobbyPlayerIDs)
        {
            if (AuthenticationService.Instance.PlayerId == id) continue;//never remove host/server

            if (!playerIDs.Contains(id))//if playerID doesnt exist in player list, add playerID to strike list
            {
                if (!strikedIDs.Contains(id)) strikedIDs.Add(id);
            }//else to add player? should be added when they join tho
        }

       
    }

    public async void ServerRemovePlayerFromLobby(string playerId)//for server/host
    {
        if (currentLobby == null) { Debug.Log("failed to remove player: currentLobby = null"); return; }
        try 
        { 
            await Lobbies.Instance.RemovePlayerAsync(currentLobby.Id, playerId);
            Debug.Log($"Host removed {playerId} from lobby"); 
        }
        catch { Debug.Log("failed to remove player"); }

    }

    public async Task<bool> RemoveLocalPlayerFromCurrentLobby()
    {
        if (currentLobby == null) { Debug.Log("failed to remove player: currentLobby = null"); return false; }
        try
        {
            await Lobbies.Instance.RemovePlayerAsync(currentLobby.Id, AuthenticationService.Instance.PlayerId);
            currentLobby = null;
            Debug.Log($"removed local from current lobby");
            return true;
        }
        catch(LobbyServiceException e) { Debug.Log($"failed to remove player: {e}"); return false; }

    }

    async void DeleteCreatedLobbies()
    {
        await DeleteCreatedLobbiesAsync();
    }
    public async Task DeleteCreatedLobbiesAsync()
    {
        while (createdLobbyIds.TryDequeue(out var lobbyId))
        {
            await Lobbies.Instance.DeleteLobbyAsync(lobbyId);
            Debug.Log("deleted lobby: " + lobbyId);
            if (currentLobby != null)
            {
                if (currentLobby.Id == lobbyId) currentLobby = null;
            }
        }
    }

    public async void RemovePlayerFromLobbies()
    {
        await RemovePlayerFromLobbiesAsync();
    }
    async Task RemovePlayerFromLobbiesAsync()
    {
        while (joinedLobbyIds.TryDequeue(out var lobbyId))
        {
            await LobbyService.Instance.RemovePlayerAsync(lobbyId, AuthenticationService.Instance.PlayerId);
            Debug.Log("local player removed from lobby:" + lobbyId);
            if(currentLobby != null)
            {
                if (currentLobby.Id == lobbyId) currentLobby = null;
            }
        }
    }


    private void SceneUnloaded(Scene prevScene)
    {
        if(prevScene.name == "BackroomsScene")//Reset Lobby data after leaving multiplayer
        {
            currentLobby = null;
            if (heartbeat != null) { StopCoroutine(heartbeat); Debug.Log("Heartbeat Stopped"); }
            CancelInvoke("CheckLobby");
            RemovePlayerFromLobbies();
            DeleteCreatedLobbies();
        }
    }

    void OnApplicationQuit()
    {
        while (createdLobbyIds.TryDequeue(out var lobbyId))
        {
            LobbyService.Instance.DeleteLobbyAsync(lobbyId);
            Debug.Log("deleted lobby: " + lobbyId);
        }
        while (joinedLobbyIds.TryDequeue(out var lobbyId))
        {
            LobbyService.Instance.RemovePlayerAsync(lobbyId, AuthenticationService.Instance.PlayerId);
            Debug.Log("local player removed from lobby:" + lobbyId);
        }
    }


    
}
