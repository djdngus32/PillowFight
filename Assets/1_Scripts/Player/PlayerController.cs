using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Fusion;
using Fusion.Addons.SimpleKCC;
using UnityEngine.EventSystems;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private SimpleKCC KCC;
    [SerializeField] private PlayerWeaponController weaponController;
    [SerializeField] private PlayerStat stat;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform cameraFollowTarget;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float groundAcceleration = 55f;
    [SerializeField] private float groundDeceleration = 25f;
    [SerializeField] private float airAcceleration = 25f;
    [SerializeField] private float airDeceleration = 1.3f;

    [Header("Camera")]
    [SerializeField, Range(50f, 85f), Tooltip("ī�޶��� ���ϰ����� �����ϱ� ���� ����")]
    private float clampLookPitchAngle = 70f;

    //Animation ���� ����
    private float animationBlend;
    private readonly float ANIMATION_WALK_SPEED = 2f;
    private readonly float ANIMATION_RUN_SPEED = 6f;
    private readonly float ANIMATION_ATTACK_LENGTH = 1.4f;

    // �ִϸ��̼� ID
    //Parameter Hash
    private int animIDMoveSpeed;
    private int animIDIsGrounded;
    private int animIDJump;
    private int animIDIsAlive;
    private int animIDTakeDamage;

    [Networked, HideInInspector] public Vector3 MoveVelocity { get; set; }

    [Networked, HideInInspector] 
    private int JumpCount { get; set; }
    private int visibleJumpCount;

    private readonly float GRAVITY = -9.81f;

    public override void Spawned()
    {
        if (Object.HasInputAuthority == true)
        {
            FollowCameraController followCamera = FindAnyObjectByType<FollowCameraController>();
            if (followCamera != null)
            {
                followCamera.SetFollowTarget(cameraFollowTarget);
            }
            PlayerManager.Instance.Controller = this;
        }

        InitializeAnimator();
    }

    public override void FixedUpdateNetwork()
    {
        if(GameManager.Instance.IsPause)
        {
            //������ ������ ���¿����� �Էµ� �ȹް� ���� �������� ����ǵ����Ѵ�. Ȥ�� ���߿� ������ �� �� �����ϱ�.
            MovePlayer();
            return;
        }

        if (IsProxy)
            return;

        //UpdateInput();

        //�׾��ٸ� �ƹ��͵� ���Ѵ�.
        if (stat != null && stat.IsAlive == false)
            return;

        if(GetInput(out PlayerInputData inputData))
        {
            ProcessInput(inputData);
        }
        else
        {
            //�Է��� ���ٸ� ���� �����ִ� �������� ����� �� �ְ� �Ѵ�. ��) �������� �ִ� ��
            MovePlayer();
        }
    }

    public override void Render()
    {

        Vector3 moveVelocity = GetAnimationMoveVelocity();

        animator.SetFloat("MoveX", moveVelocity.x, 0.05f, Time.deltaTime);
        animator.SetFloat("MoveZ", moveVelocity.z, 0.05f, Time.deltaTime);
        animator.SetFloat(animIDMoveSpeed, moveVelocity.magnitude);

        animator.SetBool(animIDIsGrounded, KCC.IsGrounded);
        animator.SetBool(animIDIsAlive, stat.IsAlive);

        if(visibleJumpCount < JumpCount)
        {
            //Ʈ���ŷ� ������ ��
            animator.SetTrigger(animIDJump);
        }
        visibleJumpCount = JumpCount;


        if (stat.DamagedCount > stat.localDamagedCount)
        {
            animator.SetTrigger(animIDTakeDamage);
            stat.localDamagedCount++;
        }
    }

    private void LateUpdate()
    {
        if (HasInputAuthority == false)
            return;

        var pitchRotation = KCC.GetLookRotation(true, false);
        cameraFollowTarget.localRotation = Quaternion.Euler(pitchRotation);
    }

    private void InitializeAnimator()
    {
        animIDMoveSpeed = Animator.StringToHash("MoveSpeed");
        animIDIsGrounded = Animator.StringToHash("IsGrounded");
        animIDJump = Animator.StringToHash("Jump");
        animIDIsAlive = Animator.StringToHash("IsAlive");
        animIDTakeDamage = Animator.StringToHash("TakeDamage");
    }

    //�Է¿� ���� ���� ȸ��, �̵�, ����, ������ ó���ϴ� �Լ�
    private void ProcessInput(PlayerInputData inputData)
    {
        KCC.AddLookRotation(inputData.RotationDelta, -clampLookPitchAngle, clampLookPitchAngle);

        KCC.SetGravity(GRAVITY);

        Vector3 inputMoveDirection = KCC.TransformRotation * new Vector3(inputData.MoveDirection.x, 0, inputData.MoveDirection.y);
        float jumpImpulse = 0f;

        if (inputData.IsPressed(PlayerInputData.BUTTON_JUMP) && KCC.IsGrounded)
        {
            jumpImpulse = jumpForce;
        }

        MovePlayer(inputMoveDirection * moveSpeed, jumpImpulse);

        if(KCC.HasJumped)
        {
            JumpCount++;
        }

        if(inputData.IsPressed(PlayerInputData.BUTTON_ATTACK))
        {
            weaponController.Attack();
        }
    }

    private void MovePlayer(Vector3 desiredMoveVelocity = default, float jumpImpulse = 0)
    {
        float acceleration;

        if(desiredMoveVelocity == Vector3.zero)
        {
            acceleration = KCC.IsGrounded == true ? groundDeceleration : airDeceleration;
        }
        else
        {
            acceleration = KCC.IsGrounded == true ? groundAcceleration : airAcceleration;
        }

        MoveVelocity = Vector3.Lerp(MoveVelocity, desiredMoveVelocity, acceleration * Runner.DeltaTime);
        KCC.Move(MoveVelocity, jumpImpulse);
    }

    //���� �÷��̾��� �̵��ӵ��� �ִϸ��̼� ��������� �ǳ��ִ� �Լ�
    private Vector3 GetAnimationMoveVelocity()
    {
        if (KCC.RealSpeed < 0.01f)
            return Vector3.zero;

        Vector3 velocity = KCC.RealVelocity;

        velocity.y = 0f;

        if(velocity.sqrMagnitude > 1f)
        {
            velocity.Normalize();
        }

        return transform.InverseTransformVector(velocity);
    }

    /// <summary>
    /// �ش� �ִϸ��̼��� ��������� Ȯ���ϴ� �Լ�
    /// </summary>
    /// <param name="animationStateID">�˰���� �ִϸ��̼� State�� �̸��� Animator.StringToHash�� ��ȯ�� ��</param>
    /// <returns>�ش� �ִϸ��̼��� ������̴ٸ� true�� ��ȯ</returns>
    private bool IsPlayAnimation(int animationStateID)
    {
        AnimatorStateInfo currentStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        AnimatorStateInfo nextStateInfo = animator.GetNextAnimatorStateInfo(0);

        return currentStateInfo.shortNameHash == animationStateID || nextStateInfo.shortNameHash == animationStateID;
    }
}
