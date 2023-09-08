using UnityEngine;

public class Option : MonoBehaviour
{
    public int id;

    public Type type;

    CharacterSelect panel;

    private void Start()
    {
        panel = GameObject.FindGameObjectWithTag("CharacterSelection").GetComponent<CharacterSelect>();
    }

    public enum Type
    {
        Character,
        Item
    }

    public void OnClick()
    {
        switch (type) {
            case Type.Character:
                panel.SelectCharacter(id);
                break;

            case Type.Item:
                panel.item = id;
                break;
        }

    }
}

