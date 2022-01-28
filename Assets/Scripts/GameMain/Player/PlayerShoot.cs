using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    // 弾プレハブ
    [SerializeField] private GameObject _bulletPrefab;

    // ロックマンの弾発射位置
    [SerializeField] private Transform _muzzlePos;

    // ロックオンカーソル
    [SerializeField] private LockonCursor _lockonCursor;

    // 弾のダメージ
    [SerializeField] private int _bulletDamage = 100;

    // ファイアレート
    [SerializeField] private float _fireRate = 0.2f;

    // 次の発射までのクールダウン
    private float nextFireWaitTime;
    // ロックオン可能リスト管理マネージャー
    private WithinCameraLangeEnemyManager cameraLangeEnemyManager;
    // アニメーションパラメーターコントローラー
    private AnimParamController animParamController;
    // 狙ってる敵
    private Enemy targetingEnemy;
    // 開始時のScale.x（横幅）
    private float startLocalScaleX;

    private void Awake()
    {
        animParamController = GetComponent<AnimParamController>();
        startLocalScaleX = transform.localScale.x;
    }

    private void Start()
    {
        cameraLangeEnemyManager = WithinCameraLangeEnemyManager.Instance;
        if(_lockonCursor == null)
        {
            _lockonCursor = FindObjectOfType<LockonCursor>();
        }
    }

    void Update()
    {
        // 一番近くにいるロックオン可能な敵を取得
        targetingEnemy = cameraLangeEnemyManager.GetNearestEnemy(transform.position);

        if (targetingEnemy != null)
        {
            // 敵がいたらロックオンカーソル表示
            _lockonCursor.SetImageEnabled(true, targetingEnemy.transform);
        }
        else
        {
            // いなかったら非表示
            _lockonCursor.SetImageEnabled(false, null);
        }

        // 発射ボタン押したとき、発射クールダウン終わってたら
        if (InputManager.Instance.FireKey != 0 && nextFireWaitTime <= 0)
        {
            // 向いてる方向のベクトル
            Vector3 vec = _muzzlePos.right * transform.localScale.x;

            if (targetingEnemy != null)
            {
                // 銃口から敵までの方向ベクトル作成
                vec = (targetingEnemy.transform.position - _muzzlePos.position).normalized;

                // 狙ってる敵が自分より右側のとき
                if (transform.position.x <= targetingEnemy.transform.position.x)
                {
                    // 右向く
                    transform.localScale = new Vector3(startLocalScaleX, transform.localScale.y, transform.localScale.z);
                }
                // 自分より左側のとき
                else
                {
                    // 左向く
                    transform.localScale = new Vector3(-startLocalScaleX, transform.localScale.y, transform.localScale.z);
                }
            }

            // 弾発射
            GameObject obj = Instantiate(_bulletPrefab, _muzzlePos.position, Quaternion.identity) as GameObject;
            Bullet bulletSc = obj.GetComponent<Bullet>();
            
            bulletSc.ShotBullet( _bulletDamage, 10f, vec);
            nextFireWaitTime += _fireRate;
            animParamController.SetAnimParamBool("Shooting", true);

            SoundManager.Instance.PlaySE(SE.Shot);
        }
        
    }

    private void LateUpdate()
    {
        // 狙ってる敵がいるかつ射撃モーション中
        if (targetingEnemy != null && _fireRate - 0.05f < nextFireWaitTime && nextFireWaitTime <= _fireRate)
        {
            // 狙ってる敵が自分より右側のとき
            if (transform.position.x <= targetingEnemy.transform.position.x)
            {
                transform.localScale = new Vector3( startLocalScaleX, transform.localScale.y, transform.localScale.z);
            }
            // 自分より左側のとき
            else
            {
                transform.localScale = new Vector3( -startLocalScaleX, transform.localScale.y, transform.localScale.z);
            }
        }
    }

    private void FixedUpdate()
    {
        if(0 < nextFireWaitTime)
        {
            nextFireWaitTime -= Time.deltaTime;
            if(nextFireWaitTime <= 0)
            {
                animParamController.SetAnimParamBool("Shooting", false);
            }
        }
    }

    
}
