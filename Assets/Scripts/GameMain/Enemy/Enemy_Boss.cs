using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Boss : Enemy
{
    // 弾プレハブ
    [SerializeField] private GameObject _bulletPrefab;

    // 弾発射位置
    [SerializeField] private Transform _muzzlePos;

    // 弾のダメージ
    [SerializeField] private int _bulletDamage = 100;

    protected override void Update()
    {
        base.Update();

        if (!updateFlag) return;        
    }
}
