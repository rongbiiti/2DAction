using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private int damage;
    public int Damage {
        get { return damage; }
    }

    private NGHMRigidbody rb;                           // Rigidbody2Dコンポーネント
    private SpriteCol mySpriteCol;
    private bool isInit;


    public void ShotBullet(int damage, float speed, Vector3 direction)
    {
        this.damage = damage;
        mySpriteCol = GetComponentInChildren<SpriteCol>();
        rb = GetComponent<NGHMRigidbody>();
        rb.Velocity = direction * speed;
        Destroy(gameObject, 5f);

        //isInit = true;
    }

    private void Update()
    {
        //if (!isInit) return;

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
