using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Loot : MonoBehaviour
{
    public int lootId;
    public int amount;
    public bool isItem;
    public string lootName;
    public LootSO lootSO;
    public Image image;

    // Start is called before the first frame update
    void Start()
    {
        lootId = lootSO.lootId;
        image.sprite = lootSO.sprite;
        amount = lootSO.amount;
        isItem = lootSO.isItem;
        lootName = lootSO.lootName;
    }

    public void ExecuteEffect() {
        foreach (CardEffectSO effect in lootSO.effects) { 
            effect.ExecuteEffects();
        }
    }
}
