using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 각 씬에 필요한 팝업들을 가지고 있는 객체 (씬마다 존재한다)
/// </summary>
public class PopupContainer : MonoBehaviour
{
    Dictionary<EPopupType, PopupUI> popupUI = new Dictionary<EPopupType, PopupUI> ();

    private void Start()
    {
        var popups = GetComponentsInChildren<PopupUI>(true);

        foreach (var popup in popups)
        {
            if(popupUI.TryAdd(popup.PopupType, popup))
            {
                popup.gameObject.SetActive(false);
            }
        }

        PopupManager.Instance?.SetPopupContainer(this);
    }

    public PopupUI GetPopup(EPopupType type)
    {
        popupUI.TryGetValue(type, out var popup);

        return popup;
    }
}
