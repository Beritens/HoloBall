using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class PlayerNameInput : MonoBehaviour
{
    [Header("UI")]
    [SerializeField]private TMP_InputField nameInputField = null;
    public static string DisplayName{get;private set;}
    private const string PlayerPrefsNameKey = "PlayerName";
    // Start is called before the first frame update
    void Start()
    {
        SetUpInputField();
    }
    private void SetUpInputField(){
        if(!PlayerPrefs.HasKey(PlayerPrefsNameKey)){
            return;
        }
        string defaultName = PlayerPrefs.GetString(PlayerPrefsNameKey);
        nameInputField.text=defaultName;
        DisplayName=defaultName;
        SetPlayerName(defaultName);
    }
    public void SetPlayerName(string name){
        //activate buttons if name is valid
    }

    public void SavePlayerName(){
        DisplayName=nameInputField.text;
        PlayerPrefs.SetString(PlayerPrefsNameKey,DisplayName);
        Debug.Log(DisplayName);
    }
}
