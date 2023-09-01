using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System.Collections;
using System.Collections.Generic;

public class CharacterSelect : MonoBehaviour
{
    int[] options;
    public int selection = -1;

    public GameObject selectButton;
    public GameManager gameManager;
    public GameObject CharacterOptionPrefab;
    List<GameObject> optionsList = new();

    public void GeneratePanel(int[] list)
    {
        options = list;
        foreach (int i in list)
        {
            CharacterSO characterSO = Resources.Load<CharacterSO>("Objects/Characters/Character_" + i);
            GameObject characterOption = Instantiate(CharacterOptionPrefab, Vector2.zero, Quaternion.identity);
            characterOption.GetComponent<Image>().sprite = characterSO.sprite;
            characterOption.GetComponent<CharacterOption>().characterId = characterSO.characterId;
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
        ClearComponents();
    }

    void ClearComponents()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

}