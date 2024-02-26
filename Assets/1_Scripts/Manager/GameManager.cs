using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {  get; private set; }

    public bool IsPause { get; private set; }
    public GameSetting GameSetting { get; private set; }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            GameSetting = new GameSetting();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }        
    }

    public void ResumeGame()
    {
        UIManager.Instance.PauseUI.Close();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        IsPause = false;
    }

    public void PauseGame()
    {
        UIManager.Instance.PauseUI.Open();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        IsPause = true;
    }

    public void QuitGame()
    {
        NetworkManager.Instance.DisConnect();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
