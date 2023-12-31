using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item: MonoBehaviour
{
    public enum ItemType
    {
        Eternal,
        Treasure,
        Loot
    }

    public int itemId;
    public string itemName;
    public ItemSO itemSO;
    public Image image;
    public bool isActivated;
    public bool isPassive;
    public ItemType type;
    public int player;

    void Start()
    {
        itemId = itemSO.itemId;
        itemName = itemSO.itemName;
        isPassive = itemSO.isPassive;
        isActivated = isPassive ? false : true;
        type = (ItemType)itemSO.type;
        image.sprite = itemSO.sprite;
    }

    public void ExecuteEffect() { 
    
    }

    public void ExecuteEffect(GameObject target) {

    }
}
