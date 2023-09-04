using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject table;
    public GameObject handCard;
    public GameObject lootPrefab;
    public GameObject characterSelectionPrefab;
    public GameObject characterDisplay;
    public GameObject enemyArea;
    public GameObject gamePlayerDisplayPrefab;
    public GameObject heartPrefab;
    List<LootSO> lootResources;
    List<CharacterSO> characterResources;

    void Awake()
    {
        lootResources = new List<LootSO>(Resources.LoadAll<LootSO>("Objects/Loots"));
        characterResources = new List<CharacterSO>(Resources.LoadAll<CharacterSO>("Objects/Characters"));
    }

    // Here to adjust display spacing based on the players' hand cards amount.
    void AdjustHandCard()
    {
        int cardNum = handCard.transform.childCount;
        float spacing = cardNum>6?(900 - 150 * cardNum) / (cardNum - 1):0;
        float startPos = cardNum < 6 ? -75 * (cardNum-1): 75;
        for (int i = 0; i < cardNum; i++) {
            handCard.transform.GetChild(i).GetComponent<LootUIEffect>().OnCardPosXChanged(startPos + i * (150 + spacing));
        }
    }

    public void CharaterSelectDisplay(int[] options, float timeLimit)
    {
        GameObject CharacterSelection = Instantiate(characterSelectionPrefab, Vector2.zero, Quaternion.identity);
        CharacterSelection.transform.SetParent(table.transform, false);
        List<CharacterSO> optionResources = characterResources.Where(character => options.Contains(character.characterId)).ToList();
        CharacterSelection.GetComponent<CharacterSelect>().GeneratePanel(options, optionResources);
    }

    public void SpawnCard(int i)
    {
        LootSO lootResource = lootResources.Find(loot => loot.lootId == i);
        GameObject loot = Instantiate(lootPrefab, Vector2.zero, Quaternion.identity);
        loot.transform.SetParent(handCard.transform, false);
        loot.GetComponent<Loot>().lootSO = lootResource;
        AdjustHandCard();
    }

    public GameObject SpawnPlayerDisplay(PlayerManager player)
    {
        GameObject gamePlayerDisplay = Instantiate(gamePlayerDisplayPrefab, Vector3.zero, Quaternion.identity);
        gamePlayerDisplay.GetComponent<GamePlayerDisplay>().playerName.text = player.playerName;
        if (player.isLocalPlayer) {
            gamePlayerDisplay.transform.SetParent(characterDisplay.transform, false);
        }
        else {
            gamePlayerDisplay.transform.SetParent(enemyArea.transform, false);
        }
        return gamePlayerDisplay;
    }

    public void OnPlayerNameUpdate(PlayerManager player) {
        player.gamePlayerDisplay.GetComponent<GamePlayerDisplay>().playerName.text = player.playerName;
    }

    public void OnCharacterUpdate(PlayerManager player) {
        CharacterSO character = characterResources.Find(c => c.characterId == player.characterId);
        player.gamePlayerDisplay.GetComponent<GamePlayerDisplay>().characterImg.sprite = character.sprite;
        player.gamePlayerDisplay.GetComponent<GamePlayerDisplay>().attackText.text = character.attack.ToString();
        player.gamePlayerDisplay.GetComponent<GamePlayerDisplay>().healthText.text = character.health.ToString();
    }

    public void OnAttackUpdate(PlayerManager player) {
        player.gamePlayerDisplay.GetComponent<GamePlayerDisplay>().attackText.text = player.attack.ToString();
    }

    public void OnHealthUpdate(PlayerManager player) {
        player.gamePlayerDisplay.GetComponent<GamePlayerDisplay>().healthText.text = player.health.ToString();
    }

    public void OnCoinsUpdate(PlayerManager player) {
        player.gamePlayerDisplay.GetComponent<GamePlayerDisplay>().coinsText.text = player.coins.ToString();
    }

    public void OnSoulsUpdate(PlayerManager player) {
        player.gamePlayerDisplay.GetComponent<GamePlayerDisplay>().soulsText.text = player.souls.ToString();
    }
}
