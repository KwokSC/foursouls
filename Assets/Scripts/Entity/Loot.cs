using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Loot : MonoBehaviour
{
    public int lootId;
    public int amount;
    public string lootName;
    public LootSO lootSO;
    public Image image;
    public int player;

    // Start is called before the first frame update
    void Start()
    {
        lootId = lootSO.lootId;
        image.sprite = lootSO.sprite;
        amount = lootSO.amount;
        lootName = lootSO.lootName;
    }

    public void ExecuteEffect() {
        lootSO.effect.ExecuteEffect();
    }

    public void ExecuteEffect(GameObject target) {
        switch (target.name) {
            case "GamePlayer":
                PlayerManager targetPlayer = target.GetComponent<PlayerManager>();
                break;
            case "Monster":
                MonsterManager monster = target.GetComponent<MonsterManager>();
                break;
        }
    }
}
