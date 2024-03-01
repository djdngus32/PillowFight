using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBoardPopupUI : PopupUI
{
    public override EPopupType PopupType => EPopupType.SCOREBOARD;

    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private Button closeButton;
    [SerializeField] private GameObject itemPrefab;

    private List<ScoreBoardPopupItemUI> items = new List<ScoreBoardPopupItemUI>();
    private List<PlayerData> players = new List<PlayerData>();

    private void Start()
    {
        closeButton.onClick.AddListener(Close);
    }

    private void OnEnable()
    {
        InvokeRepeating(nameof(UpdateScoreBoard), 0f, 0.5f);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    public void UpdateScoreBoard()
    {
        if (FightGameManager.Instance == null)
            return;

        players.Clear();

        foreach(var pair in FightGameManager.Instance.PlayerData)
        {
            players.Add(pair.Value);
        }

        players.Sort((a, b) => (b.KillCount - a.KillCount));

        if (players.Count > items.Count)
        {
            int createCount = players.Count - items.Count;
            for (int i = 0; i < createCount; i++)
            {
                ScoreBoardPopupItemUI item = Instantiate(itemPrefab, scrollRect.content).GetComponent<ScoreBoardPopupItemUI>();

                items.Add(item);
            }
        }

        for (int i = 0; i < items.Count; i++)
        {
            var item = items[i];

            if (i < players.Count)
            {
                var data = players[i];
                item.ChangeItemText(data.Nickname, data.KillCount, data.DeathCount);
                if (item.gameObject.activeSelf == false)
                {
                    item.gameObject.SetActive(true);
                }
            }
            else
            {
                if (item.gameObject.activeSelf)
                {
                    item.gameObject.SetActive(false);
                }
            }
        }
    }
}
