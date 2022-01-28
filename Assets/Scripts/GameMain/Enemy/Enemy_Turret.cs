using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Turret : Enemy
{
    // 弾プレハブ
    [SerializeField] private GameObject _bulletPrefab;

    // 弾発射位置
    [SerializeField] private Transform _muzzlePos;

    // 弾のダメージ
    [SerializeField] private int _bulletDamage = 100;

    // ファイアレート
    [SerializeField] private float _fireRate = 0.2f;

    private float nextFireWaitTime;

    protected override void Update()
    {
        base.Update();

        if (!updateFlag) return;

        // 弾発射
        if (nextFireWaitTime <= 0)
        {
            Vector3 vec = -transform.right * transform.localScale.x;
            GameObject obj = Instantiate(_bulletPrefab, _muzzlePos.position, Quaternion.identity) as GameObject;
            Bullet bulletSc = obj.GetComponent<Bullet>();

            bulletSc.ShotBullet(_bulletDamage, 3.5f, vec);
            nextFireWaitTime += _fireRate;
            SoundManager.Instance.PlaySE(SE.EnemyShot);
        }
        
    }

    private void FixedUpdate()
    {
        if (HP <= 0) return;

        if (0 < nextFireWaitTime)
        {
            nextFireWaitTime -= Time.deltaTime;
        }
    }
}
