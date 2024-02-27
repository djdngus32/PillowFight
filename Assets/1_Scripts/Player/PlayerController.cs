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
    [SerializeField, Range(50f, 85f), Tooltip("카메라의 상하각도를 제한하기 위한 변수")]
    private float clampLookPitchAngle = 70f;

    //Animation 관련 변수
    private float animationBlend;
    private readonly float ANIMATION_WALK_SPEED = 2f;
    private readonly float ANIMATION_RUN_SPEED = 6f;
    private readonly float ANIMATION_ATTACK_LENGTH = 1.4f;

    // 애니메이션 ID
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
            //게임이 정지된 상태에서는 입력도 안받고 그저 물리현상만 적용되도록한다. 혹시 공중에 떠있을 수 도 있으니까.
            MovePlayer();
            return;
        }

        if (IsProxy)
            return;

        //UpdateInput();

        //죽었다면 아무것도 못한다.
        if (stat != null && stat.IsAlive == false)
            return;

        if(GetInput(out PlayerInputData inputData))
        {
            ProcessInput(inputData);
        }
        else
        {
            //입력이 없다면 현재 남아있는 물리현상만 적용될 수 있게 한다. 예) 떨어지고 있는 중
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
            //트리거로 변경할 것
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

    //입력에 따라 시점 회전, 이동, 점프, 공격을 처리하는 함수
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

    //현재 플레이어의 이동속도를 애니메이션 재생용으로 건네주는 함수
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
    /// 해당 애니메이션이 재생중인지 확인하는 함수
    /// </summary>
    /// <param name="animationStateID">알고싶은 애니메이션 State의 이름을 Animator.StringToHash로 변환한 값</param>
    /// <returns>해당 애니메이션이 재생중이다면 true를 반환</returns>
    private bool IsPlayAnimation(int animationStateID)
    {
        AnimatorStateInfo currentStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        AnimatorStateInfo nextStateInfo = animator.GetNextAnimatorStateInfo(0);

        return currentStateInfo.shortNameHash == animationStateID || nextStateInfo.shortNameHash == animationStateID;
    }
}
