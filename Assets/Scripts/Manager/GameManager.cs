using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{
    List<int> lootDeck = new();
    List<int> characterDeck = new();
    List<int> discardDeck = new();
    public UIManager UIManager;
    public List<PlayerManager> playerList = new List<PlayerManager>();
    public PlayerManager localPlayer;
    public PlayerManager winner;

    public enum GameState
    {
        ServerInitialization,
        WaitingForPlayers,
        CharacterSelection,
        InGame,
        EndGame
    }

    int startIndex = -1;

    [SyncVar(hook = nameof(OnCurrentPlayerChanged))]
    public int currentPlayerIndex;

    [SyncVar(hook = nameof(OnGameStateChanged))]
    public GameState gameState = GameState.ServerInitialization;

    [SyncVar(hook = nameof(OnEndGameChanged))]
    public bool isGameOver = false;

    public float characterSelectionDuration = 15f;
    public float roundDuration = 60f;

    [Server]
    public override void OnStartServer()
    {
        StartCoroutine(GameProcess());
    }

    #region hooks

    public void OnCurrentPlayerChanged(int oldPlayer, int newPlayer)
    {
        Debug.Log("Now is " + playerList[currentPlayerIndex].playerName + "'s turn.");
        playerList[currentPlayerIndex].isSelfTurn = true;
    }

    public void OnGameStateChanged(GameState oldState, GameState newState)
    {
        Debug.Log("Now the game state is " + this.gameState);
    }

    public void OnEndGameChanged(bool oldStatus, bool newStatus)
    {
        DeclareVictory(winner.playerName);
    }

    #endregion

    #region server only

    [Server]
    void InitializeLootDeck()
    {
        LootSO[] lootResources = Resources.LoadAll<LootSO>("Objects/Loots");
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
        CharacterSO[] characterResources = Resources.LoadAll<CharacterSO>("Objects/Characters");
        for (int i = 0; i < characterResources.Length; i++)
        {
            characterDeck.Add(characterResources[i].characterId);
        }
        ShuffleDeck(characterDeck);
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
                WaitingForOtherPlayers(connection, count);
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
            int[] options = characterDeck.GetRange(i, 3).ToArray();
            TargetSendCharacterOptions(connection, options, selectionTimeLimit);
            i += 3;
        }
    }

    [Server]
    bool CheckPlayerCharacter()
    {
        foreach (PlayerManager player in playerList)
        {
            if (player.characterId == -1) return false;
        }
        return true;
    }

    [Server]
    void InitializeGame()
    {

        foreach (PlayerManager player in playerList)
        {
            if (player.characterId == 2)
            {
                startIndex = playerList.IndexOf(player);
            }

            if (lootDeck.Count > 0)
            {
                SendHandCard(player.netIdentity.connectionToClient, lootDeck.GetRange(0, 5).ToArray());
                lootDeck.RemoveRange(0, 5);
            }
        }
        if (startIndex == -1)
            startIndex = Random.Range(0, playerList.Count);

        currentPlayerIndex = startIndex;
        playerList[currentPlayerIndex].isSelfTurn = true;

        Debug.Log("The game will start from Player " + playerList[startIndex].playerName + " and clockwisely process.");
    }

    [Server]
    bool CheckTurnEnd(PlayerManager player)
    {
        return player.isSelfTurn;
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

    #endregion

    #region rpc functions

    [TargetRpc]
    void WaitingForOtherPlayers(NetworkConnection connection, int count)
    {
        Debug.Log("Waiting for other " + count + " players.");
    }

    [ClientRpc]
    void AssignLocalPlayerManager()
    {
        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("GamePlayer");
        foreach (GameObject playerObject in playerObjects)
        {
            PlayerManager player = playerObject.GetComponent<PlayerManager>();
            playerList.Add(player);
            if (player.isLocalPlayer) localPlayer = player;
        }
    }

    [TargetRpc]
    void TargetSendCharacterOptions(NetworkConnection connection, int[] options, float selectionTimeLimit)
    {
        Debug.Log(localPlayer.playerName+"'s character options are: " +string.Join(",", options));
        UIManager.CharaterSelectDisplay(options, selectionTimeLimit);
    }

    [ClientRpc]
    void ForceSelectCharacter()
    {
        GameObject characterSelection = GameObject.FindGameObjectWithTag("CharacterSelection");
        if (localPlayer.characterId == -1)
        {
            localPlayer.CmdSetupCharacter(characterSelection.GetComponent<CharacterSelect>().RandomSelect());
        }

        Destroy(characterSelection);
    }

    [ClientRpc]
    void EndSelection()
    {
        GameObject characterSelection = GameObject.FindGameObjectWithTag("CharacterSelection");
        Destroy(characterSelection);
    }

    [TargetRpc]
    void SendHandCard(NetworkConnection connection, int[] cardList)
    {
        localPlayer.DrawCard(cardList);
    }

    [ClientRpc]
    void DeclareVictory(string winner)
    {
        Debug.Log("The game is over, the winner is " + winner);
    }

    #endregion

    #region game coroutine

    IEnumerator GameProcess()
    {
        yield return StartCoroutine(ServerResourcesInitialization());
        gameState = GameState.WaitingForPlayers;
        yield return StartCoroutine(WaitForAllPlayersToLoadScene());
        gameState = GameState.CharacterSelection;
        yield return StartCoroutine(CharacterSelectionPhase());
        gameState = GameState.InGame;
        yield return StartCoroutine(StartGame());
    }

    IEnumerator ServerResourcesInitialization()
    {
        InitializeLootDeck();
        InitializeCharacterDeck();
        yield return null;
    }

    IEnumerator WaitForAllPlayersToLoadScene()
    {
        AssignLocalPlayerManager();
        while (!CheckClientReady() && localPlayer == null) Debug.Log("Waiting...");
        yield return null;
    }

    IEnumerator CharacterSelectionPhase()
    {
        GenerateCharacterOptions(characterSelectionDuration);
        float countdownTimer = characterSelectionDuration;
        while (countdownTimer > 0)
        {
            if (CheckPlayerCharacter())
            {
                EndSelection();
                yield break;
            }
            countdownTimer -= Time.deltaTime;
            yield return null;
        }
        ForceSelectCharacter();
    }

    IEnumerator StartGame()
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
        float roundTimer = roundDuration;
        while (roundTimer > 0)
        {
            if (!CheckTurnEnd(player))
            {
                yield break;
            }
            roundTimer -= Time.deltaTime;
            yield return null;
        }
        player.isSelfTurn = false;
    }
    #endregion

}