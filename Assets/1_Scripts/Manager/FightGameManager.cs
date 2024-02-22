using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;
using UnityEngine.AI;
using System.Linq;

/// <summary>
/// 게임이 진행되는 씬에 종속되어 전반적인 관리를 하는 클래스 (네트워크 관리도 포함)
/// </summary>
public class FightGameManager : NetworkBehaviour
{
    public static FightGameManager Instance { get; private set; }

    [Networked, Capacity(16)]
    public NetworkDictionary<PlayerRef, PlayerData> PlayerData { get; }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
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
        }

        //UI에 킬로그 띄우기
    }

    public Vector3 GetSpawnPoint()
    {
        return new Vector3(0, 1f, 0);
    }
}
