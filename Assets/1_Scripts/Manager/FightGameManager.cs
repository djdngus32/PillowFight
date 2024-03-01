using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Fusion;
using System.Linq;


/// <summary>
/// 게임이 진행되는 씬에 종속되어 전반적인 관리를 하는 클래스 (네트워크 관리도 포함)
/// </summary>
public class FightGameManager : NetworkBehaviour
{
    public static FightGameManager Instance { get; private set; }

    private bool isSentPlayerNickname = false;
    private List<Transform> spawnPointList = new List<Transform>();
    private GameUIHandler gameUI;

    [Networked, Capacity(16)]
    public NetworkDictionary<PlayerRef, PlayerData> PlayerData { get; }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        gameUI = FindObjectOfType<GameUIHandler>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(GameManager.Instance.IsPause)
            {
                GameManager.Instance.ResumeGame();
            }
            else
            {
                GameManager.Instance.PauseGame();
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            PopupManager.Instance.OpenPopup(EPopupType.SCOREBOARD);
        }

        if (Input.GetKeyUp(KeyCode.Tab))
        {
            PopupManager.Instance.ClosePopup(EPopupType.SCOREBOARD);
        }
    }

    public override void Spawned()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        InitSpawnPoints();

        PlayerManager.Instance.SpawnPlayer(Runner, GetSpawnPoint());

        if (UIManager.Instance.LoadingUI.IsOpen)
        {
            UIManager.Instance.LoadingUI.Close();
        }
    }

    public override void FixedUpdateNetwork()
    {
        UpdatePlayerData();
    }

    public override void Render()
    {
        if(isSentPlayerNickname == false)
        {
            RPC_SetPlayerNickname(Runner.LocalPlayer, GameDataManager.Instance.LoadDataToLocal(GlobalString.DATA_KEY_PLAYER_NICKNAME, Runner.LocalPlayer.ToString()));
            isSentPlayerNickname = true;
        }
    }

    private void InitSpawnPoints()
    {
        GameObject spawnPoints = GameObject.Find("Spawn Points");
        if(spawnPoints != null && spawnPoints.transform.childCount > 0)
        {
            spawnPointList.Clear();
            foreach(Transform spawnPoint in spawnPoints.transform)
            {
                spawnPointList.Add(spawnPoint);
            }
        }
    }

    private void UpdatePlayerData()
    {
        List<PlayerRef> tempPlayers = Runner.ActivePlayers.ToList();

        foreach (PlayerRef player in tempPlayers)
        {
            if(!PlayerData.ContainsKey(player))
            {
                PlayerData playerData = new PlayerData();
                playerData.Nickname = player.ToString();
                playerData.OwnerPlayerPrefs = player;

                PlayerData.Set(player, playerData);
            }
        }

        foreach(var pair in PlayerData)
        {
            if(!tempPlayers.Contains(pair.Key))
            {
                PlayerData.Remove(pair.Key);
            }
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_OnKilledPlayer(PlayerRef killerPlayerRef, PlayerRef killedPlayerRef)
    {
        string killerPlayerNickname = string.Empty;
        string killedPlayerNickname = string.Empty;

        if (PlayerData.TryGet(killerPlayerRef, out PlayerData killerPlayerData))
        {
            killerPlayerNickname = killerPlayerData.Nickname;
            if (Object.HasStateAuthority)
            {
                killerPlayerData.KillCount++;
                PlayerData.Set(killerPlayerRef, killerPlayerData);
            }
        }

        if (PlayerData.TryGet(killedPlayerRef, out PlayerData killedPlayerData))
        {
            killedPlayerNickname = killedPlayerData.Nickname;
            if (Object.HasStateAuthority)
            {
                killedPlayerData.DeathCount++;
                PlayerData.Set(killedPlayerRef, killedPlayerData);
            }
        }

        //UI에 킬로그 띄우기
        if(gameUI != null)
        {
            gameUI.AddKillLog(killerPlayerNickname, killedPlayerNickname);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority, Channel = RpcChannel.Reliable)]
    private void RPC_SetPlayerNickname(PlayerRef sendPlayer, string nickname)
    {
        var playerData = PlayerData.Get(sendPlayer);
        playerData.Nickname = nickname;
        PlayerData.Set(sendPlayer, playerData);
    }

    public Transform GetSpawnPoint()
    {
        if (spawnPointList == null || spawnPointList.Count == 0)
            return null;

        int rand = Random.Range(0, spawnPointList.Count);

        return spawnPointList[rand];
    }
}
