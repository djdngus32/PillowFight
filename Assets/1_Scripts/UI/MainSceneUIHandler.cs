using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainSceneUIHandler : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button settingButton;

    private void Awake()
    {
        startButton?.onClick.AddListener(OnClickStartButton);
        quitButton?.onClick.AddListener(OnClickQuitButton);
        settingButton?.onClick.AddListener(OnClickSettingButton);
    }

    private void OnClickStartButton()
    {
        PopupManager.Instance?.OpenPopup(EPopupType.LOGIN);
    }

    private void OnClickQuitButton()
    {
        GameManager.Instance.QuitGame();
    }

    private void OnClickSettingButton()
    {
        PopupManager.Instance?.OpenPopup(EPopupType.GAMESETTING);
    }
}
