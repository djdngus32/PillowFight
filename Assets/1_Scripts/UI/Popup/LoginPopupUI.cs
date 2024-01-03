using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoginPopupUI : PopupUI
{
    public override EPopupType PopupType => EPopupType.LOGIN;

    [SerializeField] private TMP_InputField nickNameInputField;
    [SerializeField] private Button loginButton;

    private void Start()
    {
        loginButton?.onClick.AddListener(OnClickLoginButton);
    }
    
    private void OnClickLoginButton()
    {
        if(string.IsNullOrEmpty(nickNameInputField.text))
        {
            return;
        }

        gameObject.SetActive(false);
    }
}
