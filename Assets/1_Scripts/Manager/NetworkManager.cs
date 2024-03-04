using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;

//���� ���۽� �� �ѹ� ������ ������ ����ϴ� Ŭ����
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
        if (runner == null || runner.IsRunning == false)
            return;

        runner.Shutdown();
        runner = null;
    }

    private IEnumerator CoConnectFusionServer()
    {
        //���ǰ��� ������ ���������� ����� Runner ��ü�� ����.
        runner = Instantiate(networkRunnerPrefab);
        runner.name = networkRunnerPrefab.name;

        //����, ������ ������ ������ �Ҵ�
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
