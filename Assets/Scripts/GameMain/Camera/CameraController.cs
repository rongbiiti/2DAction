using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // プレイヤーのTransform
    [SerializeField] private Transform _playerTransform;

    // Cameraコンポーネント
    private Camera myCamera;
    public Camera Camera {
        get { return myCamera; }
    }

    // プレイヤーのビューポート座標
    private Vector3 playerViewPortPos;
    public Vector3 PlayerViewPortPos {
        get { return playerViewPortPos; }
    }

    // SmoothDamp用
    private Vector3 velocity;

    private PlayerController playerController;

    // カメラがプレイヤーを追尾しても良いか
    private bool cameraMoveFlag = true;
    public bool CameraMoveFlag {
        get { return cameraMoveFlag; }
        set { cameraMoveFlag = value; }
    }

    // ボス戦でのカメラ座標
    private Vector3 bossBattleCameraPos = new Vector3(180f, -39.85f, -10f);

    // ボス部屋前のシャッターが右端に映るカメラ座標
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

        playerController = _playerTransform.GetComponent<PlayerController>();
        myCamera = GetComponent<Camera>();
    }

    // カメラ座標をボス戦用座標に移動するコルーチン呼び出す
    public void SetCameraPosToBossBattle()
    {
        StartCoroutine(nameof(MoveToBossBattleCameraPos));
    }

    // カメラをボス戦時のカメラ座標に移動させる
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
        // プレイヤー追尾禁止なら早期リターン
        if (!CameraMoveFlag) return;

        // カメラがボス部屋前まで来たらプレイヤー追尾禁止にする
        if (bossRoomEntrancePos.x <= transform.position.x)
        {
            CameraMoveFlag = false;
        }

        // プレイヤーのビューポート座標を取得
        playerViewPortPos = myCamera.WorldToViewportPoint(_playerTransform.position);
        bool playerMoveFollowFlag = false;
        float followX = playerViewPortPos.x;
        float followY = playerViewPortPos.y;

        // プレイヤーが画面真ん中より右側に移動してたら追尾する
        if(0.499f <= playerViewPortPos.x)
        {
            playerMoveFollowFlag = true;
            followX = 0.499f;
        }

        // プレイヤーが画面の上側に移動してたら追尾する
        if(0.67f <= playerViewPortPos.y)
        {
            playerMoveFollowFlag = true;
            followY = 0.67f;
        }
        // プレイヤーが画面の下側に降りてたら追尾する。穴に落ちてたら追尾しない
        else if(playerViewPortPos.y <= 0.23f && !playerController.IsFalling)
        {
            playerMoveFollowFlag = true;
            followY = 0.23f;
        }

        // プレイヤー追尾
        if (playerMoveFollowFlag)
        {
            Vector3 delta = _playerTransform.position - myCamera.ViewportToWorldPoint(new Vector3(followX, followY, playerViewPortPos.z));
            Vector3 destination = transform.position + delta;

            transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, 0.05f);
        }
    }
}
