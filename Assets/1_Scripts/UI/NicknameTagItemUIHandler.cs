using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NicknameTagItemUIHandler : MonoBehaviour
{
    [SerializeField] private TMP_Text nicknameText;

    private Transform followTarget;

    public void SetUp(string nickname, Transform followTargetTransform)
    {
        nicknameText.text = nickname;
        followTarget = followTargetTransform;
    }

    private void OnDisable()
    {
        nicknameText.text = string.Empty;
        followTarget = null;
    }

    private void LateUpdate()
    {
        if (followTarget == null)
            return;
        
        transform.position = followTarget.position + Vector3.up * 2f;

        Vector3 toCamera = Camera.main.transform.position - transform.position;
        toCamera.y = 0; // y 축 회전만 고려
        transform.rotation = Quaternion.LookRotation(toCamera, Vector3.up);
    }
}
