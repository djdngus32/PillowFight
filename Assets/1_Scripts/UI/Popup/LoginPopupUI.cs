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

    public const string PLAYER_NICKNAME_KEY = "PlayerNickName";

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

        GameDataManager.Instance.SaveDataToLocal(PLAYER_NICKNAME_KEY, nickNameInputField.text);

        gameObject.SetActive(false);

        NetworkManager.Instance.JoinOrCreateSession();
    }
}
