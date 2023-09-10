using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class RoomPlayerManager : NetworkRoomPlayer
{
    [SyncVar(hook = nameof(OnNameChanged))]
    public string playerName;

    public Text nameText;

    public GameObject playerList;

    NetworkRoomManager room;

    public override void OnStartClient()
    {
        base.OnStartClient();
        room = NetworkManager.singleton as NetworkRoomManager;
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
    }

    // Each player inside the same will call it whenever a new client player enters a room.
    public override void OnClientEnterRoom()
    {
        base.OnClientEnterRoom();
        transform.SetParent(playerList.transform, false);
    }

    public void OnNameChanged(string oldName, string newName) {
        Debug.Log("Name display changed for " + playerName);
        nameText.text = playerName;
    }

}
