using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KillLogItemUIHandler : MonoBehaviour
{
    [SerializeField] private TMP_Text killerPlayerNicknameText;
    [SerializeField] private TMP_Text killedPlayerNicknameText;

    [HideInInspector] public RectTransform RectTransform;

    private CanvasGroup canvasGroup;
    private Coroutine fadeOutCoroutine;

    private void Awake()
    {
        this.RectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Show(string killerPlayerNickname, string killedPlayerNickname, float logDurationTime)
    {
        if(gameObject.activeSelf == false)
        {
            gameObject.SetActive(true);
        }

        killerPlayerNicknameText.text = killerPlayerNickname;
        float killerWidth = killerPlayerNicknameText.preferredWidth;
        killerPlayerNicknameText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, killerWidth); 

        killedPlayerNicknameText.text = killedPlayerNickname;
        float killedWidth = killedPlayerNicknameText.preferredWidth;
        killedPlayerNicknameText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, killedWidth);

        if(fadeOutCoroutine != null)
        {
            StopCoroutine(fadeOutCoroutine);            
        }

        canvasGroup.alpha = 1f;
        fadeOutCoroutine = StartCoroutine(CoFadeOut(logDurationTime));
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private IEnumerator CoFadeOut(float duration)
    {
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, timeElapsed / duration);
            yield return null;
        }

        fadeOutCoroutine = null;
        Hide();
        yield return null;
    }

    
}
