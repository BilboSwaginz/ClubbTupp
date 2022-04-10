using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Door : NetworkBehaviour
{
    public GameObject mainCamera;
    public Vector3 playerTarget;
    public Vector3 cameraTarget;
    public Collider2D doorCollider;


  

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponentInParent<NetworkObject>().IsLocalPlayer)
        {
            NetworkManager.Singleton.LocalClient.PlayerObject.transform.GetChild(0).transform.position = playerTarget;
            mainCamera.transform.position = cameraTarget;
            NetworkManager.Singleton.LocalClient.PlayerObject.transform.GetChild(0).GetComponent<PlayerMovement>().MouseMoveRequestServerRpc(playerTarget - new Vector3(0,0,10));

            if (gameObject.name == "ClothesStoreDoor")
            {
                NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<ClothesShop>().ActiveClothesShopButton();
            }
            if (gameObject.name == "MainStreetDoor")
            {
                NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<ClothesShop>().DeActiveClothesShopButton();
            }
        }   
    }


    

}
