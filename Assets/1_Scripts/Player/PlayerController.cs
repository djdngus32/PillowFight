using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Fusion;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private Transform cameraFollowTarget;
    [SerializeField] private CharacterController characterController;

    [Header("Movement")]
    public float walkSpeed = 5f;
    public float jumpForce = 5f;

    [Header("Animation")]
    public float locomotionAnimChangeSpeed = 10f;

    [Header("Camera")]
    [SerializeField, Range(50f, 85f), Tooltip("카메라의 상하각도를 제한하기 위한 변수")]
    private float clampLookPitchAngle = 70f;

    [Header("Weapon")]
    [SerializeField] private Weapon equippedWeapon;

    //Movement 관련 변수
    private float velocityY;

    //Animation 관련 변수
    private Animator animator;
    private float animationBlend;
    private readonly float ANIMATION_WALK_SPEED = 2f;
    private readonly float ANIMATION_RUN_SPEED = 6f;
    private readonly float ANIMATION_ATTACK_LENGTH = 1.4f;

    // 애니메이션 ID
    //Parameter Hash
    private int animIDSpeed;
    private int animIDGrounded;
    private int animIDJump;
    private int animIDDeath;
    private int animIDAttack;
    private int animIDTakeDamage;
    //StateName Hash
    private int animNameIDAttack;
    private int animNameIDTakeDamage;

    //Camera 관련 변수
    private float lookPitch;
    private float lookYaw;

    //공격 관련 변수들
    private bool isPressAttackButton;
    private int localAttackCount;
    [Networked, HideInInspector] private int AttackCount { get; set; }
    [Networked] private TickTimer AttackCooldownTimer { get; set; }

    private PlayerStat stat;

    //움직임 동기화 중 떨림 방지용 변수
    private Vector3 lastAntiJitterPosition;
    private Vector2 antiJitterDistance = new Vector2(0.025f, 0.01f);
    

    #region Networked 변수
    [Networked, HideInInspector] public Vector3 OriginPosition { get; set; }
    [Networked, HideInInspector] public Quaternion OriginRotation { get; set; }
    [Networked, HideInInspector] public Vector3 MoveDirection { get; set; }
    [Networked, HideInInspector] public Vector2 LookRotationDelta { get; set; }
    [Networked, HideInInspector] public NetworkBool IsJump { get; set; }
    [Networked, HideInInspector] private NetworkBool IsGrounded { get; set; }
    #endregion

    private readonly float GRAVITY = -9.81f;

    public override void Spawned()
    {
        if(Object.HasInputAuthority == true)
        {
            FollowCameraController followCamera = FindAnyObjectByType<FollowCameraController>();
            if(followCamera != null)
            {
                followCamera.SetFollowTarget(cameraFollowTarget);
            }
            PlayerManager.Instance.Controller = this;
        }

        InitializeAnimator();

        localAttackCount = AttackCount;

        stat = transform.GetComponent<PlayerStat>();
    }

    public override void FixedUpdateNetwork()
    {
        if(Object.HasInputAuthority ==  false)
            Debug.Log($"Player Id : {Object.InputAuthority.PlayerId}");

        UpdateInput();

        //죽었다면 아무것도 못한다.
        if (stat != null && stat.IsAlive == false)
            return;

        UpdateRotation();
        UpdateMovement();
        UpdateAttack();        

        OriginPosition = transform.position;
        OriginRotation = transform.rotation;
    }

    public override void Render()
    {
        SynchronizeTransform(OriginPosition, OriginRotation);

        float moveAnimationSpeed = ANIMATION_WALK_SPEED * MoveDirection.magnitude;

        animationBlend = Mathf.Lerp(animationBlend, moveAnimationSpeed, Runner.DeltaTime * locomotionAnimChangeSpeed);
        if (animationBlend < 0.01f) animationBlend = 0f;

        animator.SetFloat(animIDSpeed, animationBlend);
        animator.SetBool(animIDJump, IsJump);
        animator.SetBool(animIDGrounded, IsGrounded);
        animator.SetBool(animIDDeath, !stat.IsAlive);        

        if (equippedWeapon != null)
        {
            if(AttackCount > localAttackCount)
            {
                animator.SetTrigger(animIDAttack);
                animator.SetFloat("AttackRatePerSecond", ANIMATION_ATTACK_LENGTH * equippedWeapon.AttackSpeed);
                localAttackCount++;
            }
        }

        if (stat.DamagedCount > stat.localDamagedCount)
        {
            animator.SetTrigger(animIDTakeDamage);
            stat.localDamagedCount++;
        }
    }

    public void Attack()
    {
        AttackCooldownTimer = TickTimer.CreateFromSeconds(Runner, equippedWeapon.AttackCooldownTime);
        AttackCount++;
    }

    private void InitializeAnimator()
    {
        animator = GetComponent<Animator>();
        animIDSpeed = Animator.StringToHash("Speed");
        animIDGrounded = Animator.StringToHash("Grounded");
        animIDJump = Animator.StringToHash("Jump");
        animIDDeath = Animator.StringToHash("Dead");
        animIDAttack = Animator.StringToHash("Attack");
        animIDTakeDamage = Animator.StringToHash("TakeDamage");
        animNameIDAttack = Animator.StringToHash("Combat_Attack");
        animNameIDTakeDamage = Animator.StringToHash("Combat_TakeDamage");
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

            LookRotationDelta = inputData.RotationDelta * GameManager.Instance.GameSetting.MouseSensitivity;

            if (LookRotationDelta.x != 0)
            {
                lookPitch = lookPitch + LookRotationDelta.x;
                lookPitch = Mathf.Clamp(lookPitch, -clampLookPitchAngle, clampLookPitchAngle);
            }

            if (LookRotationDelta.y != 0)
            {
                lookYaw = lookYaw + LookRotationDelta.y;
            }
            
            IsJump = inputData.IsPressed(PlayerInputData.BUTTON_JUMP);
            isPressAttackButton = inputData.IsPressed(PlayerInputData.BUTTON_ATTACK);
        }
    }

    private void UpdateRotation()
    {
        transform.rotation = Quaternion.Euler(0,lookYaw,0);
        cameraFollowTarget.localRotation = Quaternion.Euler(lookPitch,0,0);
    }

    private void UpdateMovement()
    {
        Vector3 moveVector = Vector3.zero;
        if(characterController.isGrounded)
        {
            velocityY = 0;
        }

        //공격중이거나 피격중이면 움직이지 못한다.
        if(IsPlayAnimation(animNameIDAttack) == false && IsPlayAnimation(animNameIDTakeDamage) == false)
        {
            moveVector = MoveDirection * walkSpeed;
            if (IsJump && characterController.isGrounded)
            {
                //Jump
                velocityY += jumpForce;
            }
        }
        velocityY += GRAVITY * Runner.DeltaTime;

        moveVector.y = velocityY * Runner.DeltaTime;

        IsGrounded = characterController.isGrounded;
        characterController.Move(moveVector);
    }

    private void UpdateAttack()
    {
        if (isPressAttackButton // 공격버튼을 누르고있고
            && IsGrounded // 발이 땅에 붙어있고
            && IsPlayAnimation(animNameIDTakeDamage) == false // 맞는중이 아니여야하고
            && AttackCooldownTimer.ExpiredOrNotRunning(Runner)) // 공격쿨타임이 돌아왔다면
        {
            //공격한다.
            Attack();
        }
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

    #region 애니메이션 이벤트

    private void OnAttackSwingStart(AnimationEvent animationEvent)
    {
        //애니메이션 이벤트는 모든 클라이언트에서 발생하기 때문에 모든 클라이언트에서 Attack을 호출하면 공격이 여러번 들어가게 되기 때문에
        //소유권이 있는 사람만 Attack 함수를 호출 할 수 있게 해야한다.
        if (equippedWeapon == null || Object.HasInputAuthority == false)
            return;

        equippedWeapon.Attack(transform.position + characterController.center, transform.rotation);
    }

    private void OnAttackSwingEnd(AnimationEvent animationEvent)
    {
    }

    private void OnFootstep(AnimationEvent animationEvent)
    {
        //사운드 재생
    }

    private void OnLand(AnimationEvent animationEvent)
    {
        //사운드 재생
    }

    #endregion
}
