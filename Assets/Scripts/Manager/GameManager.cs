using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{
    List<int> lootDeck = new();
    List<int> characterDeck = new();
    List<int> monsterDeck = new();
    List<int> treasureDeck = new();
    List<int> discardLootDeck = new();
    List<int> discardMonsterDeck = new();
    List<int> discardTreasureDeck = new();

    CharacterSO[] characterResources;
    MonsterSO[] monsterResources;
    ItemSO[] itemResources;
    LootSO[] lootResources;

    public List<PlayerManager> playerList = new();
    public PlayerManager localPlayer;
    public PlayerManager winner;

    public UIManager UIManager;
    public AudioManager AudioManager;
    public BehaviourManager BehaviourManager;

    IEnumerator gameProcess;
    IEnumerator currentGameStage;

    public enum GameState
    {
        ServerInitialization,
        WaitingForPlayers,
        CharacterSelection,
        InGame
    }

    int startIndex = -1;

    [SyncVar(hook = nameof(OnCurrentPlayerChanged))]
    public int currentPlayerIndex = -1;

    [SyncVar(hook = nameof(OnGameStateChanged))]
    public GameState gameState = GameState.ServerInitialization;

    [SyncVar(hook = nameof(OnEndGameChanged))]
    public bool isGameOver = false;

    public float characterSelectionDuration = 20f;
    public float roundDuration = 60f;

    [Server]
    public override void OnStartServer()
    {
        if (gameProcess != null) { StopCoroutine(gameProcess); gameProcess = null; };
        StartCoroutine(GameProcess());
    }

    #region hooks

    public void OnCurrentPlayerChanged(int oldPlayer, int newPlayer)
    {
        UIManager.AddBroadCast("Now is " + playerList[currentPlayerIndex].playerName + "'s turn.");
    }

    public void OnGameStateChanged(GameState oldState, GameState newState)
    {
        UIManager.AddBroadCast("Now the game state is " + gameState);
    }

    public void OnEndGameChanged(bool oldStatus, bool newStatus)
    {
        if (gameProcess != null)
        {
            StopCoroutine(gameProcess);
            gameProcess = null;
        }
        RpcDeclareVictory(winner.playerName);
    }

    #endregion

    #region server only

    [Server]
    void InitializeLootDeck()
    {
        lootResources = Resources.LoadAll<LootSO>("Objects/Loots");
        for (int i = 0; i < lootResources.Length; i++)
        {
            for (int j = 0; j < lootResources[i].amount; j++)
                lootDeck.Add(lootResources[i].lootId);
        }
        ShuffleDeck(lootDeck);
    }

    [Server]
    void InitializeCharacterDeck()
    {
        characterResources = Resources.LoadAll<CharacterSO>("Objects/Characters");
        for (int i = 0; i < characterResources.Length; i++)
        {
            characterDeck.Add(characterResources[i].characterId);
        }
        ShuffleDeck(characterDeck);
    }

    [Server]
    void InitializeMonsterDeck()
    {
        monsterResources = Resources.LoadAll<MonsterSO>("Objects/Monsters");
        for (int i = 0; i < monsterResources.Length; i++)
        {
            monsterDeck.Add(monsterResources[i].monsterId);
        }
        ShuffleDeck(monsterDeck);
    }

    [Server]
    void InitializeTreasureDeck()
    {
        itemResources = Resources.LoadAll<ItemSO>("Objects/Items");
        for (int i = 0; i < itemResources.Length; i++)
        {
            if (itemResources[i].type == ItemSO.ItemType.Treasure) {
                treasureDeck.Add(itemResources[i].itemId);
            }
        }
        ShuffleDeck(treasureDeck);
    }

    [Server]
    void ShuffleDeck<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    [Server]
    bool CheckClientReady()
    {
        bool isReady = true;
        int count = 0;
        foreach (NetworkConnectionToClient connection in NetworkServer.connections.Values)
        {
            if (!connection.isReady)
            {
                isReady = false;
                count++;
            }
        }
        foreach (NetworkConnectionToClient connection in NetworkServer.connections.Values)
        {
            if (connection.isReady)
            {
                TargetWaitingForOtherPlayers(connection, count);
            }
        }
        return isReady;
    }

    [Server]
    void GenerateCharacterOptions(float selectionTimeLimit)
    {
        int i = 0;
        foreach (NetworkConnectionToClient connection in NetworkServer.connections.Values)
        {
            int[] characterOptions = characterDeck.GetRange(i, 3).ToArray();
            if (characterOptions.Contains(3))
            {
                TargetSendCharacterOptions(connection, characterOptions, treasureDeck.GetRange(0,3).ToArray(), selectionTimeLimit);
                treasureDeck.RemoveRange(0,3);
            }
            else {
                TargetSendCharacterOptions(connection, characterOptions, selectionTimeLimit);
            }
            i += 3;
        }
    }

    [Server]
    bool CheckPlayerReady()
    {
        foreach (PlayerManager player in playerList)
        {
            return player.isReady;
        }
        return true;
    }

    [Server]
    void SendHandCard(NetworkConnection connection, int num) {
        if (num > 1)
        {
            TargetSendHandCard(connection, lootDeck.GetRange(0, num).ToArray());
            lootDeck.RemoveRange(0, num);
        }
        else {
            TargetSendHandCard(connection, lootDeck[0]);
            lootDeck.RemoveAt(0);
        }
    }

    [Server]
    void InitializeGame()
    {

        // Determine which player to start
        foreach (PlayerManager player in playerList)
        {
            if (player.characterId == 2)
            {
                startIndex = playerList.IndexOf(player);
            }

            if (lootDeck.Count > 0)
            {
                SendHandCard(player.netIdentity.connectionToClient, 5);
            }
        }
        if (startIndex == -1)
            startIndex = Random.Range(0, playerList.Count);

        // Rotate all players except the start player so that they can't deal cards.
        foreach (PlayerManager player in playerList)
        {
            if (playerList.IndexOf(player) != startIndex) player.isActivated = true;
        }
        currentPlayerIndex = startIndex;
        playerList[currentPlayerIndex].isSelfTurn = true;

        // TODO: Monster and Treasure Deck Initialization

        Debug.Log("The game will start from Player " + playerList[startIndex].playerName + " and clockwisely process.");
    }

    [Server]
    void CheckGameOver()
    {
        foreach (PlayerManager player in playerList)
        {
            if (player.souls == 4)
            {
                winner = player;
                isGameOver = true;
            }
        }
    }

    [Server]
    void SpawnLootOnServer() {

    }

    [Server]
    void SpawnMonsterOnServer() {

    }

    #endregion

    #region rpc functions

    [TargetRpc]
    void TargetWaitingForOtherPlayers(NetworkConnection connection, int count)
    {
        UIManager.AddBroadCast("Waiting for other " + count + " players.");
    }

    [ClientRpc]
    void RpcAssignLocalPlayerManager()
    {
        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("GamePlayer");
        foreach (GameObject playerObject in playerObjects)
        {
            PlayerManager player = playerObject.GetComponent<PlayerManager>();
            playerList.Add(player);
            if (player.isLocalPlayer) localPlayer = player;
        }
    }

    [ClientRpc]
    void RpcSpawnGamePlayerDisplay()
    {
        foreach (PlayerManager player in playerList)
        {
            player.gamePlayerDisplay = UIManager.SpawnPlayerDisplay(player);
        }
    }

    [TargetRpc]
    void TargetSendCharacterOptions(NetworkConnection connection, int[] options, float selectionTimeLimit)
    {
        UIManager.CharaterSelectDisplay(options, selectionTimeLimit);
    }


    [TargetRpc]
    void TargetSendCharacterOptions(NetworkConnection connection, int[] characterOptions, int[] itemOptions, float selectionTimeLimit)
    {
        UIManager.CharaterSelectDisplay(characterOptions, itemOptions, selectionTimeLimit);
    }

    [ClientRpc]
    void RpcForceSelectCharacter()
    {
        GameObject characterSelection = GameObject.FindGameObjectWithTag("CharacterSelection");
        if (localPlayer.characterId == -1)
        {
            int character = characterSelection.GetComponent<CharacterSelect>().RandomSelectCharacter();
            if (character == 3)
            {
                int eternal = characterSelection.GetComponent<CharacterSelect>().RandomSelectEternal();
                localPlayer.CmdSetupCharacter(character, eternal);
            }
            else {
                CharacterSO characterSO = Resources.Load<CharacterSO>("Objects/Characters/Character_"+character);
                localPlayer.CmdSetupCharacter(character, characterSO.eternal);
            }
        }
        localPlayer.CmdPlayerReady();
        Destroy(characterSelection);
    }

    [ClientRpc]
    void RpcEndSelection()
    {
        GameObject characterSelection = GameObject.FindGameObjectWithTag("CharacterSelection");
        Destroy(characterSelection);
    }

    [TargetRpc]
    void TargetSendHandCard(NetworkConnection connection, int[] cardList)
    {
        localPlayer.CmdDrawCard(cardList);
    }

    [TargetRpc]
    void TargetSendHandCard(NetworkConnection connection, int card) {
        localPlayer.CmdDrawCard(card);
    }

    [TargetRpc]
    void TargetInitializePlayerTurn(NetworkConnection connection)
    {
        localPlayer.isActivated = false;
        localPlayer.isSelfTurn = true;
        localPlayer.dealTimes = localPlayer.availableDeals;
        localPlayer.attackTimes = localPlayer.availableAttack;
    }

    [TargetRpc]
    void TargetEndPlayerTurn(NetworkConnection connection)
    {
        localPlayer.isSelfTurn = false;
    }

    [ClientRpc]
    void RpcDeclareVictory(string winner)
    {
        UIManager.AddBroadCast("The game is over, the winner is " + winner);
    }

    #endregion

    #region game coroutine

    IEnumerator GameProcess()
    {
        if (currentGameStage != null) { StopCoroutine(currentGameStage); currentGameStage = null; }
        currentGameStage = ServerResourcesInitialization();
        yield return StartCoroutine(currentGameStage);
        gameState = GameState.WaitingForPlayers;

        if (currentGameStage != null) { StopCoroutine(currentGameStage); currentGameStage = null; }
        currentGameStage = WaitForAllPlayersToLoadScene();
        yield return StartCoroutine(currentGameStage);
        gameState = GameState.CharacterSelection;

        if (currentGameStage != null) { StopCoroutine(currentGameStage); currentGameStage = null; }
        currentGameStage = CharacterSelectionPhase();
        yield return StartCoroutine(currentGameStage);
        gameState = GameState.InGame;

        if (currentGameStage != null) { StopCoroutine(currentGameStage); currentGameStage = null; }
        currentGameStage = InGame();
        yield return StartCoroutine(currentGameStage);

    }

    IEnumerator ServerResourcesInitialization()
    {
        InitializeLootDeck();
        InitializeCharacterDeck();
        InitializeMonsterDeck();
        InitializeTreasureDeck();
        yield return null;
    }

    IEnumerator WaitForAllPlayersToLoadScene()
    {
        RpcAssignLocalPlayerManager();
        while (!CheckClientReady() && localPlayer == null) Debug.Log("Waiting...");
        yield return null;
    }

    IEnumerator CharacterSelectionPhase()
    {
        GenerateCharacterOptions(characterSelectionDuration);
        RpcSpawnGamePlayerDisplay();
        float countdownTimer = characterSelectionDuration;
        while (countdownTimer > 0)
        {
            if (CheckPlayerReady())
            {
                RpcEndSelection();
                yield break;
            }
            countdownTimer -= Time.deltaTime;
            yield return null;
        }
        RpcForceSelectCharacter();
    }

    IEnumerator InGame()
    {
        InitializeGame();
        while (!isGameOver)
        {
            CheckGameOver();
            yield return StartCoroutine(PlayerTurn(playerList[currentPlayerIndex]));
            currentPlayerIndex = (currentPlayerIndex + 1) % playerList.Count;
        }
    }

    IEnumerator PlayerTurn(PlayerManager player)
    {
        NetworkConnection currentPlayerConnection = player.netIdentity.connectionToClient;
        TargetInitializePlayerTurn(currentPlayerConnection);
        SendHandCard(currentPlayerConnection, 1);
        float roundTimer = roundDuration;
        while (roundTimer > 0)
        {
            if (!player.isSelfTurn)
            {
                yield break;
            }
            roundTimer -= Time.deltaTime;
            yield return null;
        }
        TargetEndPlayerTurn(currentPlayerConnection);
    }
    #endregion

}