using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using PlayFab;
using PlayFab.ClientModels;

public class PlayerManager : NetworkBehaviour
{
    public GameObject char1Prefab;
    public GameObject char2Prefab;
    public GameObject char3Prefab;
    public string selectedChar;

    private void Start()
    {
        if (IsOwner)
        {
            GetSelectedCharacter();
        }
    }

    public void GetSelectedCharacter()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest()
        {

        },
        result => {
            if (result.Data == null || !result.Data.ContainsKey("SelectedCharacter")) Debug.Log("No" + "SelectedCharacter");
            else
            {
                selectedChar = result.Data["SelectedCharacter"].Value;
                if (selectedChar == "1")
                {
                    PlayerSpawn1ServerRpc();
                }
                else if (selectedChar == "2")
                {
                    PlayerSpawn2ServerRpc();
                }
                else if (selectedChar == "3")
                {
                    PlayerSpawn3ServerRpc();
                }
            }

        },
        error => {
            Debug.Log("Got error getting read-only user data:");
            Debug.Log(error.GenerateErrorReport());
        });
    }

    [ServerRpc]
    void PlayerSpawn1ServerRpc()
    {
        NetworkObject playerNetworkObject = Instantiate(char1Prefab, Vector3.zero, Quaternion.Euler(0,0,0)).GetComponent<NetworkObject>();
        playerNetworkObject.SpawnWithOwnership(OwnerClientId);
    }
    [ServerRpc]
    void PlayerSpawn2ServerRpc()
    {
        NetworkObject playerNetworkObject = Instantiate(char2Prefab, Vector3.zero, Quaternion.Euler(0, 0, 0)).GetComponent<NetworkObject>();
        playerNetworkObject.SpawnWithOwnership(OwnerClientId);
    }
    [ServerRpc]
    void PlayerSpawn3ServerRpc()
    {
        NetworkObject playerNetworkObject = Instantiate(char3Prefab, Vector3.zero, Quaternion.Euler(0, 0, 0)).GetComponent<NetworkObject>();
        playerNetworkObject.SpawnWithOwnership(OwnerClientId);
    }
}
