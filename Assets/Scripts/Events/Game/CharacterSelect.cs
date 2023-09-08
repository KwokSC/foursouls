using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CharacterSelect : MonoBehaviour
{
    List<CharacterSO> characterOptions;
    List<ItemSO> eternalOptions;

    public int character = -1;
    public int item = -1;

    public GameObject optionArea;
    public GameObject timer;
    public GameObject optionPrefab;
    List<GameObject> optionsList = new();
    List<GameObject> eternalList = new();

    public GameManager GameManager;

    private void Awake()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void Update() {
        if (character != -1 && item != -1) {
            GameManager.localPlayer.CmdSetupCharacter(character, item);
            GameManager.localPlayer.CmdPlayerReady();
        }
    }

    public void GeneratePanel(List<CharacterSO> optionResources, float limit)
    {
        characterOptions = optionResources;
        timer.GetComponent<TimerScript>().StartCountdown(limit);
        foreach (CharacterSO resource in optionResources)
        {
            GameObject characterOption = Instantiate(optionPrefab, Vector2.zero, Quaternion.identity);
            characterOption.GetComponent<Image>().sprite = resource.sprite;
            characterOption.GetComponent<Option>().id = resource.characterId;
            characterOption.transform.SetParent(optionArea.transform, false);
            optionsList.Add(characterOption);
        }
    }

    public void GeneratePanel(List<CharacterSO> optionResources, List<ItemSO> itemOptions, float limit)
    {
        characterOptions = optionResources;
        eternalOptions = itemOptions;
        timer.GetComponent<TimerScript>().StartCountdown(limit);
        foreach (CharacterSO resource in optionResources)
        {
            GameObject characterOption = Instantiate(optionPrefab, Vector2.zero, Quaternion.identity);
            characterOption.GetComponent<Image>().sprite = resource.sprite;
            characterOption.GetComponent<Option>().id = resource.characterId;
            characterOption.GetComponent<Option>().type = Option.Type.Character;
            characterOption.transform.SetParent(optionArea.transform, false);
            optionsList.Add(characterOption);
        }
        foreach (ItemSO item in itemOptions)
        {
            GameObject eternalItem = Instantiate(optionPrefab, Vector2.zero, Quaternion.identity);
            eternalItem.GetComponent<Image>().sprite = item.sprite;
            eternalItem.GetComponent<Option>().id = item.itemId;
            eternalItem.GetComponent<Option>().type = Option.Type.Item;
            eternalList.Add(eternalItem);
        }
    }

    public void SelectCharacter(int id)
    {
        character = id;
        if (id != 3)
            item = characterOptions.Find(character => character.characterId == id).eternal;
        else
            SelectItems();
    }

    public void SelectItems()
    {
        foreach (Transform child in optionArea.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (GameObject item in eternalList)
        {
            item.transform.SetParent(optionArea.transform, false);
        }
    }

    public int RandomSelectCharacter()
    {
        return characterOptions[Random.Range(0, characterOptions.Count)].characterId;
    }

    public int RandomSelectEternal()
    {
        return eternalOptions[Random.Range(0, eternalOptions.Count)].itemId;
    }

}