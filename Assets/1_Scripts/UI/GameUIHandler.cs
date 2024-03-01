using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameUIHandler : MonoBehaviour
{
    [Header("Component")]
    [SerializeField] private KillLogUIHandler killLogUI;

    [Header("Player")]
    [SerializeField] private TMP_Text playerHPText;


    private void OnEnable()
    {
        PlayerManager.Instance.onChangedHP += OnChangedPlayerHP;
    }

    private void OnDisable()
    {
        PlayerManager.Instance.onChangedHP -= OnChangedPlayerHP;
    }

    public void AddKillLog(string killerPlayerNickname, string killedPlayerNickname)
    {
        killLogUI.ShowKillLog(killerPlayerNickname, killedPlayerNickname);
    }

    private void OnChangedPlayerHP(int playerHP)
    {
        playerHPText.text = playerHP.ToString();
    }
}
