using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;
using UnityEngine.AI;
using System.Linq;

/// <summary>
/// ������ ����Ǵ� ���� ���ӵǾ� �������� ������ �ϴ� Ŭ���� (��Ʈ��ũ ������ ����)
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
        }

        //UI�� ų�α� ����
    }

    public Vector3 GetSpawnPoint()
    {
        return new Vector3(0, 1f, 0);
    }
}
