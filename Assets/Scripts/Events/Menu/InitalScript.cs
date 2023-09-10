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
    RoomManager room = NetworkManager.singleton as RoomManager;

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
        SceneManager.LoadScene(1);
    }
}
