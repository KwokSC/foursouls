using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject handCard;
    public GameObject lootPrefab;
    public GameObject CharacterSelectionPrefab;
    public GameObject Canvas;

    void Update()
    {
        AdjustHandCard();
    }

    public void CharaterSelectDisplay(int[] options, float timeLimit)
    {
        GameObject CharacterSelection = Instantiate(CharacterSelectionPrefab, Vector2.zero, Quaternion.identity);
        Canvas = GameObject.Find("Canvas");
        CharacterSelection.transform.SetParent(Canvas.transform, false);
        CharacterSelection.GetComponent<CharacterSelect>().GeneratePanel(options);
    }

    // Here to adjust display spacing based on the players' hand cards amount.
    void AdjustHandCard()
    {
        int cardNum = handCard.transform.childCount;
        if (cardNum > 6)
        {
            handCard.GetComponent<GridLayoutGroup>().spacing = new Vector2(-15 * cardNum + 90, 0);
        }
    }
}
