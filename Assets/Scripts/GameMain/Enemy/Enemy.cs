using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // プレイヤーと衝突したときにダメージを与えるか
    private enum ConflictType
    {
        None,
        Damage
    }

    // 敵撃破時エフェクト
    [SerializeField] private GameObject _diedParticle;

    // SpriteCol
    [SerializeField] private SpriteCol _mySpriteCol;

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

    // 衝突タイプ
    [SerializeField] private ConflictType _conflictType = ConflictType.Damage;

    // プレイヤーに衝突したときに与えるダメージ
    [SerializeField] private int _conflictDamage = 5;

    // ロックオン範囲に入っているか
    private bool isEnteredLockonLange;

    // 前のフレームでロックオン範囲に入っていたか
    private bool lastEnteredLockonLange;

    // カメラ描画範囲に入っている敵を管理するマネージャー
    private WithinCameraLangeEnemyManager cameraLangeEnemyManager;

    // メインカメラ
    protected Camera mainCamera;

    // 更新処理してもいいか
    protected bool updateFlag;

    virtual protected void Awake()
    {
        // HPを初期化
        hp = maxHP;
    }

    virtual protected void Start()
    {
        // インスタンス取得
        cameraLangeEnemyManager = WithinCameraLangeEnemyManager.Instance;
        if (mainCamera == null)
        {
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        }

        // ステージ開始時自分のX座標がプレイヤーより左なら非アクティブにする
        Transform playerTrans = GameObject.FindGameObjectWithTag("Player").transform;
        if(transform.position.x < playerTrans.position.x)
        {
            gameObject.SetActive(false);
        }
    }

    virtual protected void Update()
    {
        updateFlag = true;

        // HP0以下なら更新しない
        if (HP <= 0) updateFlag = false;

        // スクリーン座標取得
        Vector3 screenPos = mainCamera.WorldToScreenPoint(transform.position);
        Rect enemyUpdateScreenLange = new Rect(-200, -200, Screen.width + 400, Screen.height + 400);

        // 敵が画面範囲から遠ざかっていたら処理をスキップ
        if (!enemyUpdateScreenLange.Contains(screenPos))
        {
            updateFlag = false;
        }

        if (!updateFlag) return;

        if (isEnteredLockonLange)
        {
            if (!lastEnteredLockonLange)
            {
                // 前のフレームでロックオン範囲に入っておらず、
                // 今のフレームで範囲に入ってたらロックオン可能リストに加える
                cameraLangeEnemyManager._withinCameraLangeEnemies.Add(this);
            }
            
        }
        else
        {
            if (lastEnteredLockonLange)
            {
                // ロックオン範囲から出たらロックオン可能リストから削除
                cameraLangeEnemyManager._withinCameraLangeEnemies.Remove(this);
            }
            
        }

        // プレイヤーの弾と当たり判定
        SpriteCol bulletCol = _mySpriteCol.HitCheck_PlayerBullet();
        if (bulletCol)
        {
            // 弾のコンポーネント取得
            Bullet bullet = bulletCol.transform.parent.GetComponent<Bullet>();
            // ダメージ受ける
            TakeDamage(bullet.Damage);
            // 弾を破壊
            Destroy(bulletCol.transform.parent.gameObject);
            // 弾ヒットSE再生
            SoundManager.Instance.PlaySE(SE.ShotHit);
        }

        // プレイヤーと衝突したとき、ダメージを与える敵ならダメージを与える
        if(_conflictType == ConflictType.Damage)
        {
            SpriteCol playerCol = _mySpriteCol.HitCheck_Player();
            if (playerCol)
            {
                PlayerHealth playerHealth = playerCol.GetComponentInParent<PlayerHealth>();

                // 無敵時間でないときに音再生
                if (playerHealth.InvincibleWaitTime <= 0 && 0 < playerHealth.HP)
                {
                    SoundManager.Instance.PlaySE(SE.Damage);
                }
                // プレイヤーにダメージ
                playerHealth.TakeDamage(_conflictDamage);
                
            }
        }
        
        // 今フレームでロックオン範囲に入ってるか記憶
        lastEnteredLockonLange = isEnteredLockonLange;
        isEnteredLockonLange = false;
    }

    private void OnWillRenderObject()
    {
        if (HP <= 0) return;

        if (cameraLangeEnemyManager == null)
        {
            cameraLangeEnemyManager = WithinCameraLangeEnemyManager.Instance;
        }

        if (mainCamera == null)
        {
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        }

        // カメラの描画範囲内に収まっていて、ロックオン範囲内にも収まっていたらフラグ立てる
        Vector3 screenPos = mainCamera.WorldToScreenPoint(transform.position);

        if (cameraLangeEnemyManager.LockonLangeRect.Contains(screenPos)){
            isEnteredLockonLange = true;
        }
        
    }

    // ダメージを受ける
    public void TakeDamage(int damage)
    {
        if (HP <= 0) return;

        HP -= damage;

        // 撃破時
        if (HP <= 0)
        {
            HP = 0;
            // ロックオン可能リストから削除
            cameraLangeEnemyManager._withinCameraLangeEnemies.Remove(this);
            // 撃破時エフェクト出す
            Instantiate(_diedParticle, transform.position, Quaternion.identity);
            // 自分デストロイ
            Destroy(gameObject);
        }

    }

    virtual protected void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if(_conflictType == ConflictType.Damage)
            {
                PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();

                playerHealth.TakeDamage(_conflictDamage);
            }
        }
        else if (collision.gameObject.CompareTag("Lava"))
        {
            TakeDamage(maxHP);
        }
    }

    virtual protected void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("当たった");
        if (collision.CompareTag("Bullet"))
        {
            // プレイヤーの弾と衝突
            Bullet bullet = collision.gameObject.GetComponent<Bullet>();

            TakeDamage(bullet.Damage);
        }
    }

    
}
