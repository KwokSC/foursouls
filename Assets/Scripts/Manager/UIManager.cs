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
    public Transform[] spawnArea;
    public GameObject timer;
    public TextMeshProUGUI broadcast;

    public GameObject dicePrefab;
    public GameObject lootPrefab;
    public GameObject itemPrefab;
    public GameObject characterSelectionPrefab;

    List<LootSO> lootResources;
    List<CharacterSO> characterResources;
    List<ItemSO> itemResources;
    List<MonsterSO> monsterReources;

    public GameManager GameManager;

    void Awake()
    {
        lootResources = new List<LootSO>(Resources.LoadAll<LootSO>("Objects/Loots"));
        characterResources = new List<CharacterSO>(Resources.LoadAll<CharacterSO>("Objects/Characters"));
        itemResources = new List<ItemSO>(Resources.LoadAll<ItemSO>("Objects/Items"));
        monsterReources = new List<MonsterSO>(Resources.LoadAll<MonsterSO>("Objects/Monsters"));
    }

    // Here to adjust display spacing based on the players' hand cards amount.
    void AdjustHandCard(Transform handCard)
    {
        int cardNum = handCard.childCount;
        float spacing = cardNum > 4 ? (600 - 150 * cardNum) / (cardNum - 1) : 0;
        float startPos = cardNum <= 4 ? -75 * (cardNum - 1) : -225;
        for (int i = 0; i < cardNum; i++)
        {
            handCard.GetChild(i).GetComponent<LootSelect>().OnCardPosXChanged(startPos + i * (150 + spacing));
        }
    }

    public void AddBroadCast(string msg)
    {
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

    public GameObject SpawnLoot(PlayerManager player, int i)
    {
        GameObject lootObject = Instantiate(lootPrefab, Vector2.zero, Quaternion.identity);
        Loot loot = lootObject.GetComponent<Loot>();
        LootSO lootResource = lootResources.Find(loot => loot.lootId == i);
        loot.lootSO = lootResource;
        loot.player = player.playerIndex;
        return lootObject;
    }

    public void ShowLoot(GameObject loot)
    {

    }

    public GameObject SpawnItem(PlayerManager player, int i)
    {
        ItemSO itemResource = itemResources.Find(item => item.itemId == i);
        GameObject itemObject = Instantiate(itemPrefab, Vector2.zero, Quaternion.identity);
        Item item = itemObject.GetComponent<Item>();
        item.itemSO = itemResource;
        item.player = player.playerIndex;
        return itemObject;
    }

    public void ShowItem(GameObject item)
    {

    }

    public void ShowPlayerDisplay(PlayerManager player) {
        Transform spawnPosition = spawnArea[(player.playerIndex - GameManager.localPlayerIndex + 4) % 4];
        player.transform.SetParent(spawnPosition, false);
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

    public CharacterSO OnCharacterUpdate(PlayerManager player)
    {
        CharacterSO character = characterResources.Find(c => c.characterId == player.characterId);
        player.GetComponent<GamePlayerDisplay>().characterImg.sprite = character.sprite;
        return character;
    }

    public void OnAttackUpdate(PlayerManager player)
    {
        player.GetComponent<GamePlayerDisplay>().attackText.text = player.attack.ToString();
    }

    public void OnHealthUpdate(PlayerManager player)
    {
        player.GetComponent<GamePlayerDisplay>().healthText.text = player.health.ToString();
    }

    public void OnCoinsUpdate(PlayerManager player)
    {
        player.GetComponent<GamePlayerDisplay>().coinsText.text = player.coins.ToString();
    }

    public void OnSoulsUpdate(PlayerManager player)
    {
        player.GetComponent<GamePlayerDisplay>().soulsText.text = player.souls.ToString();
    }

    public void OnActivatedUpdate(GameObject card, bool isActivated)
    {
        card.GetComponent<ActivateHoverEffect>().OnIsActivatedChanged(isActivated);
    }
}
