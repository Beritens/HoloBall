using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;
using TMPro;

public class initialize : NetworkBehaviour
{
    [SerializeField] Color32 color1;
    [SerializeField] Color32 color2;
    [ColorUsageAttribute(true,true)]
    [SerializeField] Color glowColor1;
    [ColorUsageAttribute(true,true)]
    [SerializeField] Color glowColor2;
    [SerializeField] Renderer rend;
    private NetworkManagerLobby room;
    [SyncVar(hook = nameof(TeamChange))]
    private int team;
    [SyncVar(hook = nameof(NameChange))]
    private string playerName;
    [SerializeField] TextMeshPro nameText;

    private NetworkManagerLobby Room
    {
        get{
            if(room != null){return room;}
            return room = NetworkManager.singleton as NetworkManagerLobby;
        }
    }
    void Start()
    {
        if(isServer)
            GetInfo();
        if(hasAuthority){
            nameText.transform.parent.gameObject.SetActive(false);
        }
        
    }
    public void TeamChange(int oldValue, int newValue){
        //change the color based on the team
        rend.materials[0].color=newValue==1?color1:color2;
        rend.materials[0].SetColor("_EmissionColor",newValue==1?glowColor1:glowColor2);
    }
    void GetInfo(){
        //change the synced vairable on the server
        NetworkGamePlayer p= null;
        foreach (NetworkGamePlayer player in Room.GamePlayers)
        {
            if(player.connectionToClient==connectionToClient){
                p=player;
            }
        }
        team=p.team;
        playerName = p.DisplayName;
    }
    void NameChange(string oldValue, string newValue){
        nameText.text = (team== 1? "<color=#0096FF>":"<color=#FF9600>")+playerName+"</color>";
    }
}
