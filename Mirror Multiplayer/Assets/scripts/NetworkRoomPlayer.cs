using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NetworkRoomPlayer : NetworkBehaviour
{
    //https://www.youtube.com/watch?v=Fx8efi2MNz0&t=539s
    
    [Header("UI")]
    [SerializeField] private GameObject lobbyUI = null;
    [SerializeField] private Transform[] playerNameText = new Transform[4];
    [SerializeField] private Transform orange;
    [SerializeField] private Transform blue;
    [SerializeField] private Transform spectator;
    [SerializeField] private Button startGameButton = null;
    [SerializeField] TMP_InputField timeInput;
    [SerializeField] TMP_InputField scoreInput;
    
    

    [SyncVar(hook = nameof(HandleDisplayNameChanged))]
    public string DisplayName = "";
    [SyncVar(hook = nameof(HandleTeamChanged))]
    public int team = 0;
    private bool isLeader;
    public bool IsLeader{
        set{
            isLeader = value;
            startGameButton.gameObject.SetActive(value);
            timeInput.interactable=true;
            scoreInput.interactable=true;
        }
        get{
            return isLeader;
        }
    }
    private NetworkManagerLobby room;
    private NetworkManagerLobby Room
    {
        get{
            if(room != null){return room;}
            return room = NetworkManager.singleton as NetworkManagerLobby;
        }
    }

    public override void OnStartAuthority()
    {
        
        CmdSetDisplayName(PlayerNameInput.DisplayName);
        lobbyUI.SetActive(true);
    }
    public override void OnStartClient()
    {
        DontDestroyOnLoad(gameObject);
        //activate Cursor
        Cursor.lockState= CursorLockMode.None;
        Cursor.visible= true;
        Room.RoomPlayers.Add(this);

        UpdateDisplay();
    }
    public override void OnStopClient()
    {
        Room.RoomPlayers.Remove(this);
        UpdateDisplay();
    }
    public void HandleDisplayNameChanged(string oldValue, string newValue) => UpdateDisplay();
    public void HandleTeamChanged(int oldValue, int newValue) => UpdateDisplay();
    public void UpdateDisplay(){
        if(!hasAuthority){
            foreach (var player in Room.RoomPlayers)
            {
                if(player.hasAuthority){
                    player.UpdateDisplay();
                    break;
                }
            }
        }
        for(int i = 0; i<playerNameText.Length;i++){
            playerNameText[i].gameObject.SetActive(false);
        }
        for(int i = 0; i<Room.RoomPlayers.Count; i++){
            playerNameText[i].gameObject.SetActive(true);
            switch (Room.RoomPlayers[i].team)
            {
                case 0:
                    playerNameText[i].SetParent(spectator);
                    break;
                case 1:
                    playerNameText[i].SetParent(blue);
                    break;
                case 2:
                    playerNameText[i].SetParent(orange);
                    break;
                default:
                    playerNameText[i].SetParent(spectator);
                    break;
            }
            playerNameText[i].GetComponent<TextMeshProUGUI>().text = Room.RoomPlayers[i].DisplayName;
        }
        if(SceneManager.GetActiveScene().name == Room.lobbyScene){
            timeInput.text =(GameObject.FindObjectOfType<LobbyManager>().time/60).ToString();
            scoreInput.text =GameObject.FindObjectOfType<LobbyManager>().score.ToString();
        }   
        
        

    }
    public void HandleReadyToStart(bool readyToStart){
        if(!isLeader){return;}
        startGameButton.interactable = readyToStart;
    }
    [Command]
    private void CmdSetDisplayName(string displayName){
        DisplayName=displayName;
    }
    [Command]
    public void CmdStartGame(){
        if(!isLeader){return;}
        Room.StartGame();
    }
    [Command]
    public void CmdChangeTeam(int _team){
        team = _team;
        Room.NotifyPlayersOfReadyState();
    }
    public void Disconnect(){
        if(isServer){
            Room.StopHost();
        }
        Room.StopClient();
    }
    public void ChangeTime(){
        if(!isLeader){
            return;
        }
        if(timeInput.text == ""){
            timeInput.text= "0";
        }
        changeTimeOnServer(int.Parse(timeInput.text)*60);

    }
    public void ChangeScore(){
        if(!isLeader){
            return;
        }
        changeScoreOnServer(int.Parse(scoreInput.text));
    }
    [Command]
    void changeTimeOnServer(int time){
        GameObject.FindObjectOfType<LobbyManager>().time=time;
        
    }
    [Command]
    void changeScoreOnServer(int score){
        GameObject.FindObjectOfType<LobbyManager>().score=score;
    }
    
}
