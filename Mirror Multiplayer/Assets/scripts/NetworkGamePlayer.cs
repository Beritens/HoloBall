using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;

public class NetworkGamePlayer : NetworkBehaviour
{
   //https://www.youtube.com/watch?v=Fx8efi2MNz0&t=539s

    [SyncVar]
    public string DisplayName = "";
    [SyncVar]
    public int team = 0;
    public bool isLeader = false;
    [SerializeField] Menu PlayerUI;
    private NetworkManagerLobby room;
    private NetworkManagerLobby Room
    {
        get{
            if(room != null){return room;}
            return room = NetworkManager.singleton as NetworkManagerLobby;
        }
    }
    void Awake()
    {
        NetworkManagerLobby.OnGameSceneStarted+=StartGame;
    }
    public override void OnStartClient()
    {
        DontDestroyOnLoad(gameObject);
        Room.GamePlayers.Add(this);
        
    }
    public override void OnStopClient()
    {
        Room.GamePlayers.Remove(this);
    }
    [Server]
    public void SetDisplayName(string displayName){
        this.DisplayName=displayName;
    }
    [Server]
    public void SetTeam(int team){
        this.team = team;
    }
    public void StartGame(){
        if(hasAuthority){
            Menu menu = GameObject.Instantiate(PlayerUI);
            
            menu.Setup(this,isLeader);
        }
    }
    public void Disconnect(){
        if(isServer){
            Room.StopHost();
        }
        Room.StopClient();
    }
    public void EndMatch(){
        if(!isServer){return;}

        Room.EndMatch();
        
    }
   
}