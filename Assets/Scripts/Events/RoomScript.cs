using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class RoomScript : MonoBehaviour
{
    public GameObject PlayerList;
    public GameObject ReadyButton;
    public GameObject roomPlayerDisplayPrefab;
    NetworkRoomManager room = NetworkManager.singleton as NetworkRoomManager;
    bool isReady = false;

    void Start()
    {
        if (NetworkServer.active)
        {
            ReadyButton.GetComponent<Text>().text = "Start";
            ReadyButton.GetComponent<Button>().interactable = false;
        }
        else
        {
            ReadyButton.GetComponent<Text>().text = "Ready";
        }
    }

    void Update()
    {
        UpdateButtonText();
    }

    void UpdateButtonText()
    {
        if (NetworkServer.active)
        {
            if (room.roomSlots.Count > 1)
            {
                bool interactable = true;
                foreach (RoomPlayerManager roomPlayer in room.roomSlots)
                {
                    if (roomPlayer.isLocalPlayer)
                    {
                        continue;
                    }

                    if (!roomPlayer.readyToBegin)
                    {
                        interactable = false;
                        break;

                    }
                }
                ReadyButton.GetComponent<Button>().interactable = interactable;
            }

        }
        else
        {
            for (int i = 0; i < room.roomSlots.Count; i++)
            {
                RoomPlayerManager roomPlayer = (RoomPlayerManager)room.roomSlots[i];
                if (roomPlayer.isLocalPlayer)
                {
                    ReadyButton.GetComponent<Text>().text = roomPlayer.readyToBegin ? "Cancel" : "Ready";
                }
            }
        }
    }

    public void OnClickReady()
    {
        isReady = !isReady;
        foreach (RoomPlayerManager player in room.roomSlots)
        {
            if (player.isLocalPlayer)
            {
                player.CmdChangeReadyState(isReady);
            }
        }
    }

    public void JumptoMenu()
    {
        if (NetworkServer.active)
        {
            room.StopServer();
        }
        else {
            room.StopClient();
        }
    }

    //public GameObject SpawnRoomPlayerDisplay(RoomPlayerManager roomPlayer)
    //{
    //    if (roomPlayerDisplayPrefab != null)
    //    {
    //        GameObject roomPlayerDisplay = Instantiate(roomPlayerDisplayPrefab, Vector2.zero, Quaternion.identity);
    //        roomPlayerDisplay.GetComponent<RoomPlayerDisplay>().PlayerName.text = roomPlayer.playerName;
    //        roomPlayerDisplay.transform.SetParent(PlayerList.transform);
    //        return roomPlayerDisplay;
    //    }
    //    return null;
    //}

    //public void DestroyRoomPlayerDisplay(GameObject displayObject)
    //{
    //    if (displayObject != null)
    //    {
    //        Destroy(displayObject);
    //    }
    //}
}
