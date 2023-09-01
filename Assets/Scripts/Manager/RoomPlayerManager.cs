using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class RoomPlayerManager : NetworkRoomPlayer
{
    [SyncVar(hook = nameof(OnNameChanged))]
    public string playerName;

    public bool isSpawned = false;

    public GameObject roomPlayerDisplayPrefab;
    public GameObject playerList;
    GameObject roomPlayerDisplay;
    NetworkRoomManager room;

    public override void OnStartClient()
    {
        base.OnStartClient();
        room = NetworkManager.singleton as NetworkRoomManager;
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        CmdSetupPlayer("Player" + Random.Range(100, 999));
    }

    // Each player inside the same will call it whenever a new client player enters a room.
    public override void OnClientEnterRoom()
    {
        base.OnClientEnterRoom();
        if (!isSpawned)
        {
            SpawnDisplay();
            Debug.Log(playerName + " Display spawned.");
        }
    }

    public override void OnClientExitRoom()
    {
        base.OnClientExitRoom();
        DestroyDisplay();
    }

    public void OnNameChanged(string oldName, string newName) {
        Debug.Log("Name display changed for " + playerName);
        if(roomPlayerDisplay != null) roomPlayerDisplay.transform.Find("Text").GetComponent<Text>().text = playerName;
    }

    [Command]
    public void CmdSetupPlayer(string _name)
    {
        playerName = _name;
    }

    public void SpawnDisplay()
    {
        roomPlayerDisplay = Instantiate(roomPlayerDisplayPrefab, Vector2.zero, Quaternion.identity);
        roomPlayerDisplay.transform.Find("Text").GetComponent<Text>().text = playerName;
        roomPlayerDisplay.transform.SetParent(playerList.transform, false);
        isSpawned = true;
    }

    public void DestroyDisplay()
    {
        Destroy(roomPlayerDisplay);
    }

}
