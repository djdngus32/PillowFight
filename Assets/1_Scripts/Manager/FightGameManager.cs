using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Fusion;
using System.Linq;


/// <summary>
/// ������ ����Ǵ� ���� ���ӵǾ� �������� ������ �ϴ� Ŭ���� (��Ʈ��ũ ������ ����)
/// </summary>
public class FightGameManager : NetworkBehaviour
{
    public static FightGameManager Instance { get; private set; }

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
        //�÷��̾� �����Ϳ� ���� ������ ���� �Ŵ��� Ŭ������ ������ �ִ� ��� == ���� ���̿��� �Ѵ�.
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
        
        //UI�� ų�α� ����
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_OnChangedPlayerData()
    {
        OnPlayerDataChanged.Invoke(PlayerDataOrderByKillCount);
    }

    public Vector3 GetSpawnPoint()
    {
        return new Vector3(0, 1f, 0);
    }
}
