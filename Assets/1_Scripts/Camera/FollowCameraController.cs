using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class FollowCameraController : MonoBehaviour
{
    private CinemachineVirtualCamera followCamera;

    private void Awake()
    {
        followCamera = transform.GetComponent<CinemachineVirtualCamera>();
        if(followCamera == null)
        {
            Debug.LogError("Failed to find the follow camera");
        }
    }

    private void Start()
    {
        followCamera.enabled = false;
    }

    public void SetFollowTarget(Transform targetObject)
    {
        if (followCamera == null || targetObject == null)
            return;

        followCamera.Follow = targetObject;
        followCamera.LookAt = targetObject;
        followCamera.enabled = true;
    }
}
