using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform _playerTransform;

    private Camera myCamera;
    public Camera Camera {
        get { return myCamera; }
    }

    private Vector3 playerViewPortPos;
    public Vector3 PlayerViewPortPos {
        get { return playerViewPortPos; }
    }

    private Vector3 velocity;
    private PlayerController _playerController;

    private bool cameraMoveFlag = true;
    public bool CameraMoveFlag {
        get { return cameraMoveFlag; }
        set { cameraMoveFlag = value; }
    }

    private Vector3 bossBattleCameraPos = new Vector3(180f, -39.85f, -10f);
    private Vector3 bossRoomEntrancePos = new Vector3(168.78f, -39.85f, -10f);

    private void Awake()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
    }

    void Start()
    {
        // nullだったら検索して取得
        if(_playerTransform == null)
        {
            _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }

        _playerController = _playerTransform.GetComponent<PlayerController>();
        myCamera = GetComponent<Camera>();
    }

    public void SetCameraPosToBossBattle()
    {
        StartCoroutine(nameof(MoveToBossBattleCameraPos));
    }

    // 指定位置にカメラ強制的に移動
    private IEnumerator ForceCameraPosMove(Vector3 pos, float duration)
    {
        float waitTime = 0;
        Vector3 startPos = transform.position;
        while (waitTime < duration)
        {
            transform.position = Vector3.Lerp(startPos, pos, waitTime / duration);
            waitTime += Time.unscaledDeltaTime;
            yield return null;
        }
        transform.position = bossBattleCameraPos;
    }

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
        if (!CameraMoveFlag) return;

        if(bossRoomEntrancePos.x <= transform.position.x)
        {
            CameraMoveFlag = false;
        }

        playerViewPortPos = myCamera.WorldToViewportPoint(_playerTransform.position);
        bool playerMoveFollowFlag = false;
        float followX = playerViewPortPos.x;
        float followY = playerViewPortPos.y;

        if(0.499f <= playerViewPortPos.x)
        {
            playerMoveFollowFlag = true;
            followX = 0.499f;
        }

        if(0.67f <= playerViewPortPos.y)
        {
            playerMoveFollowFlag = true;
            followY = 0.67f;
        }
        else if(playerViewPortPos.y <= 0.23f && !_playerController.IsFalling)
        {
            playerMoveFollowFlag = true;
            followY = 0.23f;
        }

        if (playerMoveFollowFlag)
        {
            Vector3 delta = _playerTransform.position - myCamera.ViewportToWorldPoint(new Vector3(followX, followY, playerViewPortPos.z));
            Vector3 destination = transform.position + delta;

            transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, 0.05f);
        }
    }
}
