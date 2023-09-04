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
    public int attack = 0;

    [SyncVar(hook = nameof(OnHealthChanged))]
    public int health = 0;

    [SyncVar(hook = nameof(OnCoinsChanged))]
    public int coins = 0;

    [SyncVar(hook = nameof(OnSoulsChanged))]
    public int souls = 0;

    [SyncVar(hook = nameof(OnTurnStatusChanged))]
    public bool isSelfTurn;

    [SyncVar(hook =nameof(OnStateChanged))]
    public PlayerState playerState;

    public GameObject gamePlayerDisplay;

    List<int> handCardList = new();
    int availableDeals = 0;

    GameManager GameManager;
    UIManager UIManager;

    private void Awake()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        UIManager = GameObject.Find("UIManager").GetComponent<UIManager>();
    }

    public void OnNameChanged(string oldName, string newName)
    {
        if(gamePlayerDisplay != null)
            UIManager.OnPlayerNameUpdate(this);
    }

    public void OnCharacterIdChanged(int oldId, int newId)
    {
        if(gamePlayerDisplay != null)
            UIManager.OnCharacterUpdate(this);
    }

    public void OnAttackChanged(int oldAttack, int newAttack)
    {

        if (gamePlayerDisplay != null)
            UIManager.OnAttackUpdate(this);
    }

    public void OnHealthChanged(int oldHealth, int newHealth)
    {
        if (gamePlayerDisplay != null)
            UIManager.OnHealthUpdate(this);
    }

    public void OnCoinsChanged(int oldCoins, int newCoins)
    {

        if (gamePlayerDisplay != null)
            UIManager.OnCoinsUpdate(this);
    }

    public void OnSoulsChanged(int oldSouls, int newSouls) {

        if (gamePlayerDisplay != null)
            UIManager.OnSoulsUpdate(this);
    }

    public void OnTurnStatusChanged(bool oldStatus, bool newStatus) {

    }

    public void OnStateChanged(PlayerState oldState, PlayerState newState) { 

    }

    public void SetupPlayer(RoomPlayerManager roomPlayer) {
        playerName = roomPlayer.playerName;
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
    public void CmdDealCard(GameObject loot) {
        if (!isLocalPlayer) return;
        Loot lootComponent = loot.GetComponent<Loot>();
        lootComponent.ExecuteEffect();
        handCardList.Remove(lootComponent.lootId);
    }

    [Command]
    public void CmdDealCard(GameObject loot, GameObject target) {
        if (!isLocalPlayer && availableDeals==0) return;
    }

    [Command]
    public void CmdActivateObject() {
        if (!isLocalPlayer) return;
    }

    [Command]
    public void CmdPurchase(GameObject treasure) {
        if (!isLocalPlayer) return;
    }

    [Command]
    public void CmdAttackMonster(GameObject monster) {
        if (!isLocalPlayer) return;
    }

    [Command]
    public void CmdMoneyChange(int money) { 
        if (!isLocalPlayer) return;
        coins += money;
    }
    #endregion
}
