using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �� ���� �ʿ��� �˾����� ������ �ִ� ��ü (������ �����Ѵ�)
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
