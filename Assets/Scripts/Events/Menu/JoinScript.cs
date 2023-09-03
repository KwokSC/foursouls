using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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
    }

    public void JumpToRoom()
    {
        room.StartClient();
    }


    public void JumpToMenu()
    {
        room.StopHost();
        SceneManager.LoadScene("1");
    }
}
