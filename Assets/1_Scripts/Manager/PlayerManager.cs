using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    [SerializeField] private GameObject playerPrefab;

    //Player Events
    public Action<int> onChangedHP;

    public PlayerController Controller { get; set; }

    private NetworkRunner Runner { get; set; }

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetRunner(NetworkRunner runner)
    {
        Runner = runner;
    }

    public IEnumerator CoRespawnPlayer(float delay)
    {
        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        if (Runner == null)
            yield break;

        if (Controller != null)
            DespawnPlayer(Runner);

        SpawnPlayer(Runner);

        yield return null;
    }

    public void SpawnPlayer(NetworkRunner runner)
    {
        runner.Spawn(playerPrefab, new Vector3(0, 1, 0), Quaternion.identity, runner.LocalPlayer);
    }

    public void DespawnPlayer(NetworkRunner runner)
    {
        runner.Despawn(Controller.Object);
        Controller = null;
    }
}
