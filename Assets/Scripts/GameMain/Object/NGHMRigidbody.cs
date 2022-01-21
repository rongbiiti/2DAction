using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NGHMRigidbody : MonoBehaviour
{
    [SerializeField] private SpriteCol _mySpriteCol;
    [SerializeField] private SpriteCol _groundCheckSpriteCol;
    [SerializeField] private SpriteCol _wallCheckSpriteCol;
    [SerializeField] private SpriteCol _ceilCheckSpriteCol;

    [SerializeField] private bool _useGravity = true;

    [Space(10f)]
    [SerializeField] private float _gravityRate;

    [Header("�ő�Y�d�́i���̒l��ݒ肵�Ă���j")]
    [SerializeField] private float _maxGravity = 6;

    [SerializeField] private float _fliction = 0;

    private Vector3 velocity;
    public Vector3 Velocity {
        set { velocity = value; }
        get { return velocity; }
    }

    private Vector3 lastVelocity;
    private bool isGround = true;
    private bool isInitComplete = false;

    void Start()
    {
        isInitComplete = true;
    }

    private void FixedUpdate()
    {
        if (!isInitComplete) return;

        // �ڒn�t���O�܂�
        isGround = false;

        // �ڒn����
        SpriteCol hitGroundCol = _groundCheckSpriteCol.HitCheck_Ground();

        // �n�ʂƓ������Ă��牟���߂�����
        // ���̂���Ȃ������牟���߂��͂��Ȃ�
        if (hitGroundCol != null && hitGroundCol.IsRigid)
        {
            float newY = transform.position.y + (hitGroundCol.MaxY - _mySpriteCol.MinY);
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            velocity.y = 0;

            isGround = true;
        }

        // �V�䔻��
        SpriteCol hitCeilCol = _ceilCheckSpriteCol.HitCheck_Ground();

        if (hitCeilCol != null && hitCeilCol.IsRigid)
        {
            float newY = transform.position.y - (_mySpriteCol.MaxY - hitCeilCol.MinY);
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);

            // �V��Ɠ��������Ƃ��㏸����͂������Ă���0�ɂ���
            if (0 < velocity.y)
            {
                velocity.y = 0;
            }
        }

        float flicFlag = 0f;

        if (isGround)
        {
            flicFlag = 1f;
        }

        // X�����ɂ�����͌���
        if (0 < velocity.x && _useGravity)
        {
            // �E�����ւ̗͎c���Ă��猸�炷
            velocity.x -= 5f * Time.fixedDeltaTime * _gravityRate * (flicFlag * _fliction);

            // ���炵����A�������ɗ͂����邱�ƂɂȂ����������0�ɕ␳
            if (velocity.x <= 0)
            {
                velocity.x = 0;
            }
        }
        else if (velocity.x < 0 && _useGravity)
        {
            // �������ւ̗͎c���Ă��猸�炷
            velocity.x += 5f * Time.fixedDeltaTime * _gravityRate * (flicFlag * _fliction);

            // ���炵����A�E�����ɗ͂����邱�ƂɂȂ����������0�ɕ␳
            if (0 <= velocity.x)
            {
                velocity.x = 0;
            }
        }

        // Y�����̗�
        // �ڒn���Ă��Ȃ�������d�͂�������
        if (!isGround && _useGravity)
        {
            velocity.y += Physics2D.gravity.y * _gravityRate;

            // �ő嗎���͂�苭���Ȃ��Ă��炻��ȏ㋭���Ȃ�Ȃ��悤�ɂ���
            if(velocity.y < -_maxGravity)
            {
                velocity.y = -_maxGravity;
            }
        }

        
    }

    public void AddForce(Vector3 power)
    {
        velocity = power;

        transform.position += velocity * Time.deltaTime;
    }

    void Update()
    {
        if (!isInitComplete) return;

        // �ǂɓ��������Ƃ��̉����߂�����
        SpriteCol hitWallCol = _wallCheckSpriteCol.HitCheck_Ground();

        if (hitWallCol != null && hitWallCol.IsRigid)
        {
            float newX = transform.position.x;

            if (lastVelocity.x < 0)
            {
                // �������ɐi��ł���
                newX = transform.position.x + (hitWallCol.MaxX - _mySpriteCol.MinX);
            }
            else if (0 < lastVelocity.x)
            {
                // �E�����ɐi��ł���
                newX = transform.position.x - (_mySpriteCol.MaxX - hitWallCol.MinX);
            }

            transform.position = new Vector3(newX, transform.position.y, transform.position.z);
            velocity.x = 0;
        }

        transform.position += velocity * Time.deltaTime;

        // ���̍X�V����Velocity�o���Ă���
        lastVelocity = velocity;
    }

}
