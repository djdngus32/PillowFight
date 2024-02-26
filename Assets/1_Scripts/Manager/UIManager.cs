using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private GameObject dontDestroyCanvas;
    [SerializeField] private LoadingUIHandler loadingUI;
    [SerializeField] private PauseUIHandler pauseUI;

    public LoadingUIHandler LoadingUI => loadingUI;
    public PauseUIHandler PauseUI => pauseUI;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(Instance.gameObject);
        }
        Instance = this;
        DontDestroyOnLoad(this);
        DontDestroyOnLoad(dontDestroyCanvas);
    }

    private void Start()
    {
        if(loadingUI.IsOpen)
        {
            loadingUI.Close();
        }
    }
}
