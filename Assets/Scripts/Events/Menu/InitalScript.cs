using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using UnityEngine.SceneManagement;

public class InitalScript : MonoBehaviour
{
    public Dropdown numSelection;
    public InputField nameInput;
    NetworkRoomManager room = NetworkManager.singleton as NetworkRoomManager;

    public void OnNumChange()
    {
        room.maxConnections = numSelection.value + 2;
    }

    public void OnNameChange()
    {

    }

    public void JumpToRoom()
    {
        room.StartHost();
    }

    public void JumpToMenu()
    {
        room.StopHost();
        SceneManager.LoadScene("1");
    }
}
