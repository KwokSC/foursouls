using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

public class RoomManager : NetworkRoomManager
{
    public override void OnStartHost()
    {
        base.OnStartHost();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
    }

    // Only call the first time a client player enters a room.
    public override void OnRoomClientEnter()
    {
        base.OnRoomClientEnter();
        foreach (RoomPlayerManager roomPlayer in roomSlots.Cast<RoomPlayerManager>())
        {
            if (roomPlayer.playerList == null)
            {
                roomPlayer.playerList = GameObject.Find("PlayerList");
                roomPlayer.SpawnDisplay();
            }
        }
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
    }

    public override GameObject OnRoomServerCreateGamePlayer(NetworkConnectionToClient conn, GameObject roomPlayer) {
        GameObject gamePlayerObject = Instantiate(playerPrefab, Vector2.zero, Quaternion.identity);
        PlayerManager gamePlayer = gamePlayerObject.GetComponent<PlayerManager>();
        gamePlayer.SetupPlayer(roomPlayer.GetComponent<RoomPlayerManager>());
        return gamePlayerObject;
    }
}