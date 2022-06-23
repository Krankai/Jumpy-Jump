using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool IsActive { get; set; }
    public float RangeX { get; private set; }

    [SerializeField]
    float speed = 10.0f;

    [SerializeField]
    //float gravityModifier = 2.5f;
    Vector3 gravity = new Vector3(0f, -24.5f, 0f);

    [SerializeField]
    float jumpForce = 14.0f;

    [SerializeField]
    float doubleJumpForce = 14.0f;

    bool isOnGround = true;
    bool isDoubleJumpUsed = false;
    bool isInvulnerable = false;

    Rigidbody playerRb;
    Animator animator;
    GameManager gameManager;
    UIManager uiManager;

    Quaternion originalRotation;
    Vector3 originalRelativePosition;
    Vector3 originalScale;

    Vector3 vector3Up = Vector3.up;
    Vector3 vector3Zero = Vector3.zero;
    Vector3 vector3Right = Vector3.right;
    Vector3 moveLeftRotation = new Vector3(0, 90, 0);
    Vector3 moveRightRotation = new Vector3(0, 270, 0);

    private void Awake()
    {
        SetupBoundaries();
    }

    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        //Physics.gravity *= gravityModifier;
        Physics.gravity = gravity;

        animator = GetComponentInChildren<Animator>();

        originalRotation = transform.rotation;
        originalRelativePosition = animator.gameObject.transform.localPosition;
        originalScale = transform.localScale;

        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        uiManager = GameObject.Find("UI Manager").GetComponent<UIManager>();

        ToggleOnOff(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (IsActive && !gameManager.IsGameOver)
        {
            JumpDetection();
        }
    }

    private void FixedUpdate()
    {
        if (IsActive && !gameManager.IsGameOver)
        {
            MovePlayerPhysicsBody();
        }
    }

    private void LateUpdate()
    {
        if (IsActive)
        {
            ResetPlayerRelativePosition();
            if (!gameManager.IsGameOver)
            {
                ConstraintPlayerPosition();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isOnGround = true;
            isDoubleJumpUsed = false;
        }
    }

    // Move player horizontally via its physics body instead of transformation
    private void MovePlayerPhysicsBody()
    {
        var horizontalInput = Input.GetAxis("Horizontal");
        if (horizontalInput != 0)
        {
            var isMovingLeft = horizontalInput < 0;
            var targetRotationY = isMovingLeft ? moveLeftRotation.y : moveRightRotation.y;

            // Change facing direction of player's model
            if (transform.rotation.eulerAngles.y != targetRotationY)
            {
                transform.Rotate(isMovingLeft ? moveLeftRotation : moveRightRotation);
            }

            // Move player's rigid body
            playerRb.MovePosition(transform.position + vector3Right * speed * horizontalInput * Time.deltaTime);
            MoveAnimation(isMovingLeft);
        }
        else
        {
            transform.rotation = originalRotation;
            IdleAnimation();
        }
    }

    // Make player jump (or double jump) via space bar and/or up arrow key
    private void JumpDetection()
    {
        var jumpSignal = Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow);
        if (jumpSignal && isOnGround)
        {
            isOnGround = false;

            playerRb.AddForce(vector3Up * jumpForce, ForceMode.Impulse);

            JumpAnimation(false);
        }
        else if (jumpSignal && !isDoubleJumpUsed)
        {
            isDoubleJumpUsed = true;

            playerRb.velocity = playerRb.angularVelocity = vector3Zero;            // clear currently applied force
            playerRb.AddForce(vector3Up * doubleJumpForce, ForceMode.Impulse);

            JumpAnimation(true);
        }
    }

    // Prevent player from leaving the left and right side of the screen
    private void ConstraintPlayerPosition()
    {
        if (transform.position.x < -RangeX)
        {
            transform.position = new Vector3(-RangeX, transform.position.y, transform.position.z);
        }
        else if (transform.position.x > RangeX)
        {
            transform.position = new Vector3(RangeX, transform.position.y, transform.position.z);
        }
    }

    // Execute moving animation based on player's facing direction
    private void MoveAnimation(bool isMovingLeft)
    {
        animator.SetFloat("Speed_f", 1f);
    }

    // Execute idle animation (i.e. when no input from player is registered)
    private void IdleAnimation()
    {
        animator.SetFloat("Speed_f", 0f);
    }

    // Execute jump (and double-jump) animation
    private void JumpAnimation(bool isDoubleJump)
    {
        // For double-jump animation: cancel current animation and execute another immediately
        if (isDoubleJump)
        {
            animator.Rebind();                  // reset to the original state (animation clip)
            animator.Play("Running_Jump");
        }
        else
        {
            animator.SetTrigger("Jump_trig");
        }
    }

    // Reset relative position between actual player's mesh and player object
    private void ResetPlayerRelativePosition()
    {
        if (animator.gameObject.transform.localPosition != originalRelativePosition)
        {
            animator.gameObject.transform.localPosition = originalRelativePosition;
        }
    }

    // Call when player gets hit
    public void OnHit()
    {
        if (gameManager.IsGameOver) return;
        if (isInvulnerable) return;

        isInvulnerable = true;
        uiManager.UpdateLiveText(--gameManager.lives);

        if (gameManager.lives <= 0)
        {
            DieAnimation();
            gameManager.EndGame();
        }
        else
        {
            // Flicker animation
            StartCoroutine(FlickerRoutine());
        }
    }

    // Routinely 'flicker' player sprite
    IEnumerator FlickerRoutine()
    {
        Vector3 originalScale = transform.GetChild(0).transform.localScale;
        Transform modelTransform = transform.GetChild(0).transform;
        for (int i = 0; i < 10; ++i)
        {
            modelTransform.localScale = vector3Zero;
            yield return new WaitForSeconds(0.1f);

            modelTransform.localScale = originalScale;
            yield return new WaitForSeconds(0.1f);
        }

        isInvulnerable = false;
    }

    // Execute one of many death animation randomly
    private void DieAnimation()
    {
        animator.SetBool("Death_b", true);
        animator.SetInteger("DeathType_int", Random.Range(1, 3));
    }

    // Toggle player's visibility
    public void ToggleOnOff(bool isOn)
    {
        transform.localScale = isOn ? originalScale : Vector3.zero;
        playerRb.useGravity = isOn;
    }

    // Determine X boundaries based on viewport size
    private void SetupBoundaries()
    {
        Vector3 worldBoundaryPoint = Camera.main.ViewportToWorldPoint(Vector3.right);
        RangeX = worldBoundaryPoint.x - GetComponent<BoxCollider>().size.x / 2;
    }
}
