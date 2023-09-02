using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject table;
    public GameObject handCard;
    public GameObject lootPrefab;
    public GameObject characterSelectionPrefab;
    public GameObject characterDisplay;
    public GameObject enemyArea;
    List<LootSO> lootResources;
    List<CharacterSO> characterResources;

    void Start()
    {
        lootResources = new List<LootSO>(Resources.LoadAll<LootSO>("Objects/Loots"));
        characterResources = new List<CharacterSO>(Resources.LoadAll<CharacterSO>("Objects/Characters"));
    }

    void Update()
    {
        AdjustHandCard();
    }

    // Here to adjust display spacing based on the players' hand cards amount.
    void AdjustHandCard()
    {
        int cardNum = handCard.transform.childCount;
        if (cardNum > 6)
        {
            handCard.GetComponent<GridLayoutGroup>().spacing = new Vector2(-15 * cardNum + 90, 0);
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
    }

    public void SpawnPlayerDisplay(PlayerManager player)
    {
        if (player.isLocalPlayer) player.transform.SetParent(characterDisplay.transform, false);
        else player.transform.SetParent(enemyArea.transform, false);
    }
}
