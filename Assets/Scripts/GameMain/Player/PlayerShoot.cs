using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    // �e�v���n�u
    [SerializeField] private GameObject _bulletPrefab;

    // ���b�N�}���̒e���ˈʒu
    [SerializeField] private Transform _muzzlePos;

    // ���b�N�I���J�[�\��
    [SerializeField] private LockonCursor _lockonCursor;

    // �e�̃_���[�W
    [SerializeField] private int _bulletDamage = 100;

    // �t�@�C�A���[�g
    [SerializeField] private float _fireRate = 0.2f;

    // ���̔��˂܂ł̃N�[���_�E��
    private float nextFireWaitTime;
    // ���b�N�I���\���X�g�Ǘ��}�l�[�W���[
    private WithinCameraLangeEnemyManager cameraLangeEnemyManager;
    // �A�j���[�V�����p�����[�^�[�R���g���[���[
    private AnimParamController animParamController;
    // �_���Ă�G
    private Enemy targetingEnemy;
    // �J�n����Scale.x�i�����j
    private float startLocalScaleX;

    private void Awake()
    {
        animParamController = GetComponent<AnimParamController>();
        startLocalScaleX = transform.localScale.x;
    }

    private void Start()
    {
        cameraLangeEnemyManager = WithinCameraLangeEnemyManager.Instance;
        if(_lockonCursor == null)
        {
            _lockonCursor = FindObjectOfType<LockonCursor>();
        }
    }

    void Update()
    {
        // ��ԋ߂��ɂ��郍�b�N�I���\�ȓG���擾
        targetingEnemy = cameraLangeEnemyManager.GetNearestEnemy(transform.position);

        if (targetingEnemy != null)
        {
            // �G�������烍�b�N�I���J�[�\���\��
            _lockonCursor.SetImageEnabled(true, targetingEnemy.transform);
        }
        else
        {
            // ���Ȃ��������\��
            _lockonCursor.SetImageEnabled(false, null);
        }

        // ���˃{�^���������Ƃ��A���˃N�[���_�E���I����Ă���
        if (InputManager.Instance.FireKey != 0 && nextFireWaitTime <= 0)
        {
            // �����Ă�����̃x�N�g��
            Vector3 vec = _muzzlePos.right * transform.localScale.x;

            if (targetingEnemy != null)
            {
                // �e������G�܂ł̕����x�N�g���쐬
                vec = (targetingEnemy.transform.position - _muzzlePos.position).normalized;

                // �_���Ă�G���������E���̂Ƃ�
                if (transform.position.x <= targetingEnemy.transform.position.x)
                {
                    // �E����
                    transform.localScale = new Vector3(startLocalScaleX, transform.localScale.y, transform.localScale.z);
                }
                // ������荶���̂Ƃ�
                else
                {
                    // ������
                    transform.localScale = new Vector3(-startLocalScaleX, transform.localScale.y, transform.localScale.z);
                }
            }

            // �e����
            GameObject obj = Instantiate(_bulletPrefab, _muzzlePos.position, Quaternion.identity) as GameObject;
            Bullet bulletSc = obj.GetComponent<Bullet>();
            
            bulletSc.ShotBullet( _bulletDamage, 10f, vec);
            nextFireWaitTime += _fireRate;
            animParamController.SetAnimParamBool("Shooting", true);

            SoundManager.Instance.PlaySE(SE.Shot);
        }
        
    }

    private void LateUpdate()
    {
        // �_���Ă�G�����邩�ˌ����[�V������
        if (targetingEnemy != null && _fireRate - 0.05f < nextFireWaitTime && nextFireWaitTime <= _fireRate)
        {
            // �_���Ă�G���������E���̂Ƃ�
            if (transform.position.x <= targetingEnemy.transform.position.x)
            {
                transform.localScale = new Vector3( startLocalScaleX, transform.localScale.y, transform.localScale.z);
            }
            // ������荶���̂Ƃ�
            else
            {
                transform.localScale = new Vector3( -startLocalScaleX, transform.localScale.y, transform.localScale.z);
            }
        }
    }

    private void FixedUpdate()
    {
        if(0 < nextFireWaitTime)
        {
            nextFireWaitTime -= Time.deltaTime;
            if(nextFireWaitTime <= 0)
            {
                animParamController.SetAnimParamBool("Shooting", false);
            }
        }
    }

    
}
