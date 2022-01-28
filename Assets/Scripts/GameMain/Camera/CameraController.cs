using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // �v���C���[��Transform
    [SerializeField] private Transform _playerTransform;

    // Camera�R���|�[�l���g
    private Camera myCamera;
    public Camera Camera {
        get { return myCamera; }
    }

    // �v���C���[�̃r���[�|�[�g���W
    private Vector3 playerViewPortPos;
    public Vector3 PlayerViewPortPos {
        get { return playerViewPortPos; }
    }

    // SmoothDamp�p
    private Vector3 velocity;

    private PlayerController playerController;

    // �J�������v���C���[��ǔ����Ă��ǂ���
    private bool cameraMoveFlag = true;
    public bool CameraMoveFlag {
        get { return cameraMoveFlag; }
        set { cameraMoveFlag = value; }
    }

    // �{�X��ł̃J�������W
    private Vector3 bossBattleCameraPos = new Vector3(180f, -39.85f, -10f);

    // �{�X�����O�̃V���b�^�[���E�[�ɉf��J�������W
    private Vector3 bossRoomEntrancePos = new Vector3(168.78f, -39.85f, -10f);

    private void Awake()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
    }

    void Start()
    {
        // null�������猟�����Ď擾
        if(_playerTransform == null)
        {
            _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }

        playerController = _playerTransform.GetComponent<PlayerController>();
        myCamera = GetComponent<Camera>();
    }

    // �J�������W���{�X��p���W�Ɉړ�����R���[�`���Ăяo��
    public void SetCameraPosToBossBattle()
    {
        StartCoroutine(nameof(MoveToBossBattleCameraPos));
    }

    // �J�������{�X�펞�̃J�������W�Ɉړ�������
    private IEnumerator MoveToBossBattleCameraPos()
    {
        float waitTime = 0;
        Vector3 startPos = transform.position;

        while(waitTime < 1.5f)
        {
            transform.position = Vector3.Lerp(startPos, bossBattleCameraPos, waitTime);
            waitTime += Time.unscaledDeltaTime;
            yield return null;
        }

        transform.position = bossBattleCameraPos;
    }

    private void LateUpdate()
    {
        // �v���C���[�ǔ��֎~�Ȃ瑁�����^�[��
        if (!CameraMoveFlag) return;

        // �J�������{�X�����O�܂ŗ�����v���C���[�ǔ��֎~�ɂ���
        if (bossRoomEntrancePos.x <= transform.position.x)
        {
            CameraMoveFlag = false;
        }

        // �v���C���[�̃r���[�|�[�g���W���擾
        playerViewPortPos = myCamera.WorldToViewportPoint(_playerTransform.position);
        bool playerMoveFollowFlag = false;
        float followX = playerViewPortPos.x;
        float followY = playerViewPortPos.y;

        // �v���C���[����ʐ^�񒆂��E���Ɉړ����Ă���ǔ�����
        if(0.499f <= playerViewPortPos.x)
        {
            playerMoveFollowFlag = true;
            followX = 0.499f;
        }

        // �v���C���[����ʂ̏㑤�Ɉړ����Ă���ǔ�����
        if(0.67f <= playerViewPortPos.y)
        {
            playerMoveFollowFlag = true;
            followY = 0.67f;
        }
        // �v���C���[����ʂ̉����ɍ~��Ă���ǔ�����B���ɗ����Ă���ǔ����Ȃ�
        else if(playerViewPortPos.y <= 0.23f && !playerController.IsFalling)
        {
            playerMoveFollowFlag = true;
            followY = 0.23f;
        }

        // �v���C���[�ǔ�
        if (playerMoveFollowFlag)
        {
            Vector3 delta = _playerTransform.position - myCamera.ViewportToWorldPoint(new Vector3(followX, followY, playerViewPortPos.z));
            Vector3 destination = transform.position + delta;

            transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, 0.05f);
        }
    }
}
