using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CharacterSelect : MonoBehaviour
{
    int[] options;
    public int selection = -1;

    public GameObject selectButton;
    public GameManager gameManager;
    public GameObject CharacterOptionPrefab;
    List<GameObject> optionsList = new();

    public void GeneratePanel(int[] list, List<CharacterSO> optionResources)
    {
        options = list;
        foreach (CharacterSO resource in optionResources)
        {
            GameObject characterOption = Instantiate(CharacterOptionPrefab, Vector2.zero, Quaternion.identity);
            characterOption.GetComponent<Image>().sprite = resource.sprite;
            characterOption.GetComponent<CharacterOption>().characterId = resource.characterId;
            characterOption.transform.SetParent(transform.Find("CharacterOptions"), false);
            optionsList.Add(characterOption);
        }
    }

    public int RandomSelect()
    {
        return options[Random.Range(0, options.Length)];
    }

    public void SubmitSelection()
    {
        foreach (GameObject option in optionsList)
        {
            if (option.GetComponent<CharacterOption>().isSelected)
            {
                selection = option.GetComponent<CharacterOption>().characterId;
                break;
            }
        }
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (selection == -1) return;
        gameManager.localPlayer.CmdSetupCharacter(selection);
        Destroy(this);
    }

}