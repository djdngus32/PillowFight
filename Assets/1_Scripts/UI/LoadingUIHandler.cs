using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingUIHandler : MonoBehaviour
{
    [SerializeField] private Canvas loadingUICanvas;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public bool IsOpen => loadingUICanvas.enabled;

    public void Open()
    {
        loadingUICanvas.enabled = true;
    }

    public void Close()
    {
        loadingUICanvas.enabled = false;
    }
}
