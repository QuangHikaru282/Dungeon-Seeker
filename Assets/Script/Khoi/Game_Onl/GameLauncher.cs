using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fusion;
using Fusion.Sockets;
using UnityEngine.SceneManagement;

public class GameLauncher : MonoBehaviour, INetworkRunnerCallbacks
{
    public TMP_InputField playerNameInput;
    public Button playGameButton;

    public NetworkRunner networkRunner;

    public NetworkPrefabRef playerPrefab;
    public NetworkPrefabRef coinPrefab;
    public NetworkPrefabRef enemyPrefab;
    void Start()
    {
        playGameButton.onClick.AddListener(ConnectToGame);
    }
    void ConnectToGame()
    {
        var playerName = string.IsNullOrEmpty(playerNameInput.text) ?
            "Player" + Random.Range(1000, 100000) : playerNameInput.text;

        PlayerPrefs.SetString("PlayerName", playerName);

        playerNameInput.interactable = false;
        playGameButton.interactable = false;

        // ❗ Quan trọng: GỌI StartGame
        StartGame(GameMode.Shared);     // Nếu bạn muốn tạo phòng
                                      // StartGame(GameMode.Client); // Nếu bạn muốn join phòng
    }

    async void StartGame(GameMode mode)
    {
        networkRunner = gameObject.AddComponent<NetworkRunner>();
        
        networkRunner.ProvideInput = true;

        var playerSpawner = gameObject.AddComponent<PlayerSpawner>();
        playerSpawner.playerPrefab = playerPrefab;

        var coinSpawner = gameObject.AddComponent<CoinSpawner>();
        coinSpawner.coinPrefab = coinPrefab;

        var enemySpawner = gameObject.AddComponent<CoinSpawner>();
        enemySpawner.coinPrefab = enemyPrefab;

        //networkRunner.AddCallbacks(this);
        var scene = SceneRef.FromIndex(1);
        var sceneInfo = new NetworkSceneInfo();
        if (scene.IsValid)
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Single);


        var startGameArgs = new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "TestRoom",
            Scene = sceneInfo,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        };
        await networkRunner.StartGame(startGameArgs);
    }    

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        Debug.Log($"Player Joined: {player}");
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        Debug.Log($"Player Joined: {player}");
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"Player Joined: {player}");
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"Player Joined: {player}");
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {

    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
 
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {

    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {

    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {

    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, System.ArraySegment<byte> data)
    {
        Debug.Log($"Player Joined: {player}");
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
        Debug.Log($"Player Joined: {player}");
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {

    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        Debug.Log($"Player Joined: {player}");
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {

    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {

    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {

    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {

    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {

    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {

    }
}
