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
        Debug.Log("1");
        if (collision.gameObject.GetComponentInParent<NetworkObject>().IsLocalPlayer)
        {
            NetworkManager.Singleton.LocalClient.PlayerObject.transform.position = playerTarget;
            mainCamera.transform.position = cameraTarget;
        }
        
    }

    


}
