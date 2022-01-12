using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Kuribo : Enemy
{
    // ï‡çsë¨ìx
    [SerializeField] private float _moveSpeed = 5f;

    protected override void Update()
    {
        base.Update();

        if (!updateFlag) return;

        transform.Translate(new Vector3(-transform.localScale.x, 0, 0) * _moveSpeed);
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
