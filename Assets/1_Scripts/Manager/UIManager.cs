using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private LoadingUIHandler loadingUI;

    public LoadingUIHandler LoadingUI => loadingUI;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(Instance.gameObject);
        }
        Instance = this;
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        if(loadingUI.IsOpen)
        {
            loadingUI.Close();
        }
    }
}
