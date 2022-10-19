using UnityEngine;

public class SwitchboardPlug : MonoBehaviour
{
    public bool IsConnected { get; set; }

    public void Disconnect(bool shouldDisconnect)
    {
        IsConnected = shouldDisconnect;
    }
}