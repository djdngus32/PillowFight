using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

/// <summary>
/// ������ ����Ǵ� ���� ���ӵǾ� �������� ������ �ϴ� Ŭ���� (��Ʈ��ũ ������ ����)
/// </summary>
public class FightGameManager : NetworkBehaviour
{
    [SerializeField] private float playerRespawnTime = 5f;

    [Networked] public EGameState GameState { get; set; }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_OnKilledPlayer(PlayerRef killerPlayerRef, PlayerRef killedPlayerRef)
    {

    }
}
