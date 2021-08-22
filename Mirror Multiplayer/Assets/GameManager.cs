using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using System;

public class GameManager : NetworkBehaviour
{
    [SyncVar(hook = nameof(UpdateScore))]
    public int scoreA;
    [SyncVar(hook = nameof(UpdateScore))]
    public int scoreB;
    public TextMeshProUGUI textA;
    public TextMeshProUGUI textB;
    public TextMeshProUGUI timer;
    [SyncVar(hook = nameof(UpdateTime))]
    float time;
    float t;
    bool useTimer = false;
    [SyncVar]
    bool stopTimer;
    [SyncVar]
    public int winScore;
    bool scorable = true;
    [SerializeField] TextMeshProUGUI winText;
    [SerializeField] string blueWon = "blue won";
    [SerializeField] string orangeWon = "orange won";
    [SerializeField] string blueScored = "blue scored";
    [SerializeField] string orangeScored = "orange scored";
    [SerializeField] string draw = "draw";

    private NetworkManagerLobby room;
    private NetworkManagerLobby Room
    {
        get{
            if(room != null){return room;}
            return room = NetworkManager.singleton as NetworkManagerLobby;
        }
    }
    
    public void Score(int team){
        if(!isServer || !scorable)
            return;
        if(team==0){
            scoreA++;
            RpcShowText(blueScored);
        }
        else{
            scoreB++;
            RpcShowText(orangeScored);
        }
        
        scorable = false;
        stopTimer = true;
        if(scoreA>=winScore || scoreB >= winScore){
            EndMatch();
            return;
        }
        StartCoroutine(Respawn(2));
    }
    IEnumerator Respawn(float waitTime){
        yield return new WaitForSeconds(waitTime);
        RpcRespawn();
        stopTimer = false;
        scorable = true;
    }
   
    [ClientRpc]
    void RpcRespawn(){
        GameObject.FindGameObjectWithTag("Ball").transform.position= Vector3.zero;
        GameObject.FindGameObjectWithTag("Ball").GetComponent<Rigidbody>().velocity= Vector3.zero;
        player[] players = GameObject.FindObjectsOfType<player>();
        foreach (player p in players)
        {
            p.transform.position=p.spawn;
            p.GetComponent<Rigidbody>().velocity=Vector3.zero;
        }
        winText.text = "";
    }
    [ClientRpc]
    void RpcSyncTime(float _t){
        t = _t;
    }
    [ClientRpc]
    void RpcShowText(string textToShow){
        winText.text = textToShow;
    }
    [Server]
    public void Initialize(int winScore, float time){
        this.time = time;
        this.winScore = winScore;
        useTimer = time != 0;
        t= time;
        scorable=true;
    }

    void Update()
    {
        
        if(useTimer && !stopTimer){
            t -= Time.deltaTime;
            TimeSpan timeSpan = TimeSpan.FromSeconds(t);
            timer.text = string.Format("{0:D2}:{1:D2}",timeSpan.Minutes,timeSpan.Seconds);
            
            if(t <= 0){
                if(isServer){
                    EndMatch();
                }
            }
        }
        

    }
    void UpdateTime(float oldValue, float newValue){
        useTimer = time != 0;
        t= time;
    }
    void UpdateScore(int oldValue, int newValue){
        textA.text=scoreA.ToString();
        textB.text=scoreB.ToString();
    }
    void Start()
    {
        if(isServer){
            Initialize(Room.score,Room.time);
        }
        
    }
    void EndMatch(){
        stopTimer = true;
        scorable = false;
        if(scoreA == scoreB){
            RpcShowText(draw);
        }
        else if(scoreA > scoreB){
            RpcShowText(blueWon);
        }
        else{
            RpcShowText(orangeWon);
        }
        StartCoroutine(End(3));
    }
    IEnumerator End(float waitTime){
        yield return new WaitForSeconds(waitTime);
        Room.EndMatch();
    }
}
