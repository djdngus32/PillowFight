using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    [SerializeField] private GameObject playerPrefab;

    private PlayerData playerData;

    //Player Events
    public Action<int> onChangedHP;

    public PlayerController Controller { get; set; }

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetPlayerData(PlayerData data)
    { 
        if (data == null)
            return;

        playerData = data;
    }

    public void SpawnPlayer(NetworkRunner runner)
    {
        Controller = runner.Spawn(playerPrefab, new Vector3(0, 1, 0), Quaternion.identity, runner.LocalPlayer).GetComponent<PlayerController>();
    }

    public void DespawnPlayer(NetworkRunner runner)
    {
        runner.Despawn(Controller.Object);
    }
}
