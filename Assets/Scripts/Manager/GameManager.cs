using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{
    List<int> lootDeck = new();
    List<int> characterDeck = new();
    int startIndex;
    public UIManager UIManager;
    public List<PlayerManager> playerList = new List<PlayerManager>();
    public PlayerManager localPlayer;

    public enum GameState
    {
        ServerInitialization,
        WaitingForPlayers,
        CharacterSelection,
        InGame
    }

    [SyncVar(hook = nameof(OnGameStateChanged))]
    public GameState gameState = GameState.ServerInitialization;

    public float characterSelectionDuration = 15f;

    [Server]
    public override void OnStartServer()
    {
        StartCoroutine(GameProcess());
    }

    public void OnGameStateChanged(GameState oldState, GameState newState)
    {
        Debug.Log("Now the game state is " + this.gameState);
    }

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
    bool CheckClientReady()
    {
        foreach (NetworkConnectionToClient connection in NetworkServer.connections.Values)
        {
            if (!connection.isReady) return false;
        }
        return true;
    }

    [Server]
    bool CheckPlayerCharacter() {
        foreach (PlayerManager player in playerList) {
            if (player.characterId == -1) return false;
        }
        return true;
    }

    [TargetRpc]
    void TargetSendCharacterOptions(NetworkConnection connection, int[] options, float selectionTimeLimit)
    {
        UIManager.CharaterSelectDisplay(options, selectionTimeLimit);
    }

    [ClientRpc]
    void ForceSelectCharacter()
    {
        GameObject characterSelection = GameObject.FindGameObjectWithTag("CharacterSelection");
        if (localPlayer.characterId == -1) {
            localPlayer.CmdSetupCharacter(characterSelection.GetComponent<CharacterSelect>().RandomSelect());
        }

        Destroy(characterSelection);
    }

    [ClientRpc]
    void EndSelection() {
        GameObject characterSelection = GameObject.FindGameObjectWithTag("CharacterSelection");
        Destroy(characterSelection);
    }

    [ClientRpc]
    void AssignLocalPlayerManager()
    {
        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("GamePlayer");
        foreach (GameObject playerObject in playerObjects) {
            PlayerManager player = playerObject.GetComponent<PlayerManager>();
            playerList.Add(player);
            if (player.isLocalPlayer) localPlayer = player;
        }
    }

    [ClientRpc]
    void InitializeGame()
    {
        foreach (PlayerManager player in playerList) {
            if (player.characterId == 2) startIndex = playerList.IndexOf(player);
        }
    }

    IEnumerator GameProcess()
    {
        yield return StartCoroutine(ServerResourcesInitialization());
        gameState = GameState.WaitingForPlayers;
        yield return StartCoroutine(WaitForAllPlayersToLoadScene());
        gameState = GameState.CharacterSelection;
        yield return StartCoroutine(CharacterSelectionPhase());
        gameState = GameState.InGame;
        yield return StartCoroutine(StartPlayerTurn());
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
    }

    IEnumerator StartPlayerTurn()
    {

        yield return null;
    }
}
