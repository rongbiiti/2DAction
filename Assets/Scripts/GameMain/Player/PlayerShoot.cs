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

    private float nextFireWaitTime;
    private WithinCameraLangeEnemyManager cameraLangeEnemyManager;
    private AnimParamController animParamController;
    private Enemy targetingEnemy;
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
        targetingEnemy = cameraLangeEnemyManager.GetNearestEnemy(transform.position);

        if (targetingEnemy != null)
        {
            _lockonCursor.SetImageEnabled(true, targetingEnemy.transform);

        }
        else
        {
            _lockonCursor.SetImageEnabled(false, null);

        }

        if (InputManager.Instance.FireKey != 0 && nextFireWaitTime <= 0)
        {
            Vector3 vec = _muzzlePos.right * transform.localScale.x;

            if (targetingEnemy != null)
            {
                vec = (targetingEnemy.transform.position - _muzzlePos.position).normalized;
                // 狙ってる敵が自分より右側のとき
                if (transform.position.x <= targetingEnemy.transform.position.x)
                {
                    transform.localScale = new Vector3(startLocalScaleX, transform.localScale.y, transform.localScale.z);
                }
                // 自分より左側のとき
                else
                {
                    transform.localScale = new Vector3(-startLocalScaleX, transform.localScale.y, transform.localScale.z);
                }
            }

            GameObject obj = Instantiate(_bulletPrefab, _muzzlePos.position, Quaternion.identity) as GameObject;
            Bullet bulletSc = obj.GetComponent<Bullet>();
            
            bulletSc.ShotBullet( _bulletDamage, 10f, vec);
            nextFireWaitTime += _fireRate;
            animParamController.SetAnimParamBool("Shooting", true);
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
