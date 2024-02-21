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

    // Update is called once per frame
    void Update()
    {
        if(Input.backButtonLeavesApp || Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
