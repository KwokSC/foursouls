using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System;

public class PlayerManager : NetworkBehaviour
{
    public enum PlayerState { 
        Idle,
        Attack,
        Dead
    }

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

    [SyncVar(hook = nameof(OnTurnStatusChanged))]
    public bool isSelfTurn;

    [SyncVar(hook =nameof(OnStateChanged))]
    public PlayerState playerState;

    public CharacterSO characterSO;
    public Image avatarImage;
    public Text nameText;
    public Text attackText;
    public Text healthText;
    public Text coinsText;
    public Text soulsText;

    List<int> handCardList = new();

    GameManager GameManager;
    UIManager UIManager;

    public override void OnStartClient()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        UIManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        UIManager.SpawnPlayerDisplay(this);
    }

    public void OnNameChanged(string oldName, string newName)
    {
        nameText.text = playerName;
    }

    public void OnCharacterIdChanged(int oldId, int newId)
    {
        characterSO = Resources.Load<CharacterSO>("Objects/Characters/Character_" + characterId);
        avatarImage.sprite = characterSO.sprite;
        attack = characterSO.attack;
        health = characterSO.health;
        Debug.Log(playerName + "'s character is: " + characterSO.characterName);
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

    public void OnTurnStatusChanged(bool oldStatus, bool newStatus) {

    }

    public void OnStateChanged(PlayerState oldState, PlayerState newState) { 
    
    }

    public void SetupPlayer(RoomPlayerManager roomPlayer) {
        playerName = roomPlayer.playerName;
        coins = 3;
        playerState = PlayerState.Idle;
    }

    public void DrawCard(int[] cardList)
    {
        if (!isLocalPlayer) return;
        handCardList.AddRange(cardList);
        foreach (int id in cardList)
        {
            UIManager.SpawnCard(id);
        }
    }

    public void DrawCard(int card)
    {
        if (!isLocalPlayer) return;
        handCardList.Add(card);
        UIManager.SpawnCard(card);
    }

    #region command
    [Command]
    public void CmdSetupCharacter(int id) {
        characterId = id;
    }

    [Command]
    public void CmdDealCard(int id) {
        if (!isLocalPlayer) return;
    }

    [Command]
    public void CmdDealCard(int id, GameObject target) {
        if (!isLocalPlayer) return;
    }

    [Command]
    public void CmdActivateObject() {
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

    [Command]
    public void CmdGainMoney(int money) { 
        if (!isLocalPlayer) return;
        coins += money;
    }
    #endregion
}
