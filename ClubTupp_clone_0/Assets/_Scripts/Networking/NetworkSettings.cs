using System;
using UnityEngine;
using UnityEngine.Networking;
using Unity.Netcode.Transports.UNET;
using PlayFab;
using PlayFab.MultiplayerAgent;

public class NetworkSettings : UNetTransport
{
    
    private void Start()
    {
        NetworkTransport.Init();
        InitializeNetwork();
        
    }


    void InitializeNetwork()
    {
        var myConfig = new ConnectionConfig();
        myConfig.AddChannel(QosType.Reliable);
        myConfig.AddChannel(QosType.Unreliable);
        myConfig.AddChannel(QosType.StateUpdate);
        myConfig.AddChannel(QosType.AllCostDelivery);

        myConfig.NetworkDropThreshold = 95; //95% packets that need to be dropped before connection is dropped 
        myConfig.OverflowDropThreshold = 30; //30% packets that need to be dropped before sendupdate timeout is increased
        myConfig.InitialBandwidth = 0;
        myConfig.MinUpdateTimeout = 10;
        myConfig.ConnectTimeout = 2000; // timeout before re-connect attempt will be made 
        myConfig.PingTimeout = 1500; // should have more than 3 pings per disconnect timeout, and more than 5 messages per ping 
        myConfig.DisconnectTimeout = 6000; // with a ping of 500 a disconnectimeout of 2000 is supposed to work well 
        myConfig.PacketSize = 1470; myConfig.SendDelay = 2;
        myConfig.FragmentSize = 1300;
        myConfig.AcksType = ConnectionAcksType.Acks128;
        myConfig.MaxSentMessageQueueSize = 256; 
        myConfig.AckDelay = 1;

        HostTopology myTopology = new HostTopology(myConfig,100);
        NetworkTransport.AddHost(myTopology);
        
    }













}
