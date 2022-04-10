using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using Unity.Netcode;
using System.Linq;
using System;

public class ClothesInventory : NetworkBehaviour
{
    [Header("Shirts")]
    public Clothes[] clothesListShirt;
    public Clothes[] ownedClothesShirt;
    public GameObject[] clothesDisplayShirt;

    [Header("Pants")]
    public Clothes[] clothesListPants;
    public Clothes[] ownedClothesPants;
    public GameObject[] clothesDisplayPants;

    [Header("Hats")]
    public Clothes[] clothesListHat;
    public Clothes[] ownedClothesHat;
    public GameObject[] clothesDisplayHat;


    [Header("UI")]
    [SerializeField] private GameObject InventoryUI;
    [SerializeField] private GameObject ClothesUI;

    [SerializeField] private GameObject shirtPages;
    [SerializeField] private GameObject pantsPages;
    [SerializeField] private GameObject hatsPages;

    public NetworkVariable<int> shirtIndex = new NetworkVariable<int>(-1);
    public NetworkVariable<int> pantsIndex = new NetworkVariable<int>(-1);
    public NetworkVariable<int> hatIndex = new NetworkVariable<int>(-1);

    public Transform shirt;
    public Transform pants;
    public Transform hat;

    private void Start()
    {

        /*if (IsServer)
        {
            shirtIndex.Value = -1;
            pantsIndex.Value = -1;
            hatIndex.Value = -1;
        }*/
        if (IsOwner)
        {
            ClothesUI.SetActive(true);
            InventoryUI.SetActive(false);

            UpdateOwnedClothes();

            UpdateClothesDisplay(ownedClothesShirt, clothesDisplayShirt);
            UpdateClothesDisplay(ownedClothesPants, clothesDisplayPants);
            UpdateClothesDisplay(ownedClothesHat, clothesDisplayHat);


        }
        UpdateClothesStart();

    }



    private void Update()
    {
        OnClickEquipClothes();
    }

    public void OpenInventory()
    {
        if (InventoryUI.activeSelf)
        {
            InventoryUI.SetActive(false);
            return;
        }
        InventoryUI.SetActive(true);
    }

    public void CloseInventory()
    {
        InventoryUI.SetActive(false);
    }


    
    public void OnClickEquipClothes()
    {
        if (ClickOnClothesInInventory(clothesDisplayShirt)) //Check if the player is clickon on a Clothes in the clothes inventory
        {
            UpdateClickedClothes(ownedClothesShirt, clothesListShirt, clothesDisplayShirt, UpdateShirtClothesIndexServerRpc);
        }
        else if (ClickOnClothesInInventory(clothesDisplayPants))
        {
            UpdateClickedClothes(ownedClothesPants, clothesListPants, clothesDisplayPants, UpdatePantsClothesIndexServerRpc);
        }
        else if (ClickOnClothesInInventory(clothesDisplayHat))
        {
            UpdateClickedClothes(ownedClothesHat, clothesListHat, clothesDisplayHat, UpdateHatClothesIndexServerRpc);
        }

    }


    private void UpdateClickedClothes(Clothes[] ownedClothesList, Clothes[]clothesList, GameObject[] clothesDisplayList, Action<int> UpdateClothesIndexServerRpc)
    {   
        //Get the clothes that was clicked
        var clickedClothes = ownedClothesList[Array.IndexOf(clothesDisplayList, EventSystem.current.currentSelectedGameObject)];

        //Update the players equiped clothes index with the clicked clothes
        UpdateClothesIndexServerRpc(Array.IndexOf(clothesList, clickedClothes));
    }


    private bool ClickOnClothesInInventory(GameObject[] clothesDisplayList)
    {   
        //Is the mouse over an object in the clothesDisplayList (Not some random object) 
        return clothesDisplayList.Contains(EventSystem.current.currentSelectedGameObject) && Input.GetMouseButtonDown(0);
    }


    public void UpdateClothesDisplay(Clothes[] ownedClothes, GameObject[] clothesDisplay)
    {
        for (int i = 0; i < ownedClothes.Length; i++)
        {
            if (ownedClothes[i] != null)
            {
                clothesDisplay[i].GetComponent<Image>().sprite = ownedClothes[i].displaySprite;
            }
        }
    }
  


    public void UpdateOwnedClothes()
    {
        foreach (Clothes clothes in clothesListShirt)
        {
            ClothesOwnershipCheck(clothes);
        }

        foreach (Clothes clothes in clothesListPants)
        {
            ClothesOwnershipCheck(clothes);
        }

        foreach (Clothes clothes in clothesListHat)
        {
            ClothesOwnershipCheck(clothes);
        }

    }

    private void ClothesOwnershipCheck(Clothes clothes)
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest()
        {

        },
        result => {
            if (result.Data.ContainsKey(clothes.name))
            {
                if (result.Data[clothes.name].Value == "owned")
                {
                    switch (clothes.type)
                    {
                        case Clothes.Type.Shirt:
                            ownedClothesShirt[Array.IndexOf(clothesListShirt, clothes)] = clothes;
                            return;

                        case Clothes.Type.Pants:
                            ownedClothesPants[Array.IndexOf(clothesListPants, clothes)] = clothes;
                            return;

                        case Clothes.Type.Hat:
                            ownedClothesHat[Array.IndexOf(clothesListHat, clothes)] = clothes;
                            return;
                    }
                }
            }

        },
        error => {

        });
        UpdateClothesDisplay(ownedClothesShirt, clothesDisplayShirt);
        UpdateClothesDisplay(ownedClothesPants, clothesDisplayPants);
        UpdateClothesDisplay(ownedClothesHat, clothesDisplayHat);
    }


    [ServerRpc]
    public void UpdateShirtClothesIndexServerRpc(int index)
    {
        shirtIndex.Value = index;
    }


    [ServerRpc]
    public void UpdatePantsClothesIndexServerRpc(int index)
    {
        pantsIndex.Value = index;
    }


    [ServerRpc]
    public void UpdateHatClothesIndexServerRpc(int index)
    {
        hatIndex.Value = index;
    }



    private void OnEnable()
    {
        shirtIndex.OnValueChanged += ShirtChange;
        pantsIndex.OnValueChanged += PantsChange;
        hatIndex.OnValueChanged += HatChange;
    }

    private void OnDisable()
    {
        shirtIndex.OnValueChanged -= ShirtChange;
        pantsIndex.OnValueChanged -= PantsChange;
        hatIndex.OnValueChanged -= HatChange;
    }


    private void ShirtChange(int oldIndex, int newIndex)
    {
        ChangeClothes(shirt, clothesListShirt, newIndex);
    }


    private void PantsChange(int oldIndex, int newIndex)
    {
        ChangeClothes(pants, clothesListPants, newIndex);
    }

    
    private void HatChange(int oldIndex, int newIndex)
    {
        ChangeClothes(hat, clothesListHat, newIndex);
    }


    private void ChangeClothes(Transform clothes, Clothes[] clothesList, int newIndex)
    {
         if (!IsClient)
         {
             return;
         }
         clothes.GetComponent<Renderer>().material = clothesList[newIndex].GetComponent<Renderer>().sharedMaterial;
         clothes.GetComponent<MeshFilter>().mesh = clothesList[newIndex].GetComponent<MeshFilter>().sharedMesh;
         clothes.localPosition = clothesList[newIndex].transform.localPosition;
         clothes.localRotation = clothesList[newIndex].transform.localRotation;
         clothes.localScale = clothesList[newIndex].transform.localScale;
    }
    private void UpdateClothesStart()
    {
        if (shirtIndex.Value >= 0)
        {
            ChangeClothes(shirt, clothesListShirt, shirtIndex.Value);
        }
        if (pantsIndex.Value >= 0)
        {
            ChangeClothes(pants, clothesListPants, pantsIndex.Value);
        }
        if (hatIndex.Value >= 0)
        {
            ChangeClothes(hat, clothesListHat, hatIndex.Value);
        }
    }

    public void ShirtPages()
    {
        shirtPages.SetActive(true);
        pantsPages.SetActive(false);
        hatsPages.SetActive(false);
    }
    public void PantsPages()
    {
        shirtPages.SetActive(false);
        pantsPages.SetActive(true);
        hatsPages.SetActive(false);
    }
    public void HatsPages()
    {
        shirtPages.SetActive(false);
        pantsPages.SetActive(false);
        hatsPages.SetActive(true);
    }

    public void NextPage()
    {
        if (shirtPages.activeSelf)
        {
            for (int i=0; i < shirtPages.transform.childCount; i++)
            {
                if (shirtPages.transform.GetChild(i).gameObject.activeSelf)
                {
                    if (i == shirtPages.transform.childCount - 1)
                    {                       
                        shirtPages.transform.GetChild(i).gameObject.SetActive(false);
                        shirtPages.transform.GetChild(0).gameObject.SetActive(true);
                        break;
                    }
                    else
                    {                      
                        shirtPages.transform.GetChild(i).gameObject.SetActive(false);
                        shirtPages.transform.GetChild(i + 1).gameObject.SetActive(true);
                        break;
                    }
                }
            }
        }
        else if (pantsPages.activeSelf)
        {
            for (int i = 0; i < pantsPages.transform.childCount; i++)
            {
                if (pantsPages.transform.GetChild(i).gameObject.activeSelf)
                {
                    if (i == pantsPages.transform.childCount - 1)
                    {
                        pantsPages.transform.GetChild(i).gameObject.SetActive(false);
                        pantsPages.transform.GetChild(0).gameObject.SetActive(true);
                        break;
                    }
                    else
                    {
                        pantsPages.transform.GetChild(i).gameObject.SetActive(false);
                        pantsPages.transform.GetChild(i + 1).gameObject.SetActive(true);
                        break;
                    }
                }
            }
        }
        else if (hatsPages.activeSelf)
        {
            for (int i = 0; i < hatsPages.transform.childCount; i++)
            {
                if (hatsPages.transform.GetChild(i).gameObject.activeSelf)
                {
                    if (i == hatsPages.transform.childCount - 1)
                    {
                        hatsPages.transform.GetChild(i).gameObject.SetActive(false);
                        hatsPages.transform.GetChild(0).gameObject.SetActive(true);
                        break;
                    }
                    else
                    {
                        hatsPages.transform.GetChild(i).gameObject.SetActive(false);
                        hatsPages.transform.GetChild(i + 1).gameObject.SetActive(true);
                        break;
                    }
                }
            }
        }
    }

    public void PreviousPage()
    {
        if (shirtPages.activeSelf)
        {
            for (int i = 0; i < shirtPages.transform.childCount; i++)
            {
                if (shirtPages.transform.GetChild(i).gameObject.activeSelf)
                {
                    if (i == 0)
                    {
                        shirtPages.transform.GetChild(i).gameObject.SetActive(false);
                        shirtPages.transform.GetChild(shirtPages.transform.childCount - 1).gameObject.SetActive(true);
                        break;
                    }
                    else
                    {
                        shirtPages.transform.GetChild(i).gameObject.SetActive(false);
                        shirtPages.transform.GetChild(i - 1).gameObject.SetActive(true);
                        break;
                    }
                }
            }
        }
        else if (pantsPages.activeSelf)
        {
            for (int i = 0; i < pantsPages.transform.childCount; i++)
            {
                if (pantsPages.transform.GetChild(i).gameObject.activeSelf)
                {
                    if (i == 0)
                    {
                        pantsPages.transform.GetChild(i).gameObject.SetActive(false);
                        pantsPages.transform.GetChild(pantsPages.transform.childCount - 1).gameObject.SetActive(true);
                        break;
                    }
                    else
                    {
                        pantsPages.transform.GetChild(i).gameObject.SetActive(false);
                        pantsPages.transform.GetChild(i - 1).gameObject.SetActive(true);
                        break;
                    }
                }
            }
        }
        else if (hatsPages.activeSelf)
        {
            for (int i = 0; i < hatsPages.transform.childCount; i++)
            {
                if (hatsPages.transform.GetChild(i).gameObject.activeSelf)
                {
                    if (i == 0)
                    {
                        hatsPages.transform.GetChild(i).gameObject.SetActive(false);
                        hatsPages.transform.GetChild(hatsPages.transform.childCount - 1).gameObject.SetActive(true);
                        break;
                    }
                    else
                    {
                        hatsPages.transform.GetChild(i).gameObject.SetActive(false);
                        hatsPages.transform.GetChild(i - 1).gameObject.SetActive(true);
                        break;
                    }
                }
            }
        }
    }
}

