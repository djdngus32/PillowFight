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

    [Networked, Capacity(16)]
    public NetworkDictionary<PlayerRef, PlayerData> PlayerData { get; }

    private UnityEvent<List<PlayerData>> OnPlayerDataChanged = new UnityEvent<List<PlayerData>>();

    private List<PlayerData> PlayerDataOrderByKillCount => PlayerData.Select(pair => pair.Value).OrderByDescending(data => data.KillCount).ToList();

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    public override void Spawned()
    {
        var scoreBoardPopup = PopupManager.Instance.GetPopup(EPopupType.SCOREBOARD) as ScoreBoardPopupUI;
        if (scoreBoardPopup != null)
        {
            OnPlayerDataChanged.AddListener(scoreBoardPopup.UpdateScoreBoard);
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
                RPC_OnChangedPlayerData();
            }
        }

        foreach(var pair in PlayerData)
        {
            if(!tempPlayers.Contains(pair.Key))
            {
                PlayerData.Remove(pair.Key);
                RPC_OnChangedPlayerData();
            }
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_OnKilledPlayer(PlayerRef killerPlayerRef, PlayerRef killedPlayerRef)
    {
        //플레이어 데이터에 접근 가능한 것은 매니저 클래스의 권한이 있는 사람 == 방장 뿐이여야 한다.
        if(Object.HasStateAuthority)
        {
            if(PlayerData.TryGet(killerPlayerRef, out PlayerData killerPlayerData))
            {
                killerPlayerData.KillCount++;
                PlayerData.Set(killerPlayerRef, killerPlayerData);
            }

            if(PlayerData.TryGet(killedPlayerRef, out PlayerData killedPlayerData))
            {
                killedPlayerData.DeathCount++;
                PlayerData.Set(killedPlayerRef, killedPlayerData);                
            }

            RPC_OnChangedPlayerData();
        }
        
        //UI에 킬로그 띄우기
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_OnChangedPlayerData()
    {
        OnPlayerDataChanged.Invoke(PlayerDataOrderByKillCount);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority, Channel = RpcChannel.Reliable)]
    private void RPC_SetPlayerNickname(PlayerRef sendPlayer, string nickname)
    {
        var playerData = PlayerData.Get(sendPlayer);
        playerData.Nickname = nickname;
        PlayerData.Set(sendPlayer, playerData);

        RPC_OnChangedPlayerData();
    }

    public Vector3 GetSpawnPoint()
    {
        return new Vector3(0, 1f, 0);
    }
}
