using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WithinCameraLangeEnemyManager : SingletonMonoBehaviour<WithinCameraLangeEnemyManager>
{
    [SerializeField] private Vector2 _lockonLange = new Vector2(0.7f, 0.8f);

    // 画面内にいる敵のList
    public List<Enemy> _withinCameraLangeEnemies = new List<Enemy>();

    private Rect lockonLangeRect = new Rect(0.5f, 0.5f, 0.7f, 0.8f);
    public Rect LockonLangeRect {
        get { return lockonLangeRect; }
    }

    protected override void Awake()
    {
        base.Awake();
        lockonLangeRect = new Rect(0.5f - _lockonLange.x / 2f, 0.5f - _lockonLange.y / 2f, _lockonLange.x, _lockonLange.y);
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
