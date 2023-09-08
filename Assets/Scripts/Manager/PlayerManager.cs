using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerManager : NetworkBehaviour
{
    public enum PlayerState
    {
        Idle,
        Dead,
        Attack,
        Toss,
        Select,
        Discard
    }

    [SyncVar]
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

    [SyncVar(hook = nameof(OnReadyChanged))]
    public bool isReady = false;

    [SyncVar(hook = nameof(OnTurnStatusChanged))]
    public bool isSelfTurn;

    [SyncVar(hook = nameof(OnStateChanged))]
    public PlayerState playerState;

    [SyncVar(hook = nameof(OnActivatedChanged))]
    public bool isActivated = false;

    public GameObject gamePlayerDisplay;

    public List<int> handCardList = new();
    public List<int> itemList = new();
    List<GameObject> lootObjects = new();
    List<GameObject> itemGameObjects = new();
    public int dealTimes = 0;
    public int attackTimes = 0;
    public int availableDeals = 1;
    public int availableAttack = 1;

    GameManager GameManager;
    UIManager UIManager;

    void Awake()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        UIManager = GameObject.Find("UIManager").GetComponent<UIManager>();
    }

    public void OnCharacterIdChanged(int oldId, int newId)
    {
        UIManager.OnCharacterUpdate(this);
    }

    public void OnAttackChanged(int oldAttack, int newAttack)
    {
        UIManager.OnAttackUpdate(this);
    }

    public void OnHealthChanged(int oldHealth, int newHealth)
    {
        UIManager.OnHealthUpdate(this);
    }

    public void OnCoinsChanged(int oldCoins, int newCoins)
    {
        UIManager.OnCoinsUpdate(this);
    }

    public void OnSoulsChanged(int oldSouls, int newSouls)
    {
        UIManager.OnSoulsUpdate(this);
    }

    public void OnReadyChanged(bool oldStatus, bool newStatus) {

    }

    public void OnActivatedChanged(bool oldStatus, bool newStatus)
    {
        UIManager.OnActivatedUpdate(gamePlayerDisplay.transform.Find("Avatar").gameObject, isActivated);
    }

    public void OnTurnStatusChanged(bool oldStatus, bool newStatus)
    {
        UIManager.OnTurnStatusUpdate(this);
    }

    public void OnStateChanged(PlayerState oldState, PlayerState newState)
    {

    }

    public void SetupPlayer(RoomPlayerManager roomPlayer)
    {
        playerName = roomPlayer.playerName;
        playerState = PlayerState.Idle;
    }

    public void DrawCard(int[] cardList)
    {
        if (!isLocalPlayer) return;
        handCardList.AddRange(cardList);
        foreach (int id in cardList)
        {
            lootObjects.Add(UIManager.SpawnLoot(id));
        }
    }

    public void DrawCard(int card)
    {
        if (!isLocalPlayer) return;
        handCardList.Add(card);
        lootObjects.Add(UIManager.SpawnLoot(card));
    }

    #region command
    [Command]
    public void CmdPlayerReady() {
        isReady = true;
    }

    [Command]
    public void CmdSetupCharacter(int character, int item)
    {
        characterId = character;
    }

    [Command]
    public void CmdSetupEternal(int item) {
        UIManager.SpawnItem(item);
    }

    [Command]
    public void CmdDealCard(GameObject loot)
    {
        if (dealTimes == 0) return;
        Loot lootComponent = loot.GetComponent<Loot>();
        lootComponent.ExecuteEffect();
        handCardList.Remove(lootComponent.lootId);
    }

    [Command]
    public void CmdDealCard(GameObject loot, GameObject target)
    {
        Loot lootComponent = loot.GetComponent<Loot>();
        lootComponent.ExecuteEffect();
        handCardList.Remove(lootComponent.lootId);
    }

    [Command]
    public void CmdActivateCharacter()
    {
        isActivated = true;
        dealTimes += 1;
    }

    [Command]
    public void CmdActivateItem(GameObject item)
    {
        if (isLocalPlayer) return;
        item.GetComponent<Item>().ExecuteEffect();
    }

    [Command]
    public void CmdActivatedItem(GameObject item, GameObject target) {

    }

    [Command]
    public void CmdAttackMonster(GameObject monster)
    {

    }

    [Command]
    public void CmdMoneyChange(int money)
    {
        if (!isLocalPlayer) return;
        coins += money;
    }

    #endregion
}
