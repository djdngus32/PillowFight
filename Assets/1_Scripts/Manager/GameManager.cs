using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
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
