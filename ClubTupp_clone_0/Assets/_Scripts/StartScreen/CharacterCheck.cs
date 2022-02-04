using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class CharacterCheck : NetworkBehaviour
{
    [SerializeField] private GameObject characterCreatorDisplay = default;


    void Start()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest()
        {

        },
        result => {
            if (result.Data == null || !result.Data.ContainsKey("SelectedCharacter"))
            {
                characterCreatorDisplay.SetActive(true);
            }
            else
            {
                SceneManager.LoadScene("Main");
            }

        },
        error => {
            Debug.Log("Got error getting read-only user data:");
            Debug.Log(error.GenerateErrorReport());
        });
    }

   
}
