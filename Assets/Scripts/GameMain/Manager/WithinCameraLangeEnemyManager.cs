using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WithinCameraLangeEnemyManager : SingletonMonoBehaviour<WithinCameraLangeEnemyManager>
{
    [SerializeField] private CameraController _cameraController;

    [SerializeField] private Vector2 _lockonLange = new Vector2(0.7f, 0.8f);

    // ‰æ–Ê“à‚É‚¢‚é“G‚ÌList
    public List<Enemy> _withinCameraLangeEnemies = new List<Enemy>();

    private Transform playerTransform;
    private Vector2 lockonLangeHalf;

    private Rect lockonLangeRect = new Rect(0.5f, 0.5f, 0.7f, 0.8f);
    public Rect LockonLangeRect {
        get { return lockonLangeRect; }
    }

    private Rect screen;

    public Texture BoxTexture;

    protected override void Awake()
    {
        base.Awake();
        lockonLangeHalf = new Vector2(_lockonLange.x / 2f, _lockonLange.y / 2f);
    }

    private void Start()
    {
        // null‚¾‚Á‚½‚çæ“¾
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

    // w’è‚µ‚½ˆÊ’u‚©‚çÅ‚à‹ß‚¢“G‚ğæ“¾
    public Enemy GetNearestEnemy(Vector3 pos)
    {
        // ƒŠƒXƒg‚ª‹ó‚¾‚Á‚½‚çnull•Ô‚·
        if(_withinCameraLangeEnemies.Count == 0)
        {
            return null;
        }

        Enemy returnEnemy = _withinCameraLangeEnemies[0];

        // List‚É1‘Ì‚µ‚©“o˜^‚³‚ê‚Ä‚È‚©‚Á‚½‚ç‚»‚ê•Ô‚·
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
