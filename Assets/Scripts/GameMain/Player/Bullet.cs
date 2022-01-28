using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // �e�̃_���[�W
    private int damage;
    public int Damage {
        get { return damage; }
    }

    private NGHMRigidbody rb;
    private SpriteCol mySpriteCol;

    /// <summary>
    /// �e�̃p�����[�^�[�ݒ肵�Ĕ���
    /// </summary>
    /// <param name="damage">�^�_���[�W</param>
    /// <param name="speed">���x</param>
    /// <param name="direction">���˕���</param>
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
        // �n�`�Ɠ������Ă邩����
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
