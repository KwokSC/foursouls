using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Effect", menuName = "Effect")]
public class CardEffectSO : ScriptableObject
{
    [SerializeReference]
    public ICardEffect effect;

    public void ExecuteEffects()
    {
        effect.ExecuteEffect();
    }
}
