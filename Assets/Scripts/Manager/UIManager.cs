using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public float hoverDuration = 0.5f;

    public GameObject table;
    public GameObject handCard;
    public GameObject itemList;
    public GameObject characterDisplay;
    public GameObject enemyArea;
    public GameObject timer;
    public TextMeshProUGUI broadcast;

    public GameObject dicePrefab;
    public GameObject lootPrefab;
    public GameObject itemPrefab;
    public GameObject characterSelectionPrefab;
    public GameObject gamePlayerDisplayPrefab;

    List<LootSO> lootResources;
    List<CharacterSO> characterResources;
    List<ItemSO> itemResources;
    List<MonsterSO> monsterReources;

    void Awake()
    {
        lootResources = new List<LootSO>(Resources.LoadAll<LootSO>("Objects/Loots"));
        characterResources = new List<CharacterSO>(Resources.LoadAll<CharacterSO>("Objects/Characters"));
        itemResources = new List<ItemSO>(Resources.LoadAll<ItemSO>("Objects/Items"));
        monsterReources = new List<MonsterSO>(Resources.LoadAll<MonsterSO>("Objects/Monsters"));
    }

    // Here to adjust display spacing based on the players' hand cards amount.
    void AdjustHandCard()
    {
        int cardNum = handCard.transform.childCount;
        float spacing = cardNum > 4 ? (600 - 150 * cardNum) / (cardNum - 1) : 0;
        float startPos = cardNum <= 4 ? -75 * (cardNum - 1) : -225;
        for (int i = 0; i < cardNum; i++)
        {
            handCard.transform.GetChild(i).GetComponent<LootSelect>().OnCardPosXChanged(startPos + i * (150 + spacing));
        }
    }

    public void AddBroadCast(string msg) {
        broadcast.text += "\n" + msg;
    }

    public void CharaterSelectDisplay(int[] characterOptions, float timeLimit)
    {
        GameObject CharacterSelection = Instantiate(characterSelectionPrefab, Vector2.zero, Quaternion.identity);
        CharacterSelection.transform.SetParent(table.transform, false);
        List<CharacterSO> optionResources = characterResources.Where(character => characterOptions.Contains(character.characterId)).ToList();
        CharacterSelection.GetComponent<CharacterSelect>().GeneratePanel(optionResources, timeLimit);
    }


    public void CharaterSelectDisplay(int[] characterOptions, int[] itemOptions, float timeLimit)
    {
        GameObject CharacterSelection = Instantiate(characterSelectionPrefab, Vector2.zero, Quaternion.identity);
        CharacterSelection.transform.SetParent(table.transform, false);
        List<CharacterSO> optionResources = characterResources.Where(character => characterOptions.Contains(character.characterId)).ToList();
        List<ItemSO> itemResources = this.itemResources.Where(item => itemOptions.Contains(item.itemId)).ToList();
        CharacterSelection.GetComponent<CharacterSelect>().GeneratePanel(optionResources, itemResources, timeLimit);
    }

    public GameObject SpawnLoot(int i)
    {
        LootSO lootResource = lootResources.Find(loot => loot.lootId == i);
        GameObject loot = Instantiate(lootPrefab, Vector2.zero, Quaternion.identity);
        loot.transform.SetParent(handCard.transform, false);
        loot.GetComponent<Loot>().lootSO = lootResource;
        AdjustHandCard();
        return loot;
    }

    public GameObject SpawnItem(int i)
    {
        ItemSO itemResource = itemResources.Find(item => item.itemId == i);
        GameObject item = Instantiate(itemPrefab, Vector2.zero, Quaternion.identity);
        item.transform.SetParent(itemList.transform, false);
        item.GetComponent<Item>().itemSO = itemResource;
        return item;
    }

    public GameObject SpawnPlayerDisplay(PlayerManager player)
    {
        GameObject gamePlayerDisplay = Instantiate(gamePlayerDisplayPrefab, Vector3.zero, Quaternion.identity);
        gamePlayerDisplay.GetComponent<GamePlayerDisplay>().playerName.text = player.playerName;
        gamePlayerDisplay.GetComponent<GamePlayerDisplay>().player = player;
        if (player.isLocalPlayer)
        {
            gamePlayerDisplay.transform.SetParent(characterDisplay.transform, false);
        }
        else
        {
            gamePlayerDisplay.transform.SetParent(enemyArea.transform, false);
        }
        return gamePlayerDisplay;
    }

    public void OnTurnStatusUpdate(PlayerManager player)
    {
        if (player.isSelfTurn)
        {
            timer.GetComponent<TimerScript>().StartCountdown(60);
        }
        else
        {
            timer.GetComponent<Text>().text = "Your turn end";
        }
    }

    public void OnCharacterUpdate(PlayerManager player)
    {
        CharacterSO character = characterResources.Find(c => c.characterId == player.characterId);
        player.gamePlayerDisplay.GetComponent<GamePlayerDisplay>().characterImg.sprite = character.sprite;
        player.gamePlayerDisplay.GetComponent<GamePlayerDisplay>().attackText.text = character.attack.ToString();
        player.gamePlayerDisplay.GetComponent<GamePlayerDisplay>().healthText.text = character.health.ToString();
    }

    public void OnAttackUpdate(PlayerManager player)
    {
        player.gamePlayerDisplay.GetComponent<GamePlayerDisplay>().attackText.text = player.attack.ToString();
    }

    public void OnHealthUpdate(PlayerManager player)
    {
        player.gamePlayerDisplay.GetComponent<GamePlayerDisplay>().healthText.text = player.health.ToString();
    }

    public void OnCoinsUpdate(PlayerManager player)
    {
        player.gamePlayerDisplay.GetComponent<GamePlayerDisplay>().coinsText.text = player.coins.ToString();
    }

    public void OnSoulsUpdate(PlayerManager player)
    {
        player.gamePlayerDisplay.GetComponent<GamePlayerDisplay>().soulsText.text = player.souls.ToString();
    }

    public void OnActivatedUpdate(GameObject card, bool isActivated)
    {
        StartCoroutine(RotateCard(card, isActivated));
    }


    #region ienumerator
    IEnumerator RotateCard(GameObject card, bool isActivated)
    {
        float elapsedTime = 0f;
        Vector3 originalRotation = card.transform.eulerAngles;
        Quaternion targetRotation = isActivated ? Quaternion.Euler(originalRotation.x, originalRotation.y, originalRotation.z + 90f) : Quaternion.Euler(Vector3.zero);
        while (elapsedTime < hoverDuration)
        {
            card.transform.localScale = Vector3.Lerp(card.transform.localScale, Vector3.one, elapsedTime / hoverDuration);
            card.transform.rotation = Quaternion.Slerp(card.transform.rotation, targetRotation, elapsedTime / hoverDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        card.transform.localScale = Vector3.one;
        card.transform.rotation = targetRotation;
    }
    #endregion
}
