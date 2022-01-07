using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WithinCameraLangeEnemyManager : SingletonMonoBehaviour<WithinCameraLangeEnemyManager>
{
    [SerializeField] private Vector2 _lockonLange = new Vector2(0.7f, 0.8f);

    // ��ʓ��ɂ���G��List
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

    // �w�肵���ʒu����ł��߂��G���擾
    public Enemy GetNearestEnemy(Vector3 pos)
    {
        // ���X�g���󂾂�����null�Ԃ�
        if(_withinCameraLangeEnemies.Count == 0)
        {
            return null;
        }

        Enemy returnEnemy = _withinCameraLangeEnemies[0];

        // List��1�̂����o�^����ĂȂ������炻��Ԃ�
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
