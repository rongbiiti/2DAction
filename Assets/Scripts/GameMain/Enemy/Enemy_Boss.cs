using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Boss : Enemy
{
    // �e�v���n�u
    [SerializeField] private GameObject _bulletPrefab;

    // �e���ˈʒu
    [SerializeField] private Transform _muzzlePos;

    // �e�̃_���[�W
    [SerializeField] private int _bulletDamage = 100;

    protected override void Update()
    {
        base.Update();

        if (!updateFlag) return;        
    }
}
