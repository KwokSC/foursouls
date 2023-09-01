using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Loot", menuName = "Card/Loot")]
public class LootSO : ScriptableObject
{
    public int lootId;
    public string lootName;
    public int amount;
    public bool isItem;
    public Sprite sprite;

    public virtual void Effect() { }
}