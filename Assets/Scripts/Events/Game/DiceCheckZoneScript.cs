using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DiceCheckZoneScript : MonoBehaviour
{
    Vector3 diceVelocity;
    public TextMeshProUGUI resultDisplay;

    private void FixedUpdate()
    {
        diceVelocity = DiceScript.diceVelocity;
    }

    private void OnTriggerStay(Collider col)
    {
        if (diceVelocity.x == 0f && diceVelocity.y == 0f && diceVelocity.z == 0f) {
            switch (col.gameObject.name) {
                case "Side1":
                    resultDisplay.text = 1.ToString();
                    break;
                case "Side2":
                    resultDisplay.text = 2.ToString();
                    break;
                case "Side3":
                    resultDisplay.text = 3.ToString();
                    break;
                case "Side4":
                    resultDisplay.text = 4.ToString();
                    break;
                case "Side5":
                    resultDisplay.text = 5.ToString();
                    break;
                case "Side6":
                    resultDisplay.text = 6.ToString();
                    break;

            }
        }
    }
}
