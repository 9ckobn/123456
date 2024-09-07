using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Ground Check")] [SerializeField]
    private Transform groundCheckTransform;

    [SerializeField] private float checkRadius;
    [SerializeField] private LayerMask groundMask;

    [Header("Gravity Config")] [SerializeField]
    private float gravityScaleOnFall = 2.2f;

    [SerializeField] private float gravityScaleOnAirRoll = 10f;

    [Header("Movement")] [SerializeField] private float dashSpeed;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float laneWidth = 4;

    [Header("Animation Config")] [SerializeField]
    private float animationFadeTime = 0.05f;

    [Header("Sound")] [SerializeField] private AudioClip[] sounds;

    private CharacterController characterController;
    private Animator animator;
    private AudioSource audioSource;
    private PlayerInput inputReader;

    private Vector3 verticalVelocity = Vector3.zero;
    private readonly float gravity = Physics.gravity.y;
    private bool isInAir = false;
    private bool isFalling = false;
    private bool airRoll = false;
    private bool isRolling = false;
    private bool isChangingSide = false;

    private float startRunSpeed;
    private float currentRunSpeed;
    private float pastLane;
    private float currentLane = 1;
    private float targetX;

    private Vector3 colliderCenterOnRoll;
    private Vector3 colliderCenterOnStart;
    private float colliderHeightOnStart;

    private Coroutine sideMovesCoroutine;
    private WaitForSeconds rollCooldown = new WaitForSeconds(0.4f);
    private WaitForSeconds bumpCooldown = new WaitForSeconds(1f);

    private Transform playerTransform;
    [SerializeField] private ParticleSystem damageParticles;

    private readonly int animHitHash = Animator.StringToHash("hit");
    private readonly int animDodgeLeftHash = Animator.StringToHash("dodgeLeft");
    private readonly int animDodgeRightHash = Animator.StringToHash("dodgeRight");
    private readonly int animRollHash = Animator.StringToHash("roll");
    private readonly int animJumpHash = Animator.StringToHash("jump2");
    private readonly int animRunHash = Animator.StringToHash("runSpeed");
    private readonly int animRunStartHash = Animator.StringToHash("run");
    private readonly int animInAirDodgeRightHash = Animator.StringToHash("airDodgeRight");
    private readonly int animInAirDodgeLeftHash = Animator.StringToHash("airDodgeLeft");
    private readonly int animBoolIsGrounded = Animator.StringToHash("isGrounded");
    private readonly int animCaughtHash = Animator.StringToHash("caught");
    private readonly int animDeadHash = Animator.StringToHash("death");
    private readonly int animIntroRunHash = Animator.StringToHash("introRun");
    private readonly int animIdleHash = Animator.StringToHash("idle");

    public Action<int> onGetDamage;
    public Action<Action<float, int>> onDeath;

    public bool IsGrounded => Physics.CheckSphere(groundCheckTransform.position, checkRadius, groundMask);
    public bool IsInAir => isInAir;
    public bool IsFalling => isFalling;

    public event Action OnLanded;
    public event Action<float> OnGrounded;
    public event Action OnFall;

    public bool blockRunning = true;

    private int levelCoins = 0;

    public int AddCoin
    {
        set
        {
            levelCoins += value;
            collectFx.Play();
        }
        get => levelCoins;
    }

    [SerializeField] private ParticleSystem collectFx;

    public static Player instance;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        inputReader = GetComponent<PlayerInput>();
        playerTransform = GetComponent<Transform>();


        instance = this;
    }

    private void OnEnable()
    {
        inputReader.OnGoLeftEvent += HandleOnGoLeftEvent;
        inputReader.OnGoRightEvent += HandleOnGoRightEvent;
        inputReader.OnRollEvent += HandleOnRollEvent;
        inputReader.OnJumpEvent += HandleOnJumpEvent;
        OnLanded += HandleOnLanded;
        OnFall += HandleOnFall;

        onDeath += SetDeath;

        colliderCenterOnStart = characterController.center;
        colliderHeightOnStart = characterController.height;
        colliderCenterOnRoll = new Vector3(characterController.center.x, characterController.center.y / 2,
            characterController.center.z);
    }

    private void Start()
    {
        transform.position = new Vector3(0, 0.44f, 10);
        animator.SetBool(animBoolIsGrounded, true);
    }

    public void StartRun()
    {
        blockRunning = false;
        animator.ResetTrigger(animRunStartHash);
        animator.SetTrigger(animRunStartHash);
        animator.SetFloat(animRunHash, currentRunSpeed / 10);
    }

    public void ClearPlayer()
    {
        transform.position = new Vector3(0, 0.44f, 10);
        animator.SetBool(animBoolIsGrounded, true);
        animator.ResetTrigger(animHitHash);

        levelCoins = 0;
        blockRunning = true;
        currentRunSpeed = 0;
        currentLane = 1;
        pastLane = 1;

        animator.Play(animIdleHash);
        characterController.enabled = true;
        characterController.detectCollisions = true;
        inputReader.enabled = true;
    }

    public void ClearPlayerWithoutRestart()
    {
        animator.SetBool(animBoolIsGrounded, true);
        animator.ResetTrigger(animHitHash);

        blockRunning = false;
        characterController.enabled = true;
        characterController.detectCollisions = true;
        inputReader.enabled = true;

        RaycastHit[] hits;
        hits = Physics.SphereCastAll(transform.position, 50, transform.forward, 10);

        foreach (var hit in hits)
        {
            if (hit.collider.TryGetComponent(out IObstacle obstacle))
            {
                hit.collider.gameObject.SetActive(false);
            }
        }

        SideMove(pastLane);

        animator.CrossFadeInFixedTime(animRollHash, animationFadeTime);
    }

    private void Update()
    {
        if (blockRunning)
            return;

        currentRunSpeed = 1.54f * MathF.Pow(transform.position.z, 0.3f) + 6.52f;
        animator.SetFloat(animRunHash, currentRunSpeed / 10);

        HandleGravity();

        if (IsGrounded)
        {
            if (verticalVelocity.y < 0f)
                verticalVelocity.y = -2f;

            OnGrounded?.Invoke(playerTransform.position.y);
        }
        else
        {
            isInAir = true;
            if (verticalVelocity.y < 0f)
            {
                isFalling = true;
                OnFall?.Invoke();
            }
        }

        if (IsGrounded && isInAir && verticalVelocity.y < 0f)
        {
            isInAir = false;
            isFalling = false;
            OnLanded?.Invoke();
        }

        if (IsGrounded && airRoll && verticalVelocity.y < 0f)
        {
            airRoll = false;
        }

        Vector3 forwardDir = Vector3.forward * currentRunSpeed;
        characterController.Move((verticalVelocity + forwardDir) * Time.deltaTime);
    }

    private void HandleGravity()
    {
        if (airRoll)
        {
            verticalVelocity.y += gravity * Time.deltaTime * gravityScaleOnAirRoll;
        }
        else
        {
            verticalVelocity.y += gravity * Time.deltaTime * gravityScaleOnFall;
        }
    }

    private void HandleOnGoLeftEvent()
    {
        if (blockRunning)
            return;

        ChangeLane(-1);
    }

    private void HandleOnGoRightEvent()
    {
        if (blockRunning)
            return;

        ChangeLane(1);
    }

    private void ChangeLane(int direction)
    {
        var newLane = currentLane + direction;

        int animationHash = IsInAir ? animInAirDodgeRightHash : direction == 1 ? animDodgeRightHash : animDodgeLeftHash;
        animator.CrossFadeInFixedTime(animationHash, 0.05f);

        if (newLane > 2 || newLane < 0)
        {
            Debug.Log("Hit");
            GetHit();
            return;
        }

        pastLane = currentLane;
        SideMove(newLane);
    }

    private void SideMove(float lineNumber)
    {
        currentLane = lineNumber;
        targetX = (lineNumber - 1) * laneWidth;

        if (sideMovesCoroutine != null)
            StopCoroutine(sideMovesCoroutine);

        sideMovesCoroutine = StartCoroutine(MoveOnRightAxis(targetX, 1.2f));
    }

    private IEnumerator MoveOnRightAxis(float xPosition, float speed)
    {
        float howFar = 0f;
        isChangingSide = true;

        while (howFar < 1f)
        {
            howFar += Time.deltaTime * speed;
            float newX = Mathf.Lerp(playerTransform.position.x, xPosition, howFar);
            Vector3 target = new Vector3(newX, playerTransform.position.y, playerTransform.position.z);
            characterController.Move(target - playerTransform.position);

            if (Mathf.Abs(playerTransform.position.x - xPosition) < 0.7f)
            {
                isChangingSide = false;
            }

            yield return null;
        }

        characterController.detectCollisions = true;
    }

    private void HandleOnRollEvent()
    {
        if (isRolling)
            return;

        if (IsInAir)
            airRoll = true;

        characterController.height /= 2f;
        characterController.center = colliderCenterOnRoll;

        animator.CrossFadeInFixedTime(animRollHash, animationFadeTime);
        StartCoroutine(RollCooldown());
    }

    private void HandleOnJumpEvent()
    {
        if (!IsGrounded) return;

        Jump(jumpForce);
        animator.SetBool(animBoolIsGrounded, false);
        animator.CrossFadeInFixedTime(animJumpHash, animationFadeTime / 2);
    }

    private void Jump(float jumpHeight)
    {
        verticalVelocity.y = Mathf.Sqrt(jumpHeight * (-2f) * gravity);
        isInAir = true;
    }

    private void HandleOnLanded()
    {
        animator.SetBool(animBoolIsGrounded, true);
    }

    private void HandleOnFall()
    {
        animator.SetBool(animBoolIsGrounded, false);
    }

    private void HandleOnRunSpeedChange(float speed)
    {
        animator.SetFloat(animRunHash, speed);
    }

    private IEnumerator RollCooldown()
    {
        isRolling = true;
        yield return rollCooldown;
        characterController.height = colliderHeightOnStart;
        characterController.center = colliderCenterOnStart;
        isRolling = false;
    }

    private async void SetDeath(Action<float, int> onDeathDone)
    {
        blockRunning = true;
        characterController.enabled = false;
        characterController.detectCollisions = false;

        inputReader.enabled = false;

        animator.CrossFadeInFixedTime(animDeadHash, animationFadeTime);

        await UniTask.Delay(1100);

        onDeathDone?.Invoke(transform.position.z, levelCoins);
    }

    private IObstacle ignoringObstacle = null;

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.TryGetComponent(out IObstacle obstacle))
        {
            Side side = DetermineCollisionSide(hit.normal);
            if (ignoringObstacle != obstacle)
            {
                switch (side)
                {
                    case Side.Left:
                        GetHit();
                        SideMove(pastLane);
                        break;
                    case Side.Right:
                        GetHit();
                        SideMove(pastLane);
                        break;
                    case Side.Other:
                        GetHit();
                        SideMove(pastLane);
                        break;
                    case Side.NaS:
                        GetHit();
                        SideMove(pastLane);
                        break;
                    case Side.HeadOn:
                        onGetDamage?.Invoke(100);
                        animator.CrossFadeInFixedTime(animDeadHash, animationFadeTime);
                        break;
                    case Side.TopOn:
                        ignoringObstacle = obstacle;
                        break;
                }
            }
        }
    }

    enum Side
    {
        Left,
        Right,
        HeadOn,
        TopOn,
        Other,
        NaS,
    }

    Side DetermineCollisionSide(Vector3 hitNormal)
    {
        if (Mathf.Approximately(hitNormal.x, 1))
        {
            return Side.Right;
        }
        else if (Mathf.Approximately(hitNormal.x, -1))
        {
            return Side.Left;
        }
        else if (Mathf.Approximately(hitNormal.y, 1))
        {
            return Side.TopOn;
        }
        else if (Mathf.Approximately(hitNormal.y, -1))
        {
            return Side.TopOn;
        }
        else if (Mathf.Approximately(hitNormal.z, 1))
        {
            return Side.Other;
        }
        else if (Mathf.Approximately(hitNormal.z, -1))
        {
            return Side.HeadOn;
        }

        return Side.NaS;
    }
    
    private void GetHit()
    {
        onGetDamage?.Invoke(1);
        animator.SetTrigger(animHitHash);
    }

    public void PlaySound(int index)
    {
        audioSource.PlayOneShot(sounds[index]);
    }

    public void SetDamageFX(bool enable)
    {
        if (enable)
        {
            damageParticles.Play();
        }
        else
        {
            damageParticles.Stop();
        }
    }

    private void OnDisable()
    {
        inputReader.OnGoLeftEvent -= HandleOnGoLeftEvent;
        inputReader.OnGoRightEvent -= HandleOnGoRightEvent;
        inputReader.OnRollEvent -= HandleOnRollEvent;
        inputReader.OnJumpEvent -= HandleOnJumpEvent;
        OnLanded -= HandleOnLanded;
        OnFall -= HandleOnFall;
    }
}