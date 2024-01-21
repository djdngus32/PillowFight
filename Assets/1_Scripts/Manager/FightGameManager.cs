using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

/// <summary>
/// ������ ����Ǵ� ���� ���ӵǾ� �������� ������ �ϴ� Ŭ���� (��Ʈ��ũ ������ ����)
/// </summary>
public class FightGameManager : NetworkBehaviour
{
    public static FightGameManager Instance { get; private set; }

    [SerializeField] private float playerRespawnTime = 5f;

    [Networked] private TickTimer RespawnTimer { get; set; }
    [Networked] public EGameState GameState { get; set; }

    private void Awake()
    {
        Instance = this;
    }

    public override void Spawned()
    {
        RespawnTimer = TickTimer.None;
    }

    public override void FixedUpdateNetwork()
    {
        UpdatePlayer();
    }

    private void UpdatePlayer()
    {
        if (PlayerManager.Instance.Controller == null)
        {
            RespawnPlayer();
        }
    }

    public void RespawnPlayer()
    {
        if (RespawnTimer.ExpiredOrNotRunning(Runner) == false)
            return;

        PlayerManager.Instance.SpawnPlayer(Runner);
    }

    public void OnKilled(PlayerRef killerPlayerRef)
    {
        PlayerManager.Instance.DespawnPlayer(Runner);
        RespawnTimer =  TickTimer.CreateFromSeconds(Runner, playerRespawnTime);
    }


    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_OnKilledPlayer(PlayerRef killerPlayerRef, PlayerRef killedPlayerRef)
    {

    }

    public Vector3 GetSpawnPoint()
    {
        return new Vector3(0, 1f, 0);
    }
}
