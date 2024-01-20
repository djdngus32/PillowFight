using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    private PlayerData playerData;

    //Player Events
    public Action<int> onChangedHP;

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
}
