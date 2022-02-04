using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using PlayFab;
using PlayFab.ClientModels;

public class PlayerManager : NetworkBehaviour
{
    public GameObject char1Prefab;
  
    public string selectedChar;

    public Material[] materialArray;

    public NetworkObject playerNetworkObject;

    public NetworkVariable<byte> materialIndex = new NetworkVariable<byte>();
    [SerializeField] private GameObject ClothesUI;

    private void Start()
    {
        if (IsOwner)
        {
            GetSelectedCharacter();
            ClothesUI.SetActive(true);
        }
        transform.GetChild(0).GetChild(0).GetChild(2).GetComponent<Renderer>().material = materialArray[materialIndex.Value];
    }

    private void Update()
    {

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
                   
                    //PlayerSpawnServerRpc();
                    MaterialIndexServerRpc(1);
                }
                else if (selectedChar == "2")
                {
                    
                    //PlayerSpawnServerRpc();
                    MaterialIndexServerRpc(2);
                }
                else if (selectedChar == "3")
                {                   
                    //PlayerSpawnServerRpc();
                    MaterialIndexServerRpc(3);
                }
            }

        },
        error => {
            Debug.Log("Got error getting read-only user data:");
            Debug.Log(error.GenerateErrorReport());
        });
    }

    [ServerRpc]
    private void PlayerSpawnServerRpc()
    {
        
        playerNetworkObject = Instantiate(char1Prefab, Vector3.zero, Quaternion.Euler(0,0,0),gameObject.transform).GetComponent<NetworkObject>();
        playerNetworkObject.SpawnWithOwnership(OwnerClientId);
    }

    [ServerRpc]
    public void MaterialIndexServerRpc(byte newMaterialIndex)
    {
        materialIndex.Value = newMaterialIndex;
    }

    private void OnEnable()
    {
        materialIndex.OnValueChanged += MaterialChange;
    }
    private void OnDisable()
    {
        materialIndex.OnValueChanged -= MaterialChange;
    }

    private void MaterialChange(byte oldMaterialIndex,byte newMaterialIndex)
    {
        if (!IsClient)
        {
            return;
        }
        transform.GetChild(0).GetChild(0).GetChild(2).GetComponent<Renderer>().material = materialArray[newMaterialIndex];
    }

    
}
