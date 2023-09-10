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

    [SyncVar]
    public int playerIndex;

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

    [SyncVar(hook = nameof(OnStateChanged))]
    public PlayerState playerState;

    [SyncVar(hook = nameof(OnActivatedChanged))]
    public bool isActivated = false;

    public List<int> handCardList = new();
    public List<int> itemList = new();

    public List<GameObject> lootObjects = new();
    public List<GameObject> itemObjects = new();

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

    public override void OnStartClient()
    {
        base.OnStartClient();
    }

    #region hooks
    public void OnCharacterIdChanged(int oldId, int newId)
    {
        CharacterSO character = UIManager.OnCharacterUpdate(this);
        attack = character.attack;
        health = character.health;
    }

    public void OnAttackChanged(int oldAttack, int newAttack)
    {
        UIManager.OnAttackUpdate(this);
    }

    public void OnHealthChanged(int oldHealth, int newHealth)
    {
        UIManager.OnHealthUpdate(this);
        if (health <= 0) playerState = PlayerState.Dead;
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
        UIManager.OnActivatedUpdate(transform.Find("Avatar").gameObject, isActivated);
    }

    public void OnTurnStatusChanged(bool oldStatus, bool newStatus)
    {
        UIManager.OnTurnStatusUpdate(this);
    }

    public void OnStateChanged(PlayerState oldState, PlayerState newState)
    {

    }
    #endregion

    #region command

    [Command]
    public void CmdDrawCard(int[] cardList)
    {

    }

    [Command]
    public void CmdDrawCard(int card)
    {

    }

    [Command]
    public void CmdSetupCharacter(int character, int item)
    {

    }

    [Command]
    public void CmdDealCard(GameObject loot)
    {

    }

    [Command]
    public void CmdDealCard(GameObject loot, GameObject target)
    {

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

    }

    [Command]
    public void CmdActivatedItem(GameObject item, GameObject target)
    {

    }

    [Command]
    public void CmdAttackMonster(GameObject monster)
    {

    }

    [Command]
    public void CmdMoneyChange(int money)
    {
        coins += money;
    }

    #endregion

    #region rpc

    [ClientRpc]
    public void RpcShowLoot(GameObject card) {
        UIManager.ShowLoot(card);
    }

    [ClientRpc]
    public void RpcShowItem(GameObject item) {
        UIManager.ShowItem(item);
    }

    [ClientRpc]
    public void RpcShowDisplay() {
        UIManager.ShowPlayerDisplay(this);
    }

    #endregion
}
