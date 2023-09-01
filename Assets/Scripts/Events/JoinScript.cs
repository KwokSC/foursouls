using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class JoinScript : MonoBehaviour
{
    public InputField codeInput;
    public InputField nameInput;
    NetworkRoomManager room = NetworkManager.singleton as NetworkRoomManager;

    public void OnCodeChanged()
    {
        room.networkAddress = codeInput.text;
    }

    public void OnNameChanged()
    {
        Player.playerName = nameInput.text;
    }

    public void JumpToRoom()
    {
        room.StartClient();
    }


    public void JumpToMenu()
    {
        room.StopHost();
    }
}
