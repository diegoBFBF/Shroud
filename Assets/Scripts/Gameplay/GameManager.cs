using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField]
    private bool isHost;

    UnityEvent onGameStarted;
    public PlayerEvent onRendererEvent;

    private void Awake()
    {
        if(onRendererEvent == null){
            onRendererEvent = new PlayerEvent();
        }
    }

}

public class GameManagerTester : MonoBehaviour{

    
}