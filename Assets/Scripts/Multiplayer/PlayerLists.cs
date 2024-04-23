using DilmerGames.Core.Singletons;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLists : Singleton<PlayerLists>
{
    public List<PlayerManager> players = new List<PlayerManager>();

    public void AddPlayer(PlayerManager player)
    {
        players.Add(player);
    }
}
