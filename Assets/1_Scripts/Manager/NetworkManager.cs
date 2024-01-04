using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;
using Unity.VisualScripting;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance { get; private set; }

    [SerializeField] private NetworkRunner networkRunnerPrefab;
    [SerializeField] private GameObject playerPrefab;

    private NetworkRunner runner;

    private readonly string SCENE_NAME_FIELD = "1_FieldScene";

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
        var op = SceneManager.LoadSceneAsync(SCENE_NAME_FIELD, LoadSceneMode.Single);

        while(op.isDone == false)
        {
            yield return null;
        }

        runner = Instantiate(networkRunnerPrefab, transform);
        runner.name = networkRunnerPrefab.name;

        StartGameArgs startGameArgs = new StartGameArgs();

        startGameArgs.GameMode = GameMode.Shared;
        startGameArgs.SessionName = "";
        startGameArgs.PlayerCount = 20;

        yield return runner.StartGame(startGameArgs);

        yield return new WaitUntil(() => runner.IsConnectedToServer == true);

        runner.Spawn(playerPrefab, new Vector3(0, 1, 0), Quaternion.identity, runner.LocalPlayer);

        yield return null;
    }
}
