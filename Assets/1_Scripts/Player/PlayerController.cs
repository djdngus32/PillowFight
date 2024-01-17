using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private Transform cameraFollowTarget;
    [SerializeField] private CharacterController characterController;

    public float walkSpeed = 5f;
    public float cameraDistance = 1f;
    [Header("Animation")]
    public float locomotionAnimChangeSpeed = 10f;

    private float lookPitch;
    private float lookYaw;
    private float animationWalkSpeed = 2f;
    private float animationRunSpeed = 6f;
    float animationBlend;
    private Animator animator;

    private Vector2 antiJitterDistance = new Vector2(0.025f, 0.01f);

    // animation IDs
    private int animIDSpeed;
    private int animIDGrounded;
    private int animIDJump;
    private int animIDFreeFall;

    private SceneCameraHandler sceneCamera;

    #region Networked ����
    [Networked, HideInInspector] public Vector3 OriginPosition { get; set; }
    [Networked, HideInInspector] public Quaternion OriginRotation { get; set; }
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

        InitializeAnimator();
    }

    public override void FixedUpdateNetwork()
    {
        if(Object.HasInputAuthority ==  false)
            Debug.Log($"Player Id : {Object.InputAuthority.PlayerId}");

        UpdateInput();
        UpdateRotation();
        UpdateMovement();

        OriginPosition = transform.position;
        OriginRotation = transform.rotation;
    }

    public override void Render()
    {
        SynchronizeTransform(OriginPosition, OriginRotation);

        if (Object.HasInputAuthority == false)
            return;

        //�ִϸ��̼� ���
        bool isPressedSprintButton = false;
        float targetSpeed = isPressedSprintButton ? animationRunSpeed : animationWalkSpeed;
        targetSpeed *= MoveDirection.magnitude;

        animationBlend = Mathf.Lerp(animationBlend, targetSpeed, Runner.DeltaTime * locomotionAnimChangeSpeed);
        if (animationBlend < 0.01f) animationBlend = 0f;

        animator.SetFloat(animIDSpeed, animationBlend);

        Vector3 lookDirection = cameraFollowTarget.transform.forward;
        sceneCamera.Camera.transform.localPosition = cameraFollowTarget.position - lookDirection * cameraDistance;
        sceneCamera.Camera.transform.localRotation = cameraFollowTarget.rotation;
    }

    private void InitializeAnimator()
    {
        animator = GetComponent<Animator>();
        animIDSpeed = Animator.StringToHash("Speed");
        animIDGrounded = Animator.StringToHash("Grounded");
        animIDJump = Animator.StringToHash("Jump");
        animIDFreeFall = Animator.StringToHash("FreeFall");
    }

    private void UpdateInput()
    {
        if (GetInput(out PlayerInputData inputData))
        {
            //else if (myCamera != null)
            //{
            //    forward = Vector3.Scale(myCamera.transform.forward, new Vector3(1, 0, 1)); // (ī�޶� ����) ĳ���� ���ʹ���
            //    right = Vector3.Scale(myCamera.transform.right, new Vector3(1, 0, 1)); // (ī�޶� ����) ĳ���� �����ʹ���
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

    Vector3 lastAntiJitterPosition;

    private void SynchronizeTransform(Vector3 targetPosition, Quaternion targetRotation)
    {
        Vector3 vector = targetPosition;
        //_rigidbody.position = vector;

        Vector3 vector2 = vector - lastAntiJitterPosition;
        if (vector2.sqrMagnitude < 1f)
        {
            vector = lastAntiJitterPosition;
            float num = Mathf.Abs(vector2.y);
            if (num > 1E-06f)
            {
                vector.y += vector2.y * Mathf.Clamp01((num - antiJitterDistance.y) / num);
            }

            vector2.y = 0;
            Vector3 vector3 = vector2;
            float num2 = Vector3.Magnitude(vector3);
            if (num2 > 1E-06f)
            {
                vector += vector3 * Mathf.Clamp01((num2 - antiJitterDistance.x) / num2);
            }
        }

        lastAntiJitterPosition = vector;
        transform.SetPositionAndRotation(vector, targetRotation);
    }

    private void OnFootstep(AnimationEvent animationEvent)
    {

    }
}
