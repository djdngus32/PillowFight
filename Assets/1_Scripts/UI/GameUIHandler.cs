using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUIHandler : MonoBehaviour
{
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

    private void OnChangedPlayerHP(int playerHP)
    {
        playerHPText.text = playerHP.ToString();
    }
}
