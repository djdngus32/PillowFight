using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KillLogUIHandler : MonoBehaviour
{
    [SerializeField] private GameObject killLogItemUIPrefab;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private float logDurationTime = 5f;

    private List<KillLogItemUIHandler> killLogItems = new List<KillLogItemUIHandler>();
    private int latestActiveItemIndex;

    private readonly int DEFAULT_ITEM_COUNT = 5;

    private void Start()
    {
        for (int i = 0; i < DEFAULT_ITEM_COUNT; i++)
        {
            var item = Instantiate(killLogItemUIPrefab, scrollRect.content).GetComponent<KillLogItemUIHandler>();
            if (item != null)
            {
                item.Hide();
                killLogItems.Add(item);
            }
        }
    }

    public void ShowKillLog(string killerPlayerNickname, string killedPlayerNickname)
    {
        var item = Get();
        item.Show(killerPlayerNickname, killedPlayerNickname, logDurationTime);
        item.RectTransform.SetAsLastSibling();
    }

    //ų�α� ������Ʈ�� ������Ʈ Ǯó��
    //��밡���� ������Ʈ�� ���� �� �������� ������ �ʿ䰡 ���⶧����
    //�ִ� �������� ��� ������ ����ϵ��� �Ѵ�.
    private KillLogItemUIHandler Get()
    {
        latestActiveItemIndex = (latestActiveItemIndex + 1) % DEFAULT_ITEM_COUNT;
        return killLogItems[latestActiveItemIndex];
    }
}
