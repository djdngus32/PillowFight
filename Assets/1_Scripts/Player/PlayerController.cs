using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Fusion;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private Transform cameraFollowTarget;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Transform weaponEquipTransform;

    [Networked] public Weapon CurrentWeapon { get; set; }

    [Header("Movement")]
    public float walkSpeed = 5f;
    public float jumpForce = 5f;
    public float cameraDistance = 1f;
    [Header("Animation")]
    public float locomotionAnimChangeSpeed = 10f;

    private float lookPitch;
    private float lookYaw;
    private float animationWalkSpeed = 2f;
    private float animationRunSpeed = 6f;
    private Vector3 velocity;
    private float gravity = -9.81f;
    float animationBlend;
    private Animator animator;
    private Weapon[] weapons;

    private Vector2 antiJitterDistance = new Vector2(0.025f, 0.01f);

    // animation IDs
    private int animIDSpeed;
    private int animIDGrounded;
    private int animIDJump;
    private int animIDDeath;
    private int animIDAttack;
    private int animIDTakeDamage;

    private PlayerStat stat;

    #region Networked 변수
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
            FollowCameraController followCamera = FindAnyObjectByType<FollowCameraController>();
            if(followCamera != null)
            {
                followCamera.SetFollowTarget(cameraFollowTarget);
            }
            PlayerManager.Instance.Controller = this;
        }

        InitializeAnimator();
        InitializeWeapons();

        if(weapons.Length > 0)
        {
            EquipWeapon(weapons[0]);
        }
        

        stat = transform.GetComponent<PlayerStat>();
    }

    public override void FixedUpdateNetwork()
    {
        if(Object.HasInputAuthority ==  false)
            Debug.Log($"Player Id : {Object.InputAuthority.PlayerId}");

        UpdateInput();

        if (stat != null && stat.IsAlive == false)
            return;

        UpdateRotation();
        UpdateMovement();

        OriginPosition = transform.position;
        OriginRotation = transform.rotation;
    }

    public override void Render()
    {
        SynchronizeTransform(OriginPosition, OriginRotation);

        //애니메이션 재생
        bool isPressedSprintButton = false;
        float targetSpeed = isPressedSprintButton ? animationRunSpeed : animationWalkSpeed;
        targetSpeed *= MoveDirection.magnitude;

        animationBlend = Mathf.Lerp(animationBlend, targetSpeed, Runner.DeltaTime * locomotionAnimChangeSpeed);
        if (animationBlend < 0.01f) animationBlend = 0f;

        animator.SetFloat(animIDSpeed, animationBlend);
        animator.SetBool(animIDJump, IsJump);
        animator.SetBool(animIDGrounded, characterController.isGrounded);
        animator.SetBool(animIDDeath, !stat.IsAlive);

        if (CurrentWeapon != null)
        {
            if(CurrentWeapon.FireCount > CurrentWeapon.localFireCount)
            {
                animator.SetTrigger(animIDAttack);
                animator.SetFloat("AttackRatePerSecond",CurrentWeapon.FireRatePerSecond);
                CurrentWeapon.localFireCount++;
            }
        }

        if (stat.DamagedCount > stat.localDamagedCount)
        {
            animator.SetTrigger(animIDTakeDamage);
            stat.localDamagedCount++;
        }
    }

    public void EquipWeapon(Weapon weapon)
    {
        if (weapon == null) return;

        weapon.transform.parent = weaponEquipTransform;
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localRotation = Quaternion.identity;
        weapon.transform.localScale = Vector3.one;
        CurrentWeapon = weapon;
    }

    public void Fire()
    {
        if (CurrentWeapon == null)
            return;

        CurrentWeapon.Fire(CurrentWeapon.gameObject.transform.position, transform.forward);
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
    }

    private void InitializeWeapons()
    {
        if (weaponEquipTransform == null)
            return;

        weapons = weaponEquipTransform.GetComponentsInChildren<Weapon>();
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

            if(inputData.IsPressed(PlayerInputData.BUTTON_ATTACK))
            {
                Fire();
            }

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
        if(characterController.isGrounded)
        {
            velocity = new Vector3(0, -1f, 0);
        }

        velocity.y += gravity * Runner.DeltaTime;
        if(IsJump && characterController.isGrounded)
        {
            //Jump
            velocity.y += jumpForce;
        }

        characterController.Move((MoveDirection * walkSpeed) + velocity * Runner.DeltaTime);
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
        //사운드 재생
    }

    private void OnLand(AnimationEvent animationEvent)
    {
        //사운드 재생
    }
}
