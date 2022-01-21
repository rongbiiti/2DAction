using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Kuribo : Enemy
{
    // ï‡çsë¨ìx
    [Space(10f)]
    [SerializeField] private float _moveSpeed = 5f;
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

        rb.enabled = updateFlag;

        if (!updateFlag) return;

        SpriteCol hitWallCol = _wallSpriteCol.HitCheck_Ground();
        if (hitWallCol)
        {
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
