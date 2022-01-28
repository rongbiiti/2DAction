using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // 弾のダメージ
    private int damage;
    public int Damage {
        get { return damage; }
    }

    private NGHMRigidbody rb;
    private SpriteCol mySpriteCol;

    /// <summary>
    /// 弾のパラメーター設定して発射
    /// </summary>
    /// <param name="damage">与ダメージ</param>
    /// <param name="speed">速度</param>
    /// <param name="direction">発射方向</param>
    public void ShotBullet(int damage, float speed, Vector3 direction)
    {
        this.damage = damage;
        mySpriteCol = GetComponentInChildren<SpriteCol>();
        rb = GetComponent<NGHMRigidbody>();
        rb.Velocity = direction * speed;
        Destroy(gameObject, 5f);
    }

    private void Update()
    {
        // 地形と当たってるか判定
        SpriteCol hitCol = mySpriteCol.HitCheck_Ground();
        if (hitCol)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy(gameObject);
    }
}
