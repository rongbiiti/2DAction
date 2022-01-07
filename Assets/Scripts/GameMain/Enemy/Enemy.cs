using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int maxHP = 100;
    public int MaxHP {
        get { return maxHP; }
        set { maxHP = value; }
    }

    private int hp = 100;
    public int HP {
        get { return hp; }
        set { hp = value; }
    }

    private bool isEnteredLockonLange;
    private bool lastEnteredLockonLange;

    private WithinCameraLangeEnemyManager cameraLangeEnemyManager;
    private Camera mainCamera;

    virtual protected void Awake()
    {
        hp = maxHP;
    }

    virtual protected void Start()
    {
        cameraLangeEnemyManager = WithinCameraLangeEnemyManager.Instance;
        if (mainCamera == null)
        {
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        }
    }

    virtual protected void Update()
    {
        if (isEnteredLockonLange)
        {
            if (!lastEnteredLockonLange)
            {
                cameraLangeEnemyManager._withinCameraLangeEnemies.Add(this);
                Debug.Log(gameObject.name + "が範囲内に入った");
            }
            
        }
        else
        {
            if (lastEnteredLockonLange)
            {
                cameraLangeEnemyManager._withinCameraLangeEnemies.Remove(this);
                Debug.Log(gameObject.name + "が範囲から出た");
            }
            
        }

        lastEnteredLockonLange = isEnteredLockonLange;
        isEnteredLockonLange = false;
    }

    private void OnWillRenderObject()
    {
        if(cameraLangeEnemyManager == null)
        {
            cameraLangeEnemyManager = WithinCameraLangeEnemyManager.Instance;
        }

        if (mainCamera == null)
        {
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        }

        // カメラの描画範囲内に収まっていて、ロックオン範囲内にも収まっていたらフラグ立てる
        Vector3 viewPortPos = mainCamera.WorldToViewportPoint(transform.position);

        Debug.Log(gameObject.name + viewPortPos);

        if (cameraLangeEnemyManager.LockonLangeRect.Contains(viewPortPos)){
            isEnteredLockonLange = true;
        }
        
        Debug.Log(gameObject.name + "が画面内にいる");
    }
}
