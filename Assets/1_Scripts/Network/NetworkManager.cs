using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance { get; private set; }

    [SerializeField] private NetworkRunner networkRunnerPrefab;

    private NetworkRunner runner;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Fusion 서버에 연결하는 함수
    /// </summary>
    public void JoinOrCreateSession()
    {
        StartCoroutine(CoConnectFusionServer());
    }

    /// <summary>
    /// Fusion 서버와의 연결을 종료하는 함수
    /// </summary>
    public void DisConnect()
    {
        runner.Shutdown();
        runner = null;
    }

    private IEnumerator CoConnectFusionServer()
    {
        runner = Instantiate(networkRunnerPrefab);
        runner.name = networkRunnerPrefab.name;

        StartGameArgs startGameArgs = new StartGameArgs();

        startGameArgs.GameMode = GameMode.Shared;
        startGameArgs.SessionName = "";
        startGameArgs.PlayerCount = 20;

        yield return runner.StartGame(startGameArgs);

        yield return null;
    }
}
