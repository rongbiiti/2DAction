using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    private Vector2 localScale;
    private float startLocalScaleX;
    private bool isGrounded = true;
    private bool isJumping = false;
    private bool isJumpingCheck = true;
    private float jumpTimeCounter;
    private float jumpTime = 0.35f;
    private float _jumpPower;
    private float moveFlag = 1f;
    [SerializeField] private LayerMask platformLayer;
    [SerializeField] private float testY = 0.55f;
    public GUIStyle textStyle;

    private bool isFalling;
    public bool IsFalling {
        get { return isFalling; }
    }

    InputManager inputManager;
    PlayerManager playerManager;
    AnimParamController animParamController;

    void Awake()
    {
        startLocalScaleX = transform.localScale.x;
        localScale = transform.localScale;
        jumpTimeCounter = jumpTime;
        animParamController = GetComponent<AnimParamController>();
    }

    void Start()
    {
        playerManager = PlayerManager.Instance;
        inputManager = InputManager.Instance;
    }

    void Update()
    {
        isGrounded = Physics2D.Linecast(transform.position - transform.up * 0.1f, transform.position - transform.up * testY, platformLayer);
        Debug.DrawLine(transform.position - transform.up * 0.1f, transform.position - transform.up * testY, Color.red, 0, false);

        // アニメーターのパラメーターセット
        animParamController.SetAnimParamBool("Jumping", !isGrounded);
    }

    void FixedUpdate()
    {
        if (inputManager.MoveKey != 0)
        {
            // 向きを変える
            localScale.x = inputManager.MoveKey * startLocalScaleX;
            transform.localScale = localScale;

            // アニメーターのパラメーターセット
            animParamController.SetAnimParamBool("Move", true);
        }
        else
        {
            // アニメーターのパラメーターセット
            animParamController.SetAnimParamBool("Move", false);
        }

        if (isGrounded)
        {
            rb.velocity = new Vector2(inputManager.MoveKey * (playerManager.MoveSpeed + (playerManager.DashSpeed * inputManager.DashKey)) * moveFlag, rb.velocity.y);

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
                //isJumping = false;
            }

            if (!isJumping)
            {
                rb.velocity = new Vector2(inputManager.MoveKey * (playerManager.JumpMoveSpeed + (playerManager.DashSpeed * inputManager.DashKey)) * moveFlag, Physics.gravity.y * playerManager.GravityRate);
            }
        }

        if (isJumping)
        {
            jumpTimeCounter -= Time.deltaTime;

            _jumpPower -= 0.6f;
            rb.velocity = new Vector2(inputManager.MoveKey * (playerManager.JumpMoveSpeed + (playerManager.DashSpeed * inputManager.DashKey)) * moveFlag, 1 * _jumpPower);

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

    private IEnumerator EnterBossRoom(BoxCollider2D shutterCol, BossRoomShutter bossRoomShutter)
    {
        float waitTime = 0;
        Vector3 startPos = transform.position;
        Vector3 endPos = new Vector3(transform.position.x + 2.5f, transform.position.y, transform.position.z);

        shutterCol.enabled = false;
        moveFlag = 0f;
        bossRoomShutter.ShutterOff();
        inputManager.enabled = false;
        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(1f);

        Time.timeScale = 1f;

        FindObjectOfType<CameraController>().SetCameraPosToBossBattle();
        while(waitTime < 1.5f)
        {
            transform.position = Vector3.Lerp(startPos, endPos, waitTime);
            waitTime += Time.unscaledDeltaTime;
            yield return null;
        }

        Time.timeScale = 0f;

        shutterCol.enabled = true;
        transform.position = endPos;
        bossRoomShutter.ShutterOn();

        yield return new WaitForSecondsRealtime(1f);

        Time.timeScale = 1f;
        moveFlag = 1f;
        inputManager.enabled = true;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("FallZone"))
        {
            isFalling = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("BossRoomShutter"))
        {
            collision.gameObject.tag = "Untagged";
            StartCoroutine(EnterBossRoom(collision.gameObject.GetComponent<BoxCollider2D>(), collision.gameObject.GetComponent<BossRoomShutter>()) );
        }
    }
}