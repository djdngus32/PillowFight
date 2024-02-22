using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #region ╥ндц

    public void SaveDataToLocal(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
    }

    public void SaveDataToLocal(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
    }

    public void SaveDataToLocal(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
    }

    public int LoadDataToLocal(string key, int defaultValue = 0)
    {
        return PlayerPrefs.GetInt(key, defaultValue);
    }

    public float LoadDataToLocal(string key, float defaultValue = 0f)
    {
        return PlayerPrefs.GetFloat(key, defaultValue);
    }

    public string LoadDataToLocal(string key, string defaultValue = "")
    {
        return PlayerPrefs.GetString(key, defaultValue);
    }

    #endregion
}
