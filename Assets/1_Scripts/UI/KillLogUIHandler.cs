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

    //킬로그 오브젝트는 오브젝트 풀처럼
    //사용가능한 오브젝트가 없을 때 여유분을 생성할 필요가 없기때문에
    //최대 갯수에서 계속 돌려서 사용하도록 한다.
    private KillLogItemUIHandler Get()
    {
        latestActiveItemIndex = (latestActiveItemIndex + 1) % DEFAULT_ITEM_COUNT;
        return killLogItems[latestActiveItemIndex];
    }
}
