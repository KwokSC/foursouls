using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Card/Character")]
public class CharacterSO : ScriptableObject
{
	public int characterId;
	public string characterName;
	public Sprite sprite;
	public int attack;
	public int health;
	public bool isActive;
	public ItemSO eternal;
}
