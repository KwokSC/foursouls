using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Card/Item")]
public class ItemSO: ScriptableObject
{
    public enum ItemType {
        Eternal,
        Treasure,
        Loot
    }

    public int itemId;
    public string itemName;
    public Sprite sprite;
    public bool isPassive;
    public ItemType type;
    public List<CardEffectSO> effects;
}