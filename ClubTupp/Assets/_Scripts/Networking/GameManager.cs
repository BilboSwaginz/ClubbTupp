
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using PlayFab.MultiplayerModels;

public class GameManager : MonoBehaviour
{
    public bool ServerBuild;

    private void Start()
    {
        
        if (ServerBuild)
        {
            NetworkManager.Singleton.StartServer();
        }
        else
        {
            NetworkManager.Singleton.StartClient();
        }
    }

}
