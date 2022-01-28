using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WithinCameraLangeEnemyManager : SingletonMonoBehaviour<WithinCameraLangeEnemyManager>
{
    [SerializeField] private CameraController _cameraController;

    [SerializeField] private Vector2 _lockonLange = new Vector2(0.7f, 0.8f);

    // 画面内にいる敵のList
    public List<Enemy> _withinCameraLangeEnemies = new List<Enemy>();

    private Transform playerTransform;
    private Vector2 lockonLangeHalf;

    // ロックオン範囲のRectangle
    private Rect lockonLangeRect = new Rect(0.5f, 0.5f, 0.7f, 0.8f);
    public Rect LockonLangeRect {
        get { return lockonLangeRect; }
    }

    private Rect screen;

    protected override void Awake()
    {
        base.Awake();
        lockonLangeHalf = new Vector2(_lockonLange.x / 2f, _lockonLange.y / 2f);
    }

    private void Start()
    {
        // nullだったら取得
        if(_cameraController == null)
        {
            _cameraController = FindObjectOfType<CameraController>();
        }

        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void LateUpdate()
    {
        var playerScreenPos = _cameraController.Camera.WorldToScreenPoint(playerTransform.position);
        playerScreenPos.x = playerScreenPos.x - Screen.width * lockonLangeHalf.x;
        playerScreenPos.y = playerScreenPos.y - Screen.height * lockonLangeHalf.y;
        lockonLangeRect = new Rect(playerScreenPos.x, playerScreenPos.y, Screen.width * _lockonLange.x, Screen.height * _lockonLange.y);
    }        

    // 指定した位置から最も近い敵を取得
    public Enemy GetNearestEnemy(Vector3 pos)
    {
        // リストが空だったらnull返す
        if(_withinCameraLangeEnemies.Count == 0)
        {
            return null;
        }

        Enemy returnEnemy = _withinCameraLangeEnemies[0];

        // Listに1体しか登録されてなかったらそれ返す
        if (_withinCameraLangeEnemies.Count == 1)
        {
            return returnEnemy;
        }

        // ロックオン可能な敵の中で一番プレイヤーと近い敵を探す
        float minDistance = (pos - _withinCameraLangeEnemies[0].transform.position).magnitude;

        for (int i = 1; i < _withinCameraLangeEnemies.Count; i++)
        {
            float distance = (pos - _withinCameraLangeEnemies[i].transform.position).magnitude;
            if(distance < minDistance)
            {
                minDistance = distance;
                returnEnemy = _withinCameraLangeEnemies[i];
            }
        }

        return returnEnemy;
    }
}
