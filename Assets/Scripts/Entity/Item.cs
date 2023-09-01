using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item: MonoBehaviour
{
    public int itemId;
    public string itemName;
    public ItemSO itemSO;
    public Image image;
    public bool isActive;
    public bool isPassive;

    private void Start()
    {
        itemId = itemSO.itemId;
        itemName = itemSO.itemName;
        isPassive = itemSO.isPassive;
        isActive = isPassive ? false : true;
        image.sprite = itemSO.sprite;
    }
}
