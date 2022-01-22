using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using PlayFab;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using PlayFab.ClientModels;
public class CharacterCreation : NetworkBehaviour
{
    [SerializeField] private GameObject char1Button;
    [SerializeField] private GameObject char2Button;
    [SerializeField] private GameObject char3Button;
    [SerializeField] private GameObject startButton;

    private int selectedCharacter;


    private void Start()
    {
        GetCharacterData();
    }

    public void DeleteUserCharacterData()
    {
        Login.UpdateUserData("CreatedCharacter", "false");
    }
    
    public void UpdateUserCharacterData()
    {
        Login.UpdateUserData("CreatedCharacter", "true");
        Login.UpdateUserData("SelectedCharacter", selectedCharacter.ToString());
    }
    public void GetUserCharacterData()
    {
        Login.GetUserData("CreatedCharacter");
        Login.GetUserData("SelectedCharacter");
    }
    
    public void CharacterSelected1()
    {
        selectedCharacter = 1;
        char1Button.GetComponent<Image>().color = new Color(0, 1, 0);
        char2Button.GetComponent<Image>().color = new Color(1, 1, 1);
        char3Button.GetComponent<Image>().color = new Color(1, 1, 1);
        GetCharacterData();
    }

    public void CharacterSelected2()
    {
        selectedCharacter = 2;
        char1Button.GetComponent<Image>().color = new Color(1, 1, 1);
        char2Button.GetComponent<Image>().color = new Color(0, 1, 0);
        char3Button.GetComponent<Image>().color = new Color(1, 1, 1);
        GetCharacterData();
    }
    public void CharacterSelected3()
    {
        selectedCharacter = 3;
        char1Button.GetComponent<Image>().color = new Color(1, 1, 1);
        char2Button.GetComponent<Image>().color = new Color(1, 1, 1);
        char3Button.GetComponent<Image>().color = new Color(0, 1, 0);
        GetCharacterData();
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Main");
    }

    public void GetCharacterData()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest()
        {

        },
        result => {
            if (result.Data == null || !result.Data.ContainsKey("SelectedCharacter")) Debug.Log("No" + "SelectedCharacter");
            else
            {
                startButton.SetActive(true);
            }

        },
        error => {
            Debug.Log("Got error getting read-only user data:");
            Debug.Log(error.GenerateErrorReport());
        });
    }
}
