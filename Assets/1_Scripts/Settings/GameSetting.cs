public class GameSetting
{
    #region 마우스 감도 설정 관련

    private const string MOUSE_SENSITIVITY_KEY = "MouseSensitivity";
    private const float DEFAULT_MOUSE_SENSITIVITY = 10f;

    private float mouseSensitivity;

    public float MouseSensitivity
    {
        get
        {
            if(mouseSensitivity == 0f)
            {
                float? loadedData = GameDataManager.Instance?.LoadDataToLocal(MOUSE_SENSITIVITY_KEY, 0f);

                if(loadedData == null || loadedData.Value == 0f)
                {
                    mouseSensitivity = DEFAULT_MOUSE_SENSITIVITY;
                }
                else
                {
                    mouseSensitivity = loadedData.Value;
                }
            }

            return mouseSensitivity;
        }

        set
        {
            if(mouseSensitivity != value)
            {
                mouseSensitivity = value;
                GameDataManager.Instance?.SaveDataToLocal(MOUSE_SENSITIVITY_KEY, value);
            }
        }
    }

    #endregion
}
