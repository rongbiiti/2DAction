using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // �v���C���[�ƏՓ˂����Ƃ��Ƀ_���[�W��^���邩
    private enum ConflictType
    {
        None,
        Damage
    }

    // �G���j���G�t�F�N�g
    [SerializeField] private GameObject _diedParticle;

    // SpriteCol
    [SerializeField] private SpriteCol _mySpriteCol;

    // �ő�HP
    [SerializeField] private int maxHP = 100;
    public int MaxHP {
        get { return maxHP; }
        set { maxHP = value; }
    }

    // ����HP
    private int hp = 100;
    public int HP {
        get { return hp; }
        set { hp = value; }
    }

    // �Փ˃^�C�v
    [SerializeField] private ConflictType _conflictType = ConflictType.Damage;

    // �v���C���[�ɏՓ˂����Ƃ��ɗ^����_���[�W
    [SerializeField] private int _conflictDamage = 5;

    // ���b�N�I���͈͂ɓ����Ă��邩
    private bool isEnteredLockonLange;

    // �O�̃t���[���Ń��b�N�I���͈͂ɓ����Ă�����
    private bool lastEnteredLockonLange;

    // �J�����`��͈͂ɓ����Ă���G���Ǘ�����}�l�[�W���[
    private WithinCameraLangeEnemyManager cameraLangeEnemyManager;

    // ���C���J����
    protected Camera mainCamera;

    // �X�V�������Ă�������
    protected bool updateFlag;

    virtual protected void Awake()
    {
        // HP��������
        hp = maxHP;
    }

    virtual protected void Start()
    {
        // �C���X�^���X�擾
        cameraLangeEnemyManager = WithinCameraLangeEnemyManager.Instance;
        if (mainCamera == null)
        {
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        }

        // �X�e�[�W�J�n��������X���W���v���C���[��荶�Ȃ��A�N�e�B�u�ɂ���
        Transform playerTrans = GameObject.FindGameObjectWithTag("Player").transform;
        if(transform.position.x < playerTrans.position.x)
        {
            gameObject.SetActive(false);
        }
    }

    virtual protected void Update()
    {
        updateFlag = true;

        // HP0�ȉ��Ȃ�X�V���Ȃ�
        if (HP <= 0) updateFlag = false;

        // �X�N���[�����W�擾
        Vector3 screenPos = mainCamera.WorldToScreenPoint(transform.position);
        Rect enemyUpdateScreenLange = new Rect(-200, -200, Screen.width + 400, Screen.height + 400);

        // �G����ʔ͈͂��牓�������Ă����珈�����X�L�b�v
        if (!enemyUpdateScreenLange.Contains(screenPos))
        {
            updateFlag = false;
        }

        if (!updateFlag) return;

        if (isEnteredLockonLange)
        {
            if (!lastEnteredLockonLange)
            {
                // �O�̃t���[���Ń��b�N�I���͈͂ɓ����Ă��炸�A
                // ���̃t���[���Ŕ͈͂ɓ����Ă��烍�b�N�I���\���X�g�ɉ�����
                cameraLangeEnemyManager._withinCameraLangeEnemies.Add(this);
            }
            
        }
        else
        {
            if (lastEnteredLockonLange)
            {
                // ���b�N�I���͈͂���o���烍�b�N�I���\���X�g����폜
                cameraLangeEnemyManager._withinCameraLangeEnemies.Remove(this);
            }
            
        }

        // �v���C���[�̒e�Ɠ����蔻��
        SpriteCol bulletCol = _mySpriteCol.HitCheck_PlayerBullet();
        if (bulletCol)
        {
            // �e�̃R���|�[�l���g�擾
            Bullet bullet = bulletCol.transform.parent.GetComponent<Bullet>();
            // �_���[�W�󂯂�
            TakeDamage(bullet.Damage);
            // �e��j��
            Destroy(bulletCol.transform.parent.gameObject);
            // �e�q�b�gSE�Đ�
            SoundManager.Instance.PlaySE(SE.ShotHit);
        }

        // �v���C���[�ƏՓ˂����Ƃ��A�_���[�W��^����G�Ȃ�_���[�W��^����
        if(_conflictType == ConflictType.Damage)
        {
            SpriteCol playerCol = _mySpriteCol.HitCheck_Player();
            if (playerCol)
            {
                PlayerHealth playerHealth = playerCol.GetComponentInParent<PlayerHealth>();

                // ���G���ԂłȂ��Ƃ��ɉ��Đ�
                if (playerHealth.InvincibleWaitTime <= 0 && 0 < playerHealth.HP)
                {
                    SoundManager.Instance.PlaySE(SE.Damage);
                }
                // �v���C���[�Ƀ_���[�W
                playerHealth.TakeDamage(_conflictDamage);
                
            }
        }
        
        // ���t���[���Ń��b�N�I���͈͂ɓ����Ă邩�L��
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

    // �_���[�W���󂯂�
    public void TakeDamage(int damage)
    {
        if (HP <= 0) return;

        HP -= damage;

        // ���j��
        if (HP <= 0)
        {
            HP = 0;
            // ���b�N�I���\���X�g����폜
            cameraLangeEnemyManager._withinCameraLangeEnemies.Remove(this);
            // ���j���G�t�F�N�g�o��
            Instantiate(_diedParticle, transform.position, Quaternion.identity);
            // �����f�X�g���C
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
        else if (collision.gameObject.CompareTag("Lava"))
        {
            TakeDamage(maxHP);
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
