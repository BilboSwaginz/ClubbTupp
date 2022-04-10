using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using PlayFab.MultiplayerModels;
using PlayFab.MultiplayerAgent.Helpers;

public class Login : MonoBehaviour
{
    [SerializeField] private TMP_InputField usernameInputField = default;
    [SerializeField] private TMP_InputField emailInputField = default;
    [SerializeField] private TMP_InputField passwordInputField = default;
    [SerializeField] private TMP_Text accountCreateText = default;
    [SerializeField] private TMP_Text accountCreateTextError = default;
    [SerializeField] private GameObject loginDisplay = default;
    [SerializeField] private GameObject characterCreatorDisplay = default;
    [SerializeField] private GameObject characterCheck = default;
    public static string SessionTicket;
    public static string EntityId;
    public static string playFabIdString;
    public static string username;


    public void CreateAccount()
    {   

        PlayFabClientAPI.RegisterPlayFabUser(new RegisterPlayFabUserRequest
        {
            Username = usernameInputField.text,
            Email = emailInputField.text,
            Password = passwordInputField.text
        }, results => 
        {
            SessionTicket = results.SessionTicket;
            EntityId = results.EntityToken.Entity.Id; 
            accountCreateTextError.gameObject.SetActive(false);
            accountCreateText.gameObject.SetActive(true);
            playFabIdString = results.PlayFabId;

        }, error =>
        {
            Debug.LogError(error.GenerateErrorReport());
            accountCreateTextError.gameObject.SetActive(true);
        });

    }

    public void SignIn()
    {
        username = usernameInputField.text;
        PlayFabClientAPI.LoginWithPlayFab(new LoginWithPlayFabRequest
        {
            Username = usernameInputField.text,
            Password = passwordInputField.text
        }, results =>
        {
            SessionTicket = results.SessionTicket;
            EntityId = results.EntityToken.Entity.Id;
            playFabIdString = results.PlayFabId;
            loginDisplay.SetActive(false);
            characterCheck.SetActive(true);
            
        }, error =>
        {
            Debug.LogError(error.GenerateErrorReport());
        });
    }




    public static void UpdateUserData(string variable, string value)
    {
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>() {
                {variable, value},
            },
            Permission = UserDataPermission.Public

        },
        result => { },
        error => {
            
        });
    }
    public static void GetUserData(string variable)
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest()
        {
            
        },
        result => {
            if (result.Data == null || !result.Data.ContainsKey(variable)) Debug.Log("No" + variable);
            else Debug.Log(variable + ":" + result.Data[variable].Value);
            
        },
        error => {
            Debug.Log("Got error getting read-only user data:");
            Debug.Log(error.GenerateErrorReport());
        });
        
    }

    public static void StartGame()
    {
        SceneManager.LoadScene("Main");
        
    }


}
