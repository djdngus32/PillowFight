using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseUIHandler : MonoBehaviour
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button settingButton;

    public bool IsOpen => gameObject.activeSelf;

    private void Start()
    {
        resumeButton?.onClick.AddListener(OnClickResumeButton);
        quitButton?.onClick.AddListener(OnClickQuitButton);
        settingButton?.onClick.AddListener(OnClickSettingButton);
    }

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        PopupManager.Instance.ClosePopup(EPopupType.GAMESETTING);
        gameObject.SetActive(false);
    }

    private void OnClickResumeButton()
    {
        GameManager.Instance?.ResumeGame();
    }

    private void OnClickQuitButton()
    {
        GameManager.Instance?.QuitGame();
    }

    private void OnClickSettingButton()
    {
        PopupManager.Instance.OpenPopup(EPopupType.GAMESETTING);
    }
}
