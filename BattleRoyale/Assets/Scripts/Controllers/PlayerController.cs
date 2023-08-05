using Constants;
using UnityEngine;

enum PlayerState
{
    STANDING,
    JUMPING,
    WALK_JUMPING,
    CROUCHING,
    SLIDING
}

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody playerRigidbody;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Camera userCamera;
    [SerializeField] private Animator animator;

    private PlayerState playerState;

    private const float JumpForce = 5f;
    private const float SensitivityMultiplier = 100f;
    private const float Y_RotationLimit = 40f;
    private readonly Vector3 cameraStandingPosition = new Vector3(0f, 1.6f, 0.123f);
    private readonly Vector3 cameraCrouchingPosition = new Vector3(0f, 1.6f, 0.123f);

    private Vector2 rotation;
    private Vector3 movement;

    private float verticalInput;
    private float horizontalInput;
    private float sensitivity;
    private float moveSpeed = 5f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        ChangeSensitivity(1f);

        playerState = PlayerState.STANDING;
    }

    private void Update()
    {
        Move();
        Rotate();
        Jump();
        Crouch();
    }

    private void FixedUpdate()
    {
        UpdateMovementAnimations();
    }

    public void Move()
    {
        if (PlayerCanMove())
        {
            verticalInput = Input.GetAxis("Vertical");
            horizontalInput = Input.GetAxis("Horizontal");

            movement = playerTransform.right * horizontalInput + playerTransform.forward * verticalInput;
            playerTransform.Translate(movement * moveSpeed * Time.deltaTime, Space.World);
        }
    }

    public void Rotate()
    {
        rotation.x += Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        rotation.y -= Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
        rotation.y = Mathf.Clamp(rotation.y, -Y_RotationLimit, Y_RotationLimit);
        
        userCamera.transform.rotation = Quaternion.Euler(rotation.y, rotation.x, 0f);
        playerTransform.rotation = Quaternion.Euler(0f, rotation.x, 0f);
    }

    public void Jump()
    {
        if (Input.GetKeyDown(PlayerControls.Key_Jump))
        {
            if (PlayerIsMoving())
            {
                playerState = PlayerState.WALK_JUMPING;
            }
            else
            {
                playerState = PlayerState.JUMPING;
            }

            playerRigidbody.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
            animator.SetBool(PlayerAnimatorParameters.Jump, true);
        }
    }

    public void Crouch()
    {
        if (Input.GetKeyDown(PlayerControls.Key_Crouch) && playerState == PlayerState.STANDING)
        {
            playerState = PlayerState.CROUCHING;

            animator.SetBool(PlayerAnimatorParameters.IsCrouching, true);
        }
        else if (Input.GetKeyDown(PlayerControls.Key_Crouch) && playerState == PlayerState.CROUCHING)
        {
            playerState = PlayerState.STANDING;

            animator.SetBool(PlayerAnimatorParameters.IsCrouching, false);
            animator.SetBool(PlayerAnimatorParameters.CrouchToStand, true);
        }

        if (playerState == PlayerState.CROUCHING && AnimationIsFinished())
        {
            animator.SetBool(PlayerAnimatorParameters.CrouchIdle, true);
        }
    }

    private void UpdateMovementAnimations()
    {
        float currentSpeed = Mathf.Clamp01(movement.magnitude);
        animator.SetFloat(PlayerAnimatorParameters.Speed, currentSpeed);
    }

    private bool AnimationIsFinished()
    {
        AnimatorStateInfo animationState = animator.GetCurrentAnimatorStateInfo(0);
        float animationTime = animationState.normalizedTime;

        return animationTime > 1.0f;
    }

    private void ChangeSensitivity(float newSensitivity)
    {
        sensitivity = newSensitivity * SensitivityMultiplier;
    }

    private bool PlayerCanMove()
    {
        return playerState == PlayerState.STANDING || playerState == PlayerState.CROUCHING || playerState == PlayerState.WALK_JUMPING;
    }

    private bool PlayerIsMoving()
    {
        return verticalInput > 0 || horizontalInput > 0;
    }

    private void SetPlayerState(PlayerState playerState)
    {
        this.playerState = playerState;

        if (playerState == PlayerState.STANDING)
        {
            userCamera.transform.position = cameraStandingPosition;
        }
        else if (playerState == PlayerState.CROUCHING)
        {
            userCamera.transform.position = cameraCrouchingPosition;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == Tags.Ground)
        {
            animator.SetBool(PlayerAnimatorParameters.Jump, false);
            playerState = PlayerState.STANDING;
        }
    }
}
