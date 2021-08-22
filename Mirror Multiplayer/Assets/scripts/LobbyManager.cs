using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class LobbyManager : NetworkBehaviour
{
    [SyncVar(hook= nameof(HandleChange))]
    public int time;
    [SyncVar(hook= nameof(HandleChange))]
    public int score;
    bool ready = false;
    private NetworkManagerLobby room;
    private NetworkManagerLobby Room
    {
        get{
            if(room != null){return room;}
            return room = NetworkManager.singleton as NetworkManagerLobby;
        }
    }
    void HandleChange(int oldValue, int newValue){
        Debug.Log(score);
        if(Room.RoomPlayers.Count>=1){
            Room.RoomPlayers[0].UpdateDisplay();
        }
        
        
        if(ready){
            Room.time = time;
            Room.score = score;
        }
        
    }
    void Start()
    {
        if(isServer){
            
            time = Room.time;
            score = Room.score;
            ready=true;
        }
    }
}
