using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using UnityEngine.EventSystems;
using Unity.Netcode;
using System.Linq;

public class ClothesShop : MonoBehaviour
{
    public PlayerData playerData;
    [SerializeField] private GameObject clothesShopUI;
    [SerializeField] private GameObject clothesShopButton;


    public void BuyClothes(Clothes clothes)
    {
        if (playerData.coins > clothes.price)
        {
            playerData.UpdateCoins(-clothes.price);
            Login.UpdateUserData(clothes.name, "owned");
        }
    }

    public void OpenClothesShop()
    {
        clothesShopUI.SetActive(true);
    }

    public void ExitClothesShop()
    {
        clothesShopUI.SetActive(false);
    }
    
    public void ActiveClothesShopButton()
    {
        clothesShopButton.SetActive(true);
    }

    public void DeActiveClothesShopButton()
    {
        clothesShopButton.SetActive(false);
    }

}
