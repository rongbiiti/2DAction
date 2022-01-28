using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Kuribo : Enemy
{
    // 歩行速度
    [Space(10f)]
    [SerializeField] private float _moveSpeed = 5f;

    // 壁接触判定SpriteCol
    [SerializeField] private SpriteCol _wallSpriteCol;
    private NGHMRigidbody rb;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<NGHMRigidbody>();
    }

    protected override void Update()
    {
        base.Update();

        // 更新できないときはRigidBodyも止める
        rb.enabled = updateFlag;

        if (!updateFlag) return;

        SpriteCol hitWallCol = _wallSpriteCol.HitCheck_Ground();
        if (hitWallCol)
        {
            // 溶岩と当たったらダメージ受ける
            if (hitWallCol.CompareTag("Lava"))
            {
                TakeDamage(MaxHP);
                SoundManager.Instance.PlaySE(SE.MagmaDive);
                return;
            }

            // 壁と当たったら向き反転
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }

        rb.Velocity = new Vector3(-transform.localScale.x * _moveSpeed, rb.Velocity.y);

    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (HP <= 0) return;

        base.OnTriggerEnter2D(collision);
        if (collision.CompareTag("Ground") || collision.CompareTag("Enemy") || collision.CompareTag("RidableEnemy"))
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
    }
}
