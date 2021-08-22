using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Mirror;

public class JoinLobbyMenu : MonoBehaviour
{
    private NetworkManagerLobby room;
    private NetworkManagerLobby Room
    {
        get{
            if(room != null){return room;}
            return room = NetworkManager.singleton as NetworkManagerLobby;
        }
    }
    [Header("UI")]
    [SerializeField]private GameObject landingPagePanel = null;
    [SerializeField]private TMP_InputField ipAddressInputField = null;
    [SerializeField]private Button joinButton = null;
    void OnEnable()
    {
        //NetworkManagerLobby.OnClientConnected += HandleClientConnected;
        NetworkManagerLobby.OnClientDisconnected += HandleClientDisconnected;
    }
    void OnDestroy()
    {
        NetworkManagerLobby.OnClientDisconnected -= HandleClientDisconnected;
    }
    public void JoinLobby(){
        string ipAddress= ipAddressInputField.text;

        Room.networkAddress=ipAddress;
        Room.StartClient();

        joinButton.interactable = false;
    }
    // private void HandleClientConnected(){
    //     joinButton.interactable=true;
    //     landingPagePanel.SetActive(false);
    // }
    private void HandleClientDisconnected(){
        joinButton.interactable = true;
    }
}
