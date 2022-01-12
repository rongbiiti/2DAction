using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    // HP�o�[�p��Slider
    [SerializeField] private Slider _HP_Bar;

    // �c�@�ő吔
    [SerializeField] private int _maxZanki = 99;

    // �����c�@
    [SerializeField] private int _startZanki = 3;

    private int zanki = 3;
    public int Zanki {
        get { return zanki; }
        set { zanki = value; }
    }

    // �ő�HP
    [SerializeField] private int maxHP = 100;
    public int MaxHP {
        get { return maxHP; }
        set { maxHP = value; }
    }

    private int hp = 100;
    public int HP {
        get { return hp; }
        set { hp = value; }
    }

    private void Awake()
    {
        hp = maxHP;
    }

    public void TakeDamage(int damage)
    {
        if (HP <= 0) return;

        HP -= damage;

        if (HP <= 0)
        {
            HP = 0;
        }

        _HP_Bar.value = (float)HP / MaxHP;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyBullet"))
        {
            Bullet bullet = collision.GetComponent<Bullet>();

            TakeDamage(bullet.Damage);
        }
    }
}
