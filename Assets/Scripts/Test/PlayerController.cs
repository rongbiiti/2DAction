using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private NGHMRigidbody rb;
    [SerializeField] private SpriteCol _mySpriteCol;
    [SerializeField] private SpriteCol _groundCheckSpriteCol;
    [SerializeField] private SpriteCol _wallCheckSpriteCol;
    [SerializeField] private SpriteCol _ceilCheckSpriteCol;
    private Vector2 localScale;
    private float startLocalScaleX;
    private bool isGrounded = true;
    private bool lastGrounded = true;
    private bool isCeil = false;
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
    PlayerHealth playerHealth;
    AnimParamController animParamController;

    void Awake()
    {
        startLocalScaleX = transform.localScale.x;
        localScale = transform.localScale;
        jumpTimeCounter = jumpTime;
        animParamController = GetComponent<AnimParamController>();
        playerHealth = GetComponent<PlayerHealth>();
    }

    void Start()
    {
        playerManager = PlayerManager.Instance;
        inputManager = InputManager.Instance;

        rb = GetComponent<NGHMRigidbody>();

        // nullだったら取得
        if(_mySpriteCol == null)
        {
            _mySpriteCol = GetComponentInChildren<SpriteCol>();
        }

       
    }

    void Update()
    {
        //isGrounded = Physics2D.Linecast(transform.position - transform.up * 0.1f, transform.position - transform.up * testY, platformLayer);
        //Debug.DrawLine(transform.position - transform.up * 0.1f, transform.position - transform.up * testY, Color.red, 0, false);

        isGrounded = false;
        isCeil = false;


        // 地形と当たってるか判定
        SpriteCol hitGroundCol = _groundCheckSpriteCol.HitCheck_Ground();

        if (hitGroundCol)
        {
            if (hitGroundCol && hitGroundCol.CompareTag("Lava"))
            {
                playerHealth.TakeDamage(playerHealth.MaxHP, true);
                SoundManager.Instance.PlaySE(SE.MagmaDive);
                return;
            }
            isGrounded = true;
            if (!lastGrounded)
            {
                SoundManager.Instance.PlaySE(SE.Landing);
            }
        }

        // 天井と当たってるか判定
        SpriteCol hitCeilCol = _ceilCheckSpriteCol.HitCheck_Ground();

        if(hitCeilCol)
        {
            isCeil = true;
            _jumpPower = 0;
            isJumping = false;
        }

        // 敵の弾と当たってるか判定
        SpriteCol hitBulletCol = _mySpriteCol.HitCheck_EnemyBullet();

        if (hitBulletCol)
        {
            Bullet bullet = hitBulletCol.transform.parent.GetComponent<Bullet>();

            playerHealth.TakeDamage(bullet.Damage);

            Destroy(hitBulletCol.transform.parent.gameObject);

            SoundManager.Instance.PlaySE(SE.Damage);
        }

        // 特殊効果付きの地形とキャラのColが当たってるか判定
        SpriteCol hitCol = _mySpriteCol.HitCheck_Ground();

        if (hitCol)
        {
            if (hitCol.CompareTag("Lava"))
            {
                playerHealth.TakeDamage(playerHealth.MaxHP, true);
                SoundManager.Instance.PlaySE(SE.MagmaDive);
            }
            else if (hitCol.CompareTag("Needle"))
            {
                playerHealth.TakeDamage(playerHealth.MaxHP);

                // 無敵時間でないときに音再生
                if(playerHealth.InvincibleWaitTime <= 0)
                {
                    SoundManager.Instance.PlaySE(SE.Needle);
                }
                
            }
            
        }

        // トリガーと当たってるか判定
        SpriteCol trigger = _mySpriteCol.HitCheck_Trigger();

        if(trigger)
        {
            if (trigger.CompareTag("FallZone"))
            {
                isFalling = true;
            }
            else if (trigger.CompareTag("RestartPoint"))
            {
                PlayerRestartPointManager.Instance.RestartPoint = trigger.transform.parent.position;
                PlayerRestartPointManager.Instance.isUpdateRestartPos = true;
            }
            else if (trigger.CompareTag("BossRoomShutter"))
            {
                trigger.gameObject.tag = "Untagged";
                StartCoroutine(EnterBossRoom(trigger.GetComponentInParent<SpriteCol>(), trigger.GetComponentInParent<BossRoomShutter>()));
                trigger.RemoveColManagerList();
                trigger.gameObject.layer = 6;
                trigger.AddColManagerList();
            }
        }


        // アニメーターのパラメーターセット
        animParamController.SetAnimParamBool("Jumping", !isGrounded);

        lastGrounded = isGrounded;
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
            rb.Velocity = new Vector2(inputManager.MoveKey * (playerManager.MoveSpeed + (playerManager.DashSpeed * inputManager.DashKey)) * moveFlag, rb.Velocity.y);

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
                rb.Velocity = new Vector2(inputManager.MoveKey * (playerManager.JumpMoveSpeed + (playerManager.DashSpeed * inputManager.DashKey)) * moveFlag, Physics.gravity.y * playerManager.GravityRate);
            }
        }

        if (isJumping)
        {
            jumpTimeCounter -= Time.deltaTime;

            _jumpPower -= 0.6f;
            rb.Velocity = new Vector2(inputManager.MoveKey * (playerManager.JumpMoveSpeed + (playerManager.DashSpeed * inputManager.DashKey)) * moveFlag, 1 * _jumpPower);

            if (jumpTimeCounter < 0 || isCeil)
            {
                isJumping = false;
            }
        }

        if (inputManager.JumpKey == 0)
        {
            isJumpingCheck = true;
        }
        
    }

    private IEnumerator EnterBossRoom(SpriteCol shutterCol, BossRoomShutter bossRoomShutter)
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
            //collision.gameObject.tag = "Untagged";
            //StartCoroutine(EnterBossRoom(collision.gameObject.GetComponent<BoxCollider2D>(), collision.gameObject.GetComponent<BossRoomShutter>()) );
        }
    }
}