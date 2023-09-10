using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

public class RoomManager : NetworkRoomManager
{
    public RoomState roomState = RoomState.InRoom;

    public enum RoomState
    {
        InRoom,
        InGame
    }

    void ChangeRoomState(string sceneName) {
        if (sceneName.Equals(RoomScene)) {
            roomState = RoomState.InRoom;
        }
        else if(sceneName.Equals(GameplayScene)) {
            roomState = RoomState.InGame;
        }
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
            }
        }
    }

    public override GameObject OnRoomServerCreateRoomPlayer(NetworkConnectionToClient conn)
    {
        GameObject roomPlayerObject = Instantiate(roomPlayerPrefab.gameObject, Vector3.zero, Quaternion.identity);
        RoomPlayerManager roomPlayer = roomPlayerObject.GetComponent<RoomPlayerManager>();
        return roomPlayerObject;
    }

    public override GameObject OnRoomServerCreateGamePlayer(NetworkConnectionToClient conn, GameObject roomPlayerObject) {
        GameObject gamePlayerObject = Instantiate(playerPrefab, Vector2.zero, Quaternion.identity);
        PlayerManager gamePlayer = gamePlayerObject.GetComponent<PlayerManager>();
        RoomPlayerManager roomPlayer = roomPlayerObject.GetComponent<RoomPlayerManager>();
        gamePlayer.playerName = roomPlayer.playerName;
        gamePlayer.playerIndex = roomPlayer.index;
        return gamePlayerObject;
    }

    public override void OnRoomServerSceneChanged(string sceneName) {
        ChangeRoomState(sceneName);
    }
}