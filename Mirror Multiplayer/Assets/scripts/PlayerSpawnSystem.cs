using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;
public class PlayerSpawnSystem : NetworkBehaviour
{
    //https://www.youtube.com/watch?v=Fx8efi2MNz0&t=539s
    private static List<Transform> spawnPoints1 = new List<Transform>();
    private static List<Transform> spawnPoints2 = new List<Transform>();
    private int nextIndex1 = 0;

    private int nextIndex2 = 0;
    private NetworkManagerLobby room;
    private NetworkManagerLobby Room
    {
        get{
            if(room != null){return room;}
            return room = NetworkManager.singleton as NetworkManagerLobby;
        }
    }
    public static void AddSpawnPoint(Transform point,int team){
        if(team==1){
            spawnPoints1.Add(point);
            spawnPoints1 = spawnPoints1.OrderBy(x => x.GetSiblingIndex()).ToList();
        }
        else{
            spawnPoints2.Add(point);
            spawnPoints2 = spawnPoints2.OrderBy(x => x.GetSiblingIndex()).ToList();
        }
        
    }
    public static void RemoveSpawnPoint(Transform point, int team){
        if(team == 1){
            spawnPoints1.Remove(point);
        }
        else{
            spawnPoints2.Remove(point);
        }
    }
    public override void OnStartServer()
    {
        NetworkManagerLobby.OnServerReadied += SpawnPlayer;
    }
    [ServerCallback]
    void OnDestroy()
    {
        NetworkManagerLobby.OnServerReadied -= SpawnPlayer;
    }

    [Server]
    public void SpawnPlayer(NetworkConnection conn){
        int t = 0;
        foreach (NetworkGamePlayer player in Room.GamePlayers)
        {
            if(player.connectionToClient == conn){
                t= player.team;
            }
        }
        Transform spawnPoint = transform;
        switch (t)
        {
            case 1:
                spawnPoint=spawnPoints1[nextIndex1];
                nextIndex1++;
                break;
            case 2:
                spawnPoint=spawnPoints2[nextIndex2];
                nextIndex2++;
                break;
            default:
                return;
        }
        GameObject playerInstance = Instantiate(Room.inGamePlayer, spawnPoint.position,spawnPoint.rotation);
        NetworkServer.Spawn(playerInstance,conn);


    }
}
