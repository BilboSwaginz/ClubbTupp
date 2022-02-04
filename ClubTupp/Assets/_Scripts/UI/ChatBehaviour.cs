using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using PlayFab;
using PlayFab.ClientModels;

public class ChatBehaviour : NetworkBehaviour
{
    [SerializeField] private GameObject chatUI;
    [SerializeField] private TMP_Text chatText;
    [SerializeField] private TMP_InputField chatInput;
    
    [SerializeField] private GameObject chatBubble;
    [SerializeField] private GameObject chatBubbleBubble;
    [SerializeField] private TMP_Text chatBubbleText;
    [SerializeField] private TMP_Text nameDisplay;

    
    private float chatBubbleTimer;

    public Transform playerCharacter;

    private void Start()
    {   
        if (IsOwner)
        {
            chatUI.SetActive(true);
            SendUsernameServerRpc(Login.username);
        }
        
    }
    

    private void FixedUpdate()
    {   
        if (IsOwner)
        {
            SendUsernameServerRpc(Login.username);
        }
        
    }


    private void Update()
    {
        chatBubble.transform.position = playerCharacter.position;
        nameDisplay.gameObject.transform.position = playerCharacter.position + new Vector3(0,-1,0);
        chatBubbleTimer -= Time.deltaTime;
        if (chatBubbleTimer < 0)
        {
            chatBubble.SetActive(false);
            chatBubbleBubble.transform.localScale = new Vector3(1,1,1);
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            chatInput.ActivateInputField();
        }
    }

    //Called from the InputField
    public void Send(string message)
    {   
        if (IsClient)
        {
            if (!Input.GetKeyDown(KeyCode.Return) || string.IsNullOrWhiteSpace(message))
            {
                return;
            }
            
            SendMessageServerRpc(message, Login.username);
            chatInput.text = string.Empty;
        }
    }

    [ServerRpc]
    private void SendMessageServerRpc(string message, string username)
    {
        HandleMessageClientRpc(message,username);
    }

    [ClientRpc]
    private void HandleMessageClientRpc(string message, string username)
    {
        UpdateChat(message,username); 
    }
    

    public void UpdateChat(string message, string username)
    {
        chatBubbleText.text = message;


        chatBubbleBubble.transform.localScale = new Vector3(1, 1.55f, 1);
        chatBubbleBubble.transform.localPosition = new Vector3(0, 1.85f, 0);
        chatBubbleBubble.transform.localScale += new Vector3(message.Length / 20, 0, 0);
        chatBubbleBubble.transform.localPosition += new Vector3(0, message.Length / 40, 0);
        chatBubbleText.gameObject.transform.localPosition = new Vector3(0, 0, 0);
        chatBubbleText.gameObject.transform.localPosition += new Vector3(0, message.Length / 40, 0);


        chatBubble.SetActive(true);

        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<ChatBehaviour>().chatText.text += $"\n{username}:{message}";
        chatBubbleTimer = 4f;
    }

    [ServerRpc]
    private void SendUsernameServerRpc(string username)
    {
        UpdateUsernameClientRpc(username);
    }

    [ClientRpc]
    private void UpdateUsernameClientRpc(string username)
    {
        nameDisplay.text = username;
    }

}
