using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    // 死亡エフェクト
    [SerializeField] private GameObject _diedEffect;

    // HPバー用のSlider
    [SerializeField] private Slider _HP_Bar;

    // 残機UIのText
    [SerializeField] private Text _zanki_UI;

    // 無敵時間
    [SerializeField] private float _invincibleTime = 3f;

    // 無敵時間
    private float invincibleWaitTime;
    public float InvincibleWaitTime {
        set { invincibleWaitTime = value; }
        get { return invincibleWaitTime; }
    }

    // 最大HP
    [SerializeField] private int maxHP = 100;
    public int MaxHP {
        get { return maxHP; }
        set { maxHP = value; }
    }

    // 現在HP
    private int hp = 100;
    public int HP {
        get { return hp; }
        set { hp = value; }
    }

    // ゲームクリアしたか
    public bool isCleared;

    private SpriteRenderer spriteRenderer;
    private AnimParamController animParamController;

    private void Awake()
    {
        // HP初期化
        hp = maxHP;
    }

    private void Start()
    {
        // 残機が0未満ならステージの最初からリスタート
        if(PlayerZankiManager.Instance.Zanki < 0)
        {
            PlayerZankiManager.Instance.ZankiReset();
            PlayerRestartPointManager.Instance.isUpdateRestartPos = false;
        }

        // 残機UIに残機数反映
        _zanki_UI.text = PlayerZankiManager.Instance.Zanki.ToString("D2");

        // 一度もリスタートポイント通ってなかったら初期位置上書き
        if (!PlayerRestartPointManager.Instance.isUpdateRestartPos)
        {
            PlayerRestartPointManager.Instance.RestartPoint = transform.position;
        }
        else
        {
            // リスタートポイント通ってたらそこから復帰
            transform.position = PlayerRestartPointManager.Instance.RestartPoint;

            // カメラ位置も調整
            CameraController cameraController = FindObjectOfType<CameraController>();
            Vector3 cameraPos = cameraController.Camera.WorldToViewportPoint(transform.position);
            cameraPos.y += 0.3f;
            cameraPos.z = -10f;
            cameraController.transform.position = cameraController.Camera.ViewportToWorldPoint(cameraPos);
        }
        
        spriteRenderer = GetComponent<SpriteRenderer>();
        animParamController = GetComponent<AnimParamController>();

        SoundManager.Instance.PlayBGM(BGM.Main, 6.5f);
        SoundManager.Instance.BGMVolume(0.4f);
    }

    private void FixedUpdate()
    {
        // 無敵時間中はプレイヤーを点滅させる
        if(0 < invincibleWaitTime)
        {
            float level = Mathf.Abs(Mathf.Sin(Time.time * 10));
            spriteRenderer.color = new Color(1f, 1f, 1f, level);
            invincibleWaitTime -= Time.deltaTime;

            // 無敵時間終わったら元に戻す
            if(invincibleWaitTime <= 0)
            {
                spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
            }
        }
    }

    // ダメージを受ける
    public void TakeDamage(int damage, bool ignoreInvincible = false)
    {
        if (isCleared) return;  // クリアしてたらダメージ無視

        if (HP <= 0) return;    // HP0以下にさせない

        if (0 < invincibleWaitTime && !ignoreInvincible) return;    // 無敵時間中かつ無敵時間無視攻撃でなければ早期リターン

        HP -= damage;

        if (HP <= 0)
        {
            HP = 0;
            animParamController.SetAnimParamBool("Died", true);
            StartCoroutine(nameof(DiedEffect));
        }
        else
        {
            invincibleWaitTime = _invincibleTime;
            animParamController.SetAnimParamTrigger("Damaged");
        }

        _HP_Bar.value = (float)HP / MaxHP;

    }

    // プレイヤー死亡時処理
    private IEnumerator DiedEffect()
    {
        GetComponent<PlayerController>().enabled = false;   // 操作停止
        GetComponent<PlayerShoot>().enabled = false;
        GetComponent<NGHMRigidbody>().enabled = false;      // 重力停止
        //Rigidbody2D rb = GetComponent<Rigidbody2D>();
        //rb.velocity = Vector3.zero;
        //rb.isKinematic = true;
        //Collider2D[] colliders = GetComponents<Collider2D>();
        //foreach(var cols in colliders)
        //{
        //cols.enabled = false;
        //}
        SoundManager.Instance.StopBGM();

        yield return new WaitForSeconds(0.75f);

        spriteRenderer.enabled = false;     // スプライト非表示
        Instantiate(_diedEffect, transform.position, Quaternion.identity);  // 死亡時エフェクト出す
        SoundManager.Instance.PlaySE(SE.Miss);

        yield return new WaitForSeconds(5f);

        // 残機減らしてリスタートポイントから復活
        PlayerZankiManager.Instance.Zanki--;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // ゲームクリア後、最初のシーンからリスタート
    public void GameRestart()
    {
        PlayerZankiManager.Instance.Zanki = -1;
        SceneManager.LoadScene(0);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Needle"))
        {
            TakeDamage(10);
        }
        else if (collision.gameObject.CompareTag("Lava"))
        {
            TakeDamage(maxHP, true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyBullet"))
        {
            Bullet bullet = collision.GetComponent<Bullet>();

            TakeDamage(bullet.Damage);
        }
        else if (collision.CompareTag("RestartPoint"))
        {
            PlayerRestartPointManager.Instance.RestartPoint = collision.transform.position;
            PlayerRestartPointManager.Instance.isUpdateRestartPos = true;
        }
    }

}
