using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingUIHandler : MonoBehaviour
{
    public bool IsOpen => gameObject.activeSelf;

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
