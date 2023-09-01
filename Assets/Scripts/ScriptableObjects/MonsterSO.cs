using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Monster", menuName = "Card/Monster")]
public class MonsterSO : ScriptableObject
{
    public int monsterId;
    public string monsterName;
    public int attack;
    public int health;
    public bool isBoss;
    public bool isCurse;
    public int soulsAmount;
    public bool isEffect;
    public Sprite sprite;

    public virtual void Effect() { }
}
