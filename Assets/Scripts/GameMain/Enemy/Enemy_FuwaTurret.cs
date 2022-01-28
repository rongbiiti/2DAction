using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_FuwaTurret : Enemy
{
    // �e�v���n�u
    [SerializeField] private GameObject _bulletPrefab;

    // �e���ˈʒu
    [SerializeField] private Transform _muzzlePos;

    // �e�̃_���[�W
    [SerializeField] private int _bulletDamage = 100;

    // �t�@�C�A���[�g
    [SerializeField] private float _fireRate = 0.2f;

    // �㉺�ɂ����h��鑬�x
    [SerializeField] private float _floatingSpeedY = 2f;

    // �㉺�ɗh��鋗��
    [SerializeField] private float _floatingRangeY = 0.4f;

    private Vector3 startPosition;
    private float nextFireWaitTime;

    protected override void Start()
    {
        base.Start();
        startPosition = transform.position;

    }

    protected override void Update()
    {
        base.Update();

        if (!updateFlag) return;

        // �㉺�ɗh���
        transform.position = new Vector3(startPosition.x, (Mathf.Sin((Time.time) * _floatingSpeedY) * _floatingRangeY + startPosition.y), startPosition.z);

        // �e����
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
