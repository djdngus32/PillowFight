using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;

//게임 시작시 딱 한번 서버에 연결을 담당하는 클래스
public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance { get; private set; }

    [SerializeField] private NetworkRunner networkRunnerPrefab;

    private NetworkRunner runner;

    private readonly string SCENE_NAME_FIELD = "1_FillowFightScene";

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
        if (runner == null || runner.IsRunning == false)
            return;

        runner.Shutdown();
        runner = null;
    }

    private IEnumerator CoConnectFusionServer()
    {
        //세션과의 연결을 직접적으로 담당할 Runner 객체를 생성.
        runner = Instantiate(networkRunnerPrefab);
        runner.name = networkRunnerPrefab.name;

        //생성, 진입할 세션의 정보를 할당
        StartGameArgs startGameArgs = new StartGameArgs();

        startGameArgs.GameMode = GameMode.Shared;
        startGameArgs.SessionName = "";
        startGameArgs.PlayerCount = 20;

        UIManager.Instance.LoadingUI.Open();

        yield return runner.StartGame(startGameArgs);

        while(runner.IsRunning == false)
        {
            yield return null;
        }

        if (runner.IsSceneAuthority)
        {            
            var op = runner.LoadScene(SCENE_NAME_FIELD, LoadSceneMode.Single, LocalPhysicsMode.None, true);

            while (op.IsDone == false)
            {
                yield return null;
            }
        }

        yield return new WaitForSeconds(1);

        PlayerManager.Instance.SetRunner(runner);

        yield return null;
    }
}
