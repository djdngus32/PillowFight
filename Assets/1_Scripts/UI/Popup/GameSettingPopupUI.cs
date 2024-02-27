using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class GameSettingPopupUI : PopupUI
{
    public override EPopupType PopupType => EPopupType.GAMESETTING;

    [SerializeField] private TMP_InputField mouseSensitivityInputField;
    [SerializeField] private Button confirmButton;

    private void Start()
    {
        mouseSensitivityInputField.onEndEdit.AddListener(OnEndEditMouseSensitivityInputField);
        confirmButton.onClick.AddListener(OnClickConfirmButton);
    }

    public override void Open()
    {
        mouseSensitivityInputField.text = GameManager.Instance.GameSetting.MouseSensitivity.ToString();
        base.Open();
    }

    private void OnEndEditMouseSensitivityInputField(string inputText)
    {
        GameManager.Instance.GameSetting.MouseSensitivity = float.Parse(inputText);
    }

    private void OnClickConfirmButton()
    {
        Close();
    }
}
