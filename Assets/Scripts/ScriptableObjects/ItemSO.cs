using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Card/Item")]
public class ItemSO: ScriptableObject
{
    public int itemId;
    public string itemName;
    public Sprite sprite;
    public bool isPassive;

    public virtual void Effect() { }
}

