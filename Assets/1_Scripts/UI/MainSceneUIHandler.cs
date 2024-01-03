using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainSceneUIHandler : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        startButton?.onClick.AddListener(OnClickStartButton);
        quitButton?.onClick.AddListener(OnClickQuitButton);
    }

    private void OnClickStartButton()
    {
        PopupManager.Instance?.OpenPopup(EPopupType.LOGIN);
    }

    private void OnClickQuitButton()
    {

    }
}
