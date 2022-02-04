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

    private void Start()
    {
        
    }

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
}
