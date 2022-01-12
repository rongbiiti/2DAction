using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private enum ConflictType
    {
        None,
        Damage
    }

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

    [SerializeField] private ConflictType _conflictType = ConflictType.Damage;

    [SerializeField] private int _conflictDamage = 5;

    private bool isEnteredLockonLange;
    private bool lastEnteredLockonLange;

    private WithinCameraLangeEnemyManager cameraLangeEnemyManager;
    protected Camera mainCamera;
    protected bool updateFlag;

    virtual protected void Awake()
    {
        hp = maxHP;
    }

    virtual protected void Start()
    {
        cameraLangeEnemyManager = WithinCameraLangeEnemyManager.Instance;
        if (mainCamera == null)
        {
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        }
    }

    virtual protected void Update()
    {
        updateFlag = true;

        if (HP <= 0) updateFlag = false;

        Vector3 screenPos = mainCamera.WorldToScreenPoint(transform.position);
        Rect enemyUpdateScreenLange = new Rect(-200, -200, Screen.width + 200, Screen.height + 200);

        // �G����ʔ͈͂��牓�������Ă����珈�����X�L�b�v
        if (!enemyUpdateScreenLange.Contains(screenPos)) updateFlag = false;

        if (!updateFlag) return;

        if (isEnteredLockonLange)
        {
            if (!lastEnteredLockonLange)
            {
                cameraLangeEnemyManager._withinCameraLangeEnemies.Add(this);
                Debug.Log(gameObject.name + "���͈͓��ɓ�����");
            }
            
        }
        else
        {
            if (lastEnteredLockonLange)
            {
                cameraLangeEnemyManager._withinCameraLangeEnemies.Remove(this);
                Debug.Log(gameObject.name + "���͈͂���o��");
            }
            
        }

        lastEnteredLockonLange = isEnteredLockonLange;
        isEnteredLockonLange = false;
    }

    private void OnWillRenderObject()
    {
        if (HP <= 0) return;

        if (cameraLangeEnemyManager == null)
        {
            cameraLangeEnemyManager = WithinCameraLangeEnemyManager.Instance;
        }

        if (mainCamera == null)
        {
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        }

        // �J�����̕`��͈͓��Ɏ��܂��Ă��āA���b�N�I���͈͓��ɂ����܂��Ă�����t���O���Ă�
        Vector3 screenPos = mainCamera.WorldToScreenPoint(transform.position);

        if (cameraLangeEnemyManager.LockonLangeRect.Contains(screenPos)){
            isEnteredLockonLange = true;
        }
        
    }

    public void TakeDamage(int damage)
    {
        if (HP <= 0) return;

        HP -= damage;

        if (HP <= 0)
        {
            HP = 0;
            cameraLangeEnemyManager._withinCameraLangeEnemies.Remove(this);
            Destroy(gameObject);
        }

    }

    virtual protected void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if(_conflictType == ConflictType.Damage)
            {
                PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();

                playerHealth.TakeDamage(_conflictDamage);
            }
        }
    }

    virtual protected void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("��������");
        if (collision.CompareTag("Bullet"))
        {
            // �v���C���[�̒e�ƏՓ�
            Bullet bullet = collision.gameObject.GetComponent<Bullet>();

            TakeDamage(bullet.Damage);
        }
    }
}
