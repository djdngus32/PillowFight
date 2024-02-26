using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseUIHandler : MonoBehaviour
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button quitButton;

    public bool IsOpen => gameObject.activeSelf;

    private void Start()
    {
        resumeButton?.onClick.AddListener(OnClickResumeButton);
        quitButton?.onClick.AddListener(OnClickQuitButton);
    }

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
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
}
