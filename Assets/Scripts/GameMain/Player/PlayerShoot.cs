using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    // 弾プレハブ
    [SerializeField] private GameObject _bulletPrefab;

    // ロックマンの弾発射位置
    [SerializeField] private Transform _muzzlePos;

    // ファイアレート
    [SerializeField] private float _fireRate = 0.2f;

    private float nextFireWaitTime;
    AnimParamController animParamController;

    private void Awake()
    {
        animParamController = GetComponent<AnimParamController>();
    }

    void Update()
    {
        if(InputManager.Instance.FireKey == 1 && nextFireWaitTime <= 0)
        {
            var obj = Instantiate(_bulletPrefab, _muzzlePos.position, Quaternion.identity) as GameObject;
            var bulletSc = obj.GetComponent<Bullet>();
            bulletSc.ShotBullet(5f, transform.right);
            nextFireWaitTime += _fireRate;
            animParamController.SetAnimParamBool("Shooting", true);
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
