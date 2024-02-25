using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {  get; private set; }

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
