
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool isServerBuild;
    private void Awake()
    {   
        if (!isServerBuild)
        {
            NetworkManager.Singleton.StartClient();
        }
    }
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            StartButtons();
        }

        GUILayout.EndArea();
    }

    static void StartButtons()
    { 
        if (GUILayout.Button("Reconnect")) NetworkManager.Singleton.StartClient();
    }




}
