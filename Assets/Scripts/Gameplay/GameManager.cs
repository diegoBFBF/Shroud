using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField]
    private bool isHost;

    public UnityEvent onPlayersReady;
    public UnityEvent onBoardReady;
    public UnityEvent onGameStarted;
    public UnityEvent onSetupPhaseBegin;
    public UnityEvent onSetupPhaseEnd;
    
    public PlayerEvent onRendererEvent;
    public PlayerEvent onTurnStart;
    public PlayerEvent onPlayerDefeated;
    public PlayerEvent onPlayerVictory;
    public UnityEvent onTie;

    [Header("Spaceship Prefabs")]
    public GameObject[] shipPrefab;

    public List<GameObject> obstaclePool = new List<GameObject>();

    public List<Spaceship>[] spaceships;
    private int[] playerIds;

    private int targetPlayers = 2;
    private int currentPlayersReady = 0;

    private void Awake()
    {
        if(onRendererEvent == null){
            onRendererEvent = new PlayerEvent();
        }
    }

    public void StartTurn(int playerId){
        
    }

    public void EndTurn(int playerId){

    }


    public void TranckSpaceship(Spaceship ship){
        spaceships[ship.GetComponent<OwnedByPlayer>().PlayerId].Add(ship);
    }

    public void HandleShipDestroyed(Spaceship ship){
        int playerIndex = ship.GetComponent<OwnedByPlayer>().PlayerId;
        spaceships[playerIndex].Remove(ship);
        if(spaceships[playerIndex].Count == 0){
            //Player X has run out of ships;
            OnPlayerDefeated(playerIndex);
        }

    }

    public void HandlePlayerSetupReady(int playerId, bool playerIsReady = true){
        currentPlayersReady += playerIsReady ? 1 : -1;
        if(currentPlayersReady == targetPlayers){
            onPlayersReady.Invoke();
        }
    }

    private void OnPlayerDefeated(int defeatedPlayer)
    {
        //Take a quick count of still alive players
        int alivePlayers = 0;
        int lastAlivePlayer = 0;
        for(int i = 0; i < playerIds.Length; i++){

            alivePlayers += spaceships[i].Count != 0 ? 1 : 0;
            if(spaceships[i].Count != 0){
                lastAlivePlayer = i;
            }
        }
        if(alivePlayers == 0){
            onTie.Invoke();
        }else if(alivePlayers == 1){
            onPlayerVictory.Invoke(lastAlivePlayer);
        }else{
            onPlayerDefeated.Invoke(defeatedPlayer);
        }
    }
}

public class GameManagerTester : MonoBehaviour{


}

[RequireComponent(typeof(OwnedByPlayer))]
public class SpaceshipCommander : MonoBehaviour{
    
    public int maxTokensPerTurn = 2; 
    public int currentTokens;
    OwnedByPlayer owner;

    private void Awake()
    {
        owner = GetComponent<OwnedByPlayer>();
    }


    private void OnEnable(){
        GameManager.Instance.onGameStarted.AddListener(HandleGameStart);
        GameManager.Instance.onPlayerDefeated.AddListener(HandleDefeat);
        GameManager.Instance.onPlayersReady.AddListener(StartSetupPhase);
    }

    private void HandleDefeat(int playerId)
    {
        if(playerId == owner.PlayerId){
            //Yay I won
        }
    }

    private void HandleGameStart()
    {
        StartSetupPhase();
    }

    public void MarkMyselfReady(bool amIready){
        GameManager.Instance.HandlePlayerSetupReady(owner.PlayerId, amIready);
    }

    private void StartSetupPhase()
    {
        //Place object in grids
    }

    private void StartPrepPhase(){

    }

    //public void 


    public void MarkSetupReady(){
        GameManager.Instance.HandlePlayerSetupReady(owner.PlayerId);
    }


}
