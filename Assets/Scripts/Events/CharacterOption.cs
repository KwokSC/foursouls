using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CharacterOption : MonoBehaviour
{
    public int characterId;

    public bool isSelected;

    private void Start()
    {
        isSelected = false;
    }

    public void OnSelect() {
        isSelected = true;
    }
}

