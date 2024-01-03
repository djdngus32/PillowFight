using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 각 씬에 필요한 팝업들을 가지고 있는 객체 (씬마다 존재한다)
/// </summary>
public class PopupContainer : MonoBehaviour
{
    [SerializeField] private List<PopupUI> popup;

    private void Start()
    {
        PopupManager.Instance?.SetPopupContainer(this);
    }

    public PopupUI GetPopup(EPopupType type)
    {
        PopupUI result = null;

        foreach(var popupUI in popup)
        {
            if(popupUI.PopupType == type)
            {
                result = popupUI;
                break;
            }
        }

        return result;
    }
}
