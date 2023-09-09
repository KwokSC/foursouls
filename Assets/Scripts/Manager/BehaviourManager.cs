using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BehaviourManager : MonoBehaviour
{
    public GameManager GameManager;

    public GameObject handCard;
    public Button dealButton;
    public Button endButton;
    PlayerManager player;

    void Update()
    {
        if (GameManager.gameState == GameManager.GameState.InGame) {
            if(player == null) player = GameManager.localPlayer;

            if (player.playerState == PlayerManager.PlayerState.Discard) dealButton.GetComponentInChildren<TextMeshPro>().text = "Discard";

            if (player.playerState == PlayerManager.PlayerState.Select) dealButton.GetComponentInChildren<TextMeshPro>().text = "Select";

            if (player.playerState == PlayerManager.PlayerState.Select) {
                dealButton.GetComponentInChildren<TextMeshPro>().text = "Deal";
                dealButton.interactable = player.dealTimes != 0 && player.playerState != PlayerManager.PlayerState.Dead;
            }

            endButton.interactable = player.isSelfTurn;
        }
    }

    public void OnDealClick() {
        List<GameObject> selectedLoot = new();

        foreach (Transform loot in handCard.transform) {
            if (loot.gameObject.CompareTag("Loots")) {
                LootSelect lootSelect = loot.gameObject.GetComponent<LootSelect>();
                if (lootSelect.isSelected) selectedLoot.Add(loot.gameObject);
            }
        }

        if (selectedLoot.Count == 1)
        {
            player.CmdDealCard(selectedLoot[0]);
        }
        else {
            Debug.Log("You can only deal one loot at the same time.");
        }
    }

    public void OnAttackClick() {

    }

    public void OnPurchaseClick() {

    }

    public void OnEndClick() {

    }
}
