using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using Unity.Netcode;
public class ClothesStoreButton : MonoBehaviour
{
    public TMP_Text priceText;
    public Button purchaseButton;
    public Clothes clothes;
    public PlayerData playerData;

    private void Awake()
    {
        priceText.text = clothes.price.ToString();
        ClothesOwnershipCheck();
        playerData = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerData>();
    }

    private void Start()
    {   
        purchaseButton.onClick.AddListener(ClothesOwnershipCheck);
        purchaseButton.onClick.AddListener(ButtonOff);
        ClothesOwnershipCheck();
    }

    private void OnEnable()
    {
        ClothesOwnershipCheck();
    }

    public void ClothesOwnershipCheck()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest()
        {

        },
        result =>
        {
           
            if (result.Data.ContainsKey(clothes.name))
            {
                if (result.Data[clothes.name].Value.Equals("owned"))
                {
                    purchaseButton.enabled = false;
                    priceText.fontSize = 18;
                    priceText.text = "Purchased";
                }

            }
        },
        error => {

        });
    }
    
    public void ButtonOff()
    {
        if (playerData.coins > clothes.price)
        {
            purchaseButton.enabled = false;
            priceText.fontSize = 18;
            priceText.text = "Purchased";
        }
    }
}
