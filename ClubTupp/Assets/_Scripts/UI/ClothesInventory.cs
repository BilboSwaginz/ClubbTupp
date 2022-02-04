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

public class ClothesInventory : NetworkBehaviour
{
    public Clothes[] clothesListShirt;
    public Clothes[] ownedClothesShirt;
    public GameObject[] clothesDisplayShirt;

    public Clothes[] clothesListPants;
    public Clothes[] ownedClothesPants;
    public GameObject[] clothesDisplayPants;

    public Clothes[] clothesListHat;
    public Clothes[] ownedClothesHat;
    public GameObject[] clothesDisplayHat;


    private Clothes clickedClothes;

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

            UpdateClothesDisplayShirt();
            UpdateClothesDisplayPants();
            UpdateClothesDisplayHat();


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
        if (clothesDisplayShirt.Contains(EventSystem.current.currentSelectedGameObject) && Input.GetMouseButtonDown(0))
        {
            clickedClothes = ownedClothesShirt[ArrayUtility.IndexOf<GameObject>(clothesDisplayShirt, EventSystem.current.currentSelectedGameObject)];
            UpdateShirtClothesIndexServerRpc(ArrayUtility.IndexOf(clothesListShirt, clickedClothes));

        }
        else if (clothesDisplayPants.Contains(EventSystem.current.currentSelectedGameObject) && Input.GetMouseButtonDown(0))
        {
            clickedClothes = ownedClothesPants[ArrayUtility.IndexOf<GameObject>(clothesDisplayPants, EventSystem.current.currentSelectedGameObject)];
            UpdatePantsClothesIndexServerRpc(ArrayUtility.IndexOf(clothesListPants, clickedClothes));
        }
        else if (clothesDisplayHat.Contains(EventSystem.current.currentSelectedGameObject) && Input.GetMouseButtonDown(0))
        {
            clickedClothes = ownedClothesHat[ArrayUtility.IndexOf<GameObject>(clothesDisplayHat, EventSystem.current.currentSelectedGameObject)];
            UpdateHatClothesIndexServerRpc(ArrayUtility.IndexOf(clothesListHat, clickedClothes));
        }

    }

    public void UpdateClothesDisplayShirt()
    {
        for (int i = 0; i < ownedClothesShirt.Length; i++)
        {
            if (ownedClothesShirt[i] != null)
            {
                clothesDisplayShirt[i].GetComponent<Image>().sprite = ownedClothesShirt[i].displaySprite;
            }

        }
    }

    public void UpdateClothesDisplayPants()
    {
        for (int i = 0; i < ownedClothesPants.Length; i++)
        {
            if (ownedClothesPants[i] != null)
            {
                clothesDisplayPants[i].GetComponent<Image>().sprite = ownedClothesPants[i].displaySprite;
            }

        }
    }


    public void UpdateClothesDisplayHat()
    {
        for (int i = 0; i < ownedClothesHat.Length; i++)
        {
            if (ownedClothesHat[i] != null)
            {
                clothesDisplayHat[i].GetComponent<Image>().sprite = ownedClothesHat[i].displaySprite;
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
                            ownedClothesShirt[ArrayUtility.IndexOf(clothesListShirt, clothes)] = clothes;
                            return;

                        case Clothes.Type.Pants:
                            ownedClothesPants[ArrayUtility.IndexOf(clothesListPants, clothes)] = clothes;
                            return;

                        case Clothes.Type.Hat:
                            ownedClothesHat[ArrayUtility.IndexOf(clothesListHat, clothes)] = clothes;
                            return;
                    }
                }
            }

        },
        error => {

        });
        UpdateClothesDisplayShirt();
        UpdateClothesDisplayHat();
        UpdateClothesDisplayPants();
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
        if (!IsClient)
        {
            return;
        }
        shirt.GetComponent<Renderer>().material = clothesListShirt[newIndex].GetComponent<Renderer>().sharedMaterial;
        shirt.GetComponent<MeshFilter>().mesh = clothesListShirt[newIndex].GetComponent<MeshFilter>().sharedMesh;
        shirt.localPosition = clothesListShirt[newIndex].transform.localPosition;
        shirt.localRotation = clothesListShirt[newIndex].transform.localRotation;
        shirt.localScale = clothesListShirt[newIndex].transform.localScale;
    }


    private void PantsChange(int oldIndex, int newIndex)
    {
        if (!IsClient)
        {
            return;
        }
        pants.GetComponent<Renderer>().material = clothesListPants[newIndex].GetComponent<Renderer>().sharedMaterial;
        pants.GetComponent<MeshFilter>().mesh = clothesListPants[newIndex].GetComponent<MeshFilter>().sharedMesh;
        pants.localPosition = clothesListPants[newIndex].transform.localPosition;
        pants.localRotation = clothesListPants[newIndex].transform.localRotation;
        pants.localScale = clothesListPants[newIndex].transform.localScale;
    }

    private void HatChange(int oldIndex, int newIndex)
    {
        if (!IsClient)
        {
            return;
        }
        hat.GetComponent<Renderer>().material = clothesListHat[newIndex].GetComponent<Renderer>().sharedMaterial;
        hat.GetComponent<MeshFilter>().mesh = clothesListHat[newIndex].GetComponent<MeshFilter>().sharedMesh;
        hat.localPosition = clothesListHat[newIndex].transform.localPosition;
        hat.localRotation = clothesListHat[newIndex].transform.localRotation;
        hat.localScale = clothesListHat[newIndex].transform.localScale;
    }

    private void UpdateClothesStart()
    {
        if (shirtIndex.Value >= 0)
        {
            shirt.GetComponent<Renderer>().material = clothesListShirt[shirtIndex.Value].GetComponent<Renderer>().sharedMaterial;
            shirt.GetComponent<MeshFilter>().mesh = clothesListShirt[shirtIndex.Value].GetComponent<MeshFilter>().sharedMesh;
            shirt.localPosition = clothesListShirt[shirtIndex.Value].transform.localPosition;
            shirt.localRotation = clothesListShirt[shirtIndex.Value].transform.localRotation;
            shirt.localScale = clothesListShirt[shirtIndex.Value].transform.localScale;
        }
        if (pantsIndex.Value >= 0)
        {
            pants.GetComponent<Renderer>().material = clothesListPants[pantsIndex.Value].GetComponent<Renderer>().sharedMaterial;
            pants.GetComponent<MeshFilter>().mesh = clothesListPants[pantsIndex.Value].GetComponent<MeshFilter>().sharedMesh;
            pants.localPosition = clothesListPants[pantsIndex.Value].transform.localPosition;
            pants.localRotation = clothesListPants[pantsIndex.Value].transform.localRotation;
            pants.localScale = clothesListPants[pantsIndex.Value].transform.localScale;
        }
        if (hatIndex.Value >= 0)
        {
            hat.GetComponent<Renderer>().material = clothesListHat[hatIndex.Value].GetComponent<Renderer>().sharedMaterial;
            hat.GetComponent<MeshFilter>().mesh = clothesListHat[hatIndex.Value].GetComponent<MeshFilter>().sharedMesh;
            hat.localPosition = clothesListHat[hatIndex.Value].transform.localPosition;
            hat.localRotation = clothesListHat[hatIndex.Value].transform.localRotation;
            hat.localScale = clothesListHat[hatIndex.Value].transform.localScale;
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

