using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Mirror;
public class MainMenu : MonoBehaviour
{
    private NetworkManagerLobby room;
    private NetworkManagerLobby Room
    {
        get{
            if(room != null){return room;}
            return room = NetworkManager.singleton as NetworkManagerLobby;
        }
    }
    //[Header("UI")]
    //[SerializeField]private GameObject landingPagePanel = null;
    
    

    
    public void HostLobby(){
        Room.StartHost();
        
    }
    void Start()
    {
        Cursor.lockState= CursorLockMode.None;
        Cursor.visible= true;
    }
    
}
