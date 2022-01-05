using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    private Vector2 localScale;
    private bool isGrounded = true;
    private bool isJumping = false;
    private bool isJumpingCheck = true;
    private float jumpTimeCounter;
    private float jumpTime = 0.35f;
    private float _jumpPower;
    [SerializeField] private LayerMask platformLayer;
    [SerializeField] private float testY = 0.55f;
    public GUIStyle textStyle;


    InputManager inputManager;
    PlayerManager playerManager;

    void Awake()
    {
        localScale = transform.localScale;
        jumpTimeCounter = jumpTime;
    }

    void Start()
    {
        playerManager = PlayerManager.Instance;
        inputManager = InputManager.Instance;
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(50, 50, 50, 50), "isGrounded" + isGrounded, textStyle);
    }

    void Update()
    {
        isGrounded = Physics2D.Linecast(transform.position - transform.up * 0.1f, transform.position - transform.up * testY, platformLayer);
        Debug.DrawLine(transform.position - transform.up * 0.1f, transform.position - transform.up * testY, Color.red, 0, false);
    }

    void FixedUpdate()
    {
        if (inputManager.MoveKey != 0)
        {
            // ������ς���
            localScale.x = inputManager.MoveKey * localScale.x;
            transform.localScale = localScale;
        }

        if (isGrounded)
        {
            rb.velocity = new Vector2(inputManager.MoveKey * playerManager.MoveSpeed, rb.velocity.y);

            if (isJumpingCheck && inputManager.JumpKey != 0)
            {
                jumpTimeCounter = jumpTime;
                isJumpingCheck = false;
                isJumping = true;
                _jumpPower = playerManager.JumpPower;
            }
        }
        else
        {
            if (inputManager.JumpKey == 0)
            {
                isJumping = false;
            }
            if (!isJumping)
            {
                rb.velocity = new Vector2(inputManager.MoveKey * playerManager.JumpMoveSpeed, Physics.gravity.y * playerManager.GravityRate);
            }
        }

        if (isJumping)
        {
            jumpTimeCounter -= Time.deltaTime;

            if (inputManager.JumpKey == 2)
            {
                _jumpPower -= 0.2f;
                rb.velocity = new Vector2(inputManager.MoveKey * playerManager.JumpMoveSpeed, 1 * _jumpPower);
            }
            if (jumpTimeCounter < 0)
            {
                isJumping = false;
            }
        }

        if (inputManager.JumpKey == 0)
        {
            isJumpingCheck = true;
        }
    }
}