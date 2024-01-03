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
    /// Fusion ������ �����ϴ� �Լ�
    /// </summary>
    public void JoinOrCreateSession()
    {
        StartCoroutine(CoConnectFusionServer());
    }

    /// <summary>
    /// Fusion �������� ������ �����ϴ� �Լ�
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
