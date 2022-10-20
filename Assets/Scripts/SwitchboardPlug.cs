using System;
using UnityEngine;

public class SwitchboardPlug : MonoBehaviour
{
    public LED Led;
    public bool IsConnected { get; set; }

    public void Disconnect(bool shouldDisconnect)
    {
        IsConnected = shouldDisconnect;
    }
    
    
}