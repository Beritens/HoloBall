using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
public class NetworkManagerLobby : NetworkManager
{
    //https://www.youtube.com/watch?v=Fx8efi2MNz0&t=539s
    [SerializeField] private int minPlayers = 2;
    [SerializeField]private string menuScene="menu";
    [SerializeField]private string gameScene= "Game";
    [SerializeField]public string lobbyScene= "Lobby";
    [Header("Room")]
    [SerializeField]private NetworkRoomPlayer roomPlayerPrefab = null;
    [Header("Game")]
    [SerializeField]private NetworkGamePlayer gamePlayerPrefab = null;

    [SerializeField]private GameObject playerSpawnSystem = null;
    [SerializeField]private LobbyManager lobbyManagerPrefab;
    [SerializeField]public GameObject inGamePlayer;
    public static event Action OnClientConnected;
    public static event Action OnClientDisconnected;
    public static event Action OnGameSceneStarted;
    public static event Action<NetworkConnection> OnServerReadied;
    public List<NetworkRoomPlayer> RoomPlayers = new List<NetworkRoomPlayer>();
    public List<NetworkGamePlayer> GamePlayers = new List<NetworkGamePlayer>(); 
    public int score;
    public int time;
    
    // Start is called before the first frame update
    public override void OnStartServer()
    {
        spawnPrefabs=Resources.LoadAll<GameObject>("SpawnablePrefabs").ToList();
        //ServerChangeScene(lobbyScene);
    }
    public override void OnStartClient()
    {
        var spawnablePrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs");
        foreach(var prefab in spawnablePrefabs){
            ClientScene.RegisterPrefab(prefab);
        }
    }
    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        //SceneManager.LoadScene(lobbyScene);
        OnClientConnected?.Invoke();
    }
    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        OnClientDisconnected?.Invoke();
    }
    public override void OnStopClient()
    {
        base.OnStopClient();
        //SceneManager.LoadScene(menuScene);
        OnGameSceneStarted = null;
        OnServerReadied = null;
    }

    
    public override void OnServerConnect(NetworkConnection conn)
    {
        if(numPlayers >= maxConnections){
            conn.Disconnect();
            return;
        }
        if(SceneManager.GetActiveScene().name != lobbyScene){
            conn.Disconnect();
            return;
        }

    }
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        if(SceneManager.GetActiveScene().name ==menuScene||SceneManager.GetActiveScene().name ==lobbyScene){
            bool isLeader= RoomPlayers.Count==0;
            NetworkRoomPlayer roomPlayerInstance = Instantiate(roomPlayerPrefab);
            roomPlayerInstance.IsLeader = isLeader;
            NetworkServer.AddPlayerForConnection(conn,roomPlayerInstance.gameObject);
            NotifyPlayersOfReadyState();
            
        }
    }
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        if(conn.identity != null){
            var player = conn.identity.GetComponent<NetworkRoomPlayer>();
            RoomPlayers.Remove(player);
            NotifyPlayersOfReadyState();
        }
        base.OnServerDisconnect(conn);
    }
    
    public override void OnStopServer()
    {
        RoomPlayers.Clear();
        GamePlayers.Clear();
    }
    public void NotifyPlayersOfReadyState(){
        foreach(var player in RoomPlayers){
            player.HandleReadyToStart(IsReadyToStart());
        }
    }
    bool IsReadyToStart(){
        //decided that you can just start when you whenever you want, no need to check if there are enough players
        return true;
        // if(numPlayers<minPlayers){return false;}
        // int A= 0;
        // int B= 0;
        // foreach (NetworkRoomPlayer player in RoomPlayers)
        // {
        //     if(player.team==1){
        //         A++;
        //     }
        //     if(player.team==2){
        //         B++;
        //     }
        // }
        // if(A>0 && B>0){
        //     return true;
        // }
        // else{
        //     return false;
        // }
        
    }
    public void StartGame(){
        if(SceneManager.GetActiveScene().name== lobbyScene){
            if(!IsReadyToStart()){return;}

            ServerChangeScene(gameScene);
        }
    }
    public void EndMatch(){
        if(SceneManager.GetActiveScene().name==gameScene){
            ServerChangeScene(lobbyScene);
        }
    }
    public override void ServerChangeScene(string newSceneName)
    {
        if(SceneManager.GetActiveScene().name == lobbyScene && newSceneName == gameScene){
            for(int i= RoomPlayers.Count-1; i>=0; i--){
                var conn = RoomPlayers[i].connectionToClient;
                var gameplayerInstance = Instantiate(gamePlayerPrefab);
                gameplayerInstance.SetDisplayName(RoomPlayers[i].DisplayName);
                gameplayerInstance.SetTeam(RoomPlayers[i].team);
                gameplayerInstance.isLeader=RoomPlayers[i].IsLeader;
                //NetworkServer.Destroy(conn.identity.gameObject);
                
                NetworkServer.ReplacePlayerForConnection(conn,gameplayerInstance.gameObject);
                NetworkServer.Destroy(RoomPlayers[i].gameObject);

            }
            RoomPlayers.Clear();
        }
        else if(SceneManager.GetActiveScene().name == gameScene && newSceneName == lobbyScene){
            for(int i= GamePlayers.Count-1; i>=0; i--){
                var conn = GamePlayers[i].connectionToClient;
                bool isLeader= GamePlayers[i].isLeader;
                var roomPlayerInstance = Instantiate(roomPlayerPrefab);
                roomPlayerInstance.IsLeader = isLeader;
                roomPlayerInstance.DisplayName = GamePlayers[i].DisplayName;
                roomPlayerInstance.team = GamePlayers[i].team;
                //NetworkServer.Destroy(conn.identity.gameObject);
                
                NetworkServer.ReplacePlayerForConnection(conn,roomPlayerInstance.gameObject);
                NetworkServer.Destroy(GamePlayers[i].gameObject);

            }
            NotifyPlayersOfReadyState();
            GamePlayers.Clear();
        }
        base.ServerChangeScene(newSceneName);
    }
    public override void OnServerReady(NetworkConnection conn)
    {
        base.OnServerReady(conn);

        OnServerReadied?.Invoke(conn);
    }
    // public override void OnServerChangedScene(string newSceneName)
    // {
    //     if(networkSceneName == gameScene){
    //         GameObject playerSpawnSystemInstance = Instantiate(playerSpawnSystem);
    //         NetworkServer.Spawn(playerSpawnSystemInstance);

    //     }
    // }
    public override void OnServerSceneChanged(string sceneName)
    {
        base.OnServerSceneChanged(sceneName);
        if(sceneName == gameScene){
            GameObject playerSpawnSystemInstance = Instantiate(playerSpawnSystem);
            NetworkServer.Spawn(playerSpawnSystemInstance);
            
        }
    }
    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        base.OnClientSceneChanged(conn);
        
        if(SceneManager.GetActiveScene().name==gameScene){
            OnGameSceneStarted?.Invoke();
        }
         
    }
}
