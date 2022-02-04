using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class StartServer : MonoBehaviour
{
    private void Awake()
    {
        NetworkManager.Singleton.StartServer();
    }
}
