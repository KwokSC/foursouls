using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class PlayerManager : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnNameChanged))]
    public string playerName;

    [SyncVar(hook = nameof(OnCharacterIdChanged))]
    public int characterId = -1;

    [SyncVar(hook = nameof(OnAttackChanged))]
    public int attack;

    [SyncVar(hook = nameof(OnHealthChanged))]
    public int health;

    [SyncVar(hook = nameof(OnCoinsChanged))]
    public int coins;

    [SyncVar(hook = nameof(OnSoulsChanged))]
    public int souls;

    public bool isDead;

    public CharacterSO characterSO;
    public Image avatarImage;
    public Text nameText;
    public Text attackText;
    public Text healthText;
    public Text coinsText;
    public Text soulsText;

    public GameObject characterDisplay;
    public GameObject enemyArea;
    public CharacterSelect characterSelection;
    List<int> handCardList = new();

    GameManager gameManager;

    public override void OnStartClient()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        SpawnPlayerDisplay();
    }

    void Update() {
        LiveStatusChange();
    }

    public void OnNameChanged(string oldName, string newName)
    {
        nameText.text = playerName;
    }

    public void OnCharacterIdChanged(int oldId, int newId)
    {
        Debug.Log("The character id changed.");
        characterSO = Resources.Load<CharacterSO>("Objects/Characters/Character_" + characterId);
        avatarImage.sprite = characterSO.sprite;
        attack = characterSO.attack;
        health = characterSO.health;
    }

    public void OnAttackChanged(int oldAttack, int newAttack)
    {
        attackText.text = attack.ToString();
    }

    public void OnHealthChanged(int oldHealth, int newHealth)
    {
        healthText.text = health.ToString();
    }

    public void OnCoinsChanged(int oldCoins, int newCoins)
    {
        coinsText.text = coins.ToString();
    }

    public void OnSoulsChanged(int oldSouls, int newSouls) {
        soulsText.text = souls.ToString();
    }

    public void SetupPlayer(RoomPlayerManager roomPlayer) {
        playerName = roomPlayer.playerName;
        coins = 3;
        isDead = false;
    }

    [Command]
    public void CmdSetupCharacter(int id) {
        characterId = id;
    }

    [Command]
    public void CmdDrawCard() {
        if (!isLocalPlayer) return;
    }

    [Command]
    public void CmdDealCard() {
        if (!isLocalPlayer) return;
    }

    [Command]
    public void CmdUseItem() {
        if (!isLocalPlayer) return;
    }

    [Command]
    public void CmdPurchase() {
        if (!isLocalPlayer) return;
    }

    [Command]
    public void CmdAttackMonster(GameObject monster) {
        if (!isLocalPlayer) return;
    }

    void SpawnPlayerDisplay() {
        characterDisplay = GameObject.Find("Table/PlayerArea/CharacterDisplay");
        enemyArea = GameObject.Find("Table/EnemyArea");
        if (isLocalPlayer) transform.SetParent(characterDisplay.transform, false);
        else transform.SetParent(enemyArea.transform, false);
    }

    void LiveStatusChange() {
        if (health <= 0) isDead = true;
    }
}
