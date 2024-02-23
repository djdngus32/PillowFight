using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreBoardPopupItemUI : MonoBehaviour
{
    [SerializeField] private TMP_Text nicknameText;
    [SerializeField] private TMP_Text killCountText;
    [SerializeField] private TMP_Text deathCountText;

    private const string KILL_COUNT_TEXT_PREFIX = "Kill : ";
    private const string DEATH_COUNT_TEXT_PREFIX = "Death : ";

    public void ChangeItemText(string nickname, int killCount, int deathCount)
    {
        nicknameText.text = nickname;
        killCountText.text = KILL_COUNT_TEXT_PREFIX + killCount.ToString();
        deathCountText.text = DEATH_COUNT_TEXT_PREFIX + deathCount.ToString();
    }
}
