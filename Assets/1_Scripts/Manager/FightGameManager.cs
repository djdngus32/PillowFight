using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

/// <summary>
/// 게임이 진행되는 씬에 종속되어 전반적인 관리를 하는 클래스 (네트워크 관리도 포함)
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
