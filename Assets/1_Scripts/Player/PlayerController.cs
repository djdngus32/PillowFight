using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.EventSystems;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private Transform cameraFollowTarget;
    [SerializeField] private CharacterController characterController;

    public float walkSpeed = 5f;
    public float cameraDistance = 1f;

    private float lookPitch;
    private float lookYaw;

    private SceneCameraHandler sceneCamera;

    #region Networked 변수

    [Networked, HideInInspector] public Vector3 MoveDirection { get; set; }
    [Networked, HideInInspector] public Vector2 LookRotationDelta { get; set; }
    [Networked, HideInInspector] public NetworkBool IsJump { get; set; }

    #endregion

    public override void Spawned()
    {
        if(Object.HasInputAuthority == true)
        {
            sceneCamera = FindObjectOfType<SceneCameraHandler>();
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (Object.HasInputAuthority == false)
            return;

        UpdateInput();
        UpdateRotation();
        UpdateMovement();
    }

    public override void Render()
    {
        Vector3 lookDirection = cameraFollowTarget.transform.forward;
        sceneCamera.Camera.transform.localPosition = cameraFollowTarget.position - lookDirection * cameraDistance;
        sceneCamera.Camera.transform.localRotation = cameraFollowTarget.rotation;
    }

    private void UpdateInput()
    {
        if (GetInput(out PlayerInputData inputData))
        {
            //else if (myCamera != null)
            //{
            //    forward = Vector3.Scale(myCamera.transform.forward, new Vector3(1, 0, 1)); // (카메라 기준) 캐릭터 앞쪽방향
            //    right = Vector3.Scale(myCamera.transform.right, new Vector3(1, 0, 1)); // (카메라 기준) 캐릭터 오른쪽방향
            //}
            Vector3 forward = transform.forward;
            Vector3 right = transform.right;

            Vector3 inputMoveDirection = Vector3.zero;
            if (inputData.IsPressed(PlayerInputData.BUTTON_FORWARD))
                inputMoveDirection += forward;

            if (inputData.IsPressed(PlayerInputData.BUTTON_BACKWARD))
                inputMoveDirection += (-forward);

            if (inputData.IsPressed(PlayerInputData.BUTTON_LEFT))
                inputMoveDirection += (-right);

            if (inputData.IsPressed(PlayerInputData.BUTTON_RIGHT))
                inputMoveDirection += right;

            MoveDirection = inputMoveDirection.normalized;

            LookRotationDelta = inputData.RotationDelta;

            if (LookRotationDelta.x != 0)
            {
                lookPitch = lookPitch + LookRotationDelta.x;
            }

            if (LookRotationDelta.y != 0)
            {
                lookYaw = lookYaw + LookRotationDelta.y;
            }

            IsJump = inputData.IsPressed(PlayerInputData.BUTTON_JUMP);
        }
    }

    private void UpdateRotation()
    {
        transform.rotation = Quaternion.Euler(0,lookYaw,0);
        cameraFollowTarget.localRotation = Quaternion.Euler(lookPitch,0,0);

    }

    private void UpdateMovement()
    {
        characterController.Move(MoveDirection * walkSpeed);
    }
}
