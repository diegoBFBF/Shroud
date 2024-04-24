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
    
    public PlayerEvent onRendererEvent;
    public PlayerEvent onTurnStart;
    public PlayerEvent onPlayerDefeated;
    public PlayerEvent onPlayerVictory;
    public UnityEvent onTie;

    public List<Spaceship>[] spaceships;
    private int[] playerIds;

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
        spaceships[ship.GetComponent<OnwedByPlayer>().PlayerId].Add(ship);
    }

    public void HandleShipDestroyed(Spaceship ship){
        int playerIndex = ship.GetComponent<OnwedByPlayer>().PlayerId;
        spaceships[playerIndex].Remove(ship);
        if(spaceships[playerIndex].Count == 0){
            //Player X has run out of ships;
            OnPlayerDefeated(playerIndex);
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