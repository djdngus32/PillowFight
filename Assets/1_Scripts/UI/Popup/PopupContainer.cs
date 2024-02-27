using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �� ���� �ʿ��� �˾����� ������ �ִ� ��ü (������ �����Ѵ�)
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
