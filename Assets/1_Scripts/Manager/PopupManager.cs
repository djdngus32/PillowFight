using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance { get; private set; }

    private PopupContainer popupContainer;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetPopupContainer(PopupContainer container)
    {
        if(container != null)
        {
            popupContainer = container;
        }
    }

    public void OpenPopup(EPopupType type)
    {
        if(popupContainer != null)
        {
            PopupUI popup = popupContainer.GetPopup(type);
            if (popup == null)
            {
                Debug.LogError($"PopupManager::This Container does not have Type[{type}] Popup");
                return;
            }
            popup.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("PopupManager::Container is Null!!");
        }
    }

    public void ClosePopup(EPopupType type)
    {

    }
}
