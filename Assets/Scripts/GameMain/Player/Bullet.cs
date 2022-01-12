using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private int damage;
    public int Damage {
        get { return damage; }
    }

    private Rigidbody2D rb;                           // Rigidbody2Dコンポーネント

    public void ShotBullet(int damage, float speed, Vector3 direction)
    {
        this.damage = damage;
        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(direction * speed, ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        Destroy(gameObject);
    }
}
