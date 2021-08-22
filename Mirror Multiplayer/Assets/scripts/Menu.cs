using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    bool inMenu = false;
    bool leader = false;
    [SerializeField] GameObject pauseMenu;
    NetworkGamePlayer gamePlayer;
    [SerializeField] GameObject endMatchButton;

    void Start()
    {
        inMenu = false;
        Cursor.lockState= CursorLockMode.Locked;
        Cursor.visible= inMenu;
        
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)){
            inMenu = !inMenu;
            Cursor.lockState= inMenu?CursorLockMode.None:CursorLockMode.Locked;
            Cursor.visible= inMenu;
            pauseMenu.SetActive(inMenu);
        }
    }
    public void Setup(NetworkGamePlayer gamePlayer, bool leader){
        this.gamePlayer = gamePlayer;
        this.leader=leader;
        if(leader){
            endMatchButton.SetActive(true);
        }
    }
    public void Disconnect(){
        gamePlayer.Disconnect();
    }
    public void EndMatch(){
        gamePlayer.EndMatch();
    }
    
}
