using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
public class PlayerData : MonoBehaviour
{   
    public int coins = 0;
    public TMP_Text coinsText;

    private void Awake()
    {
        CoinsDataFromPlayFab();
        coinsText.text = coins.ToString();
    }
    private void Update()
    {
        coinsText.text = coins.ToString();
    }

    public void UpdateCoins(int newCoins)
    {
        coins += newCoins;
        coinsText.text = coins.ToString();
        Login.UpdateUserData("coins", coins.ToString());
    }


    public void CoinsDataFromPlayFab()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest()
        {
           
        },
        result => 
        {
            if (result.Data.ContainsKey("coins"))
            {
                coins = System.Convert.ToInt32(result.Data["coins"].Value);

            }
        },
        error => 
        {

        });
    }
}
