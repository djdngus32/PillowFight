using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBoardPopupUI : PopupUI
{
    public override EPopupType PopupType => EPopupType.SCOREBOARD;

    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private Button closeButton;
    [SerializeField] private GameObject itemPrefab;

    private List<ScoreBoardPopupItemUI> items = new List<ScoreBoardPopupItemUI>();

    private void Start()
    {
        closeButton.onClick.AddListener(Close);
    }

    public void UpdateScoreBoard(List<PlayerData> playerData)
    {
        if (playerData == null || playerData.Count == 0)
        {
            foreach (var item in items)
            {
                item.gameObject.SetActive(false);
            }
            return;
        }            

        if (playerData.Count > items.Count)
        {
            int createCount = playerData.Count - items.Count;
            for (int i = 0; i < createCount; i++)
            {
                ScoreBoardPopupItemUI item = Instantiate(itemPrefab, scrollRect.content).GetComponent<ScoreBoardPopupItemUI>();

                items.Add(item);
            }
        }

        for (int i = 0; i < items.Count; i++)
        {
            var item = items[i];

            if (i < playerData.Count)
            {
                var data = playerData[i];
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
