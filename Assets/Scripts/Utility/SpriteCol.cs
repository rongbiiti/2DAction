using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteCol : MonoBehaviour
{

    #region プロパティ

    [Header("すり抜けさせないか（剛体か）")]
    [SerializeField] private bool _isRigid;
    public bool IsRigid {
        get { return _isRigid; }
    }



    private SpriteRenderer spr;
    public SpriteRenderer SPR {
        get { return spr; }
    }



    /// <summary>
    /// 上
    /// </summary>
    public float MaxY {
        get { return spr.bounds.center.y + spr.bounds.extents.y; }
    }

    /// <summary>
    /// 下
    /// </summary>
    public float MinY {
        get { return spr.bounds.center.y - spr.bounds.extents.y; }
    }

    /// <summary>
    /// 右
    /// </summary>
    public float MaxX {
        get { return spr.bounds.center.x + spr.bounds.extents.x; }
    }

    /// <summary>
    /// 左
    /// </summary>
    public float MinX {
        get { return spr.bounds.center.x - spr.bounds.extents.x; }
    }

    #endregion

    #region private変数

    private LayerMask layer;
    private bool isRendered;
    private bool lastRendered;

    #endregion

    #region Unityコールバック（AwakeとかUpdate）
    private void Awake()
    {
        gameObject.name = transform.parent.name + "SprCol";
        spr = GetComponent<SpriteRenderer>();
        spr.color = Color.clear;
    }



    void Start()
    {
        //AddColManagerList();
        if (gameObject.CompareTag("Ground"))
        {
            AddColManagerList();
        }
    }



    void Update()
    {
        if (gameObject.CompareTag("Ground"))
        {
            return;
        }

        if (isRendered)
        {
            if (!lastRendered)
            {
                AddColManagerList();
                //spr.color = Color.yellow;
            }
        }
        else
        {
            if (lastRendered)
            {
                RemoveColManagerList();
                spr.color = Color.clear;
            }
        }

        lastRendered = isRendered;
        isRendered = false;
    }



    private void FixedUpdate()
    {
        
    }



    /// <summary>
    /// 描画されたとき
    /// </summary>
    private void OnWillRenderObject()
    {
        if (SpriteColManager.Instance == null) return;

        isRendered = true;
    }



    /// <summary>
    /// 破壊されたとき
    /// </summary>
    private void OnDestroy()
    {
        RemoveColManagerList();
    }

    #endregion

    #region 当たり判定関数
    /// <summary>
    /// プレイヤーと当たってるか判定
    /// </summary>
    /// <returns></returns>
    public SpriteCol HitCheck_Player()
    {
        if(spr.bounds.Intersects(SpriteColManager.Instance.playerSprCol.SPR.bounds)){
            return SpriteColManager.Instance.playerSprCol;
        }

        return null;
    }



    /// <summary>
    /// プレイヤーの弾と当たってるか判定
    /// </summary>
    /// <returns></returns>
    public SpriteCol HitCheck_PlayerBullet()
    {
        // Bulletと判定
        foreach (var cols in SpriteColManager.Instance.bulletSprCols)
        {
            if (spr.bounds.Intersects(cols.spr.bounds))
            {
                return cols;
            }
        }

        return null;
    }



    /// <summary>
    /// 敵の弾と当たってるか判定
    /// </summary>
    /// <returns></returns>
    public SpriteCol HitCheck_EnemyBullet()
    {
        // EnemyBulletと判定
        foreach (var cols in SpriteColManager.Instance.enemyBulletSprCols)
        {
            if (spr.bounds.Intersects(cols.spr.bounds))
            {
                return cols;
            }
        }

        return null;
    }



    /// <summary>
    /// 地形と当たってるか判定
    /// </summary>
    /// <returns></returns>
    public SpriteCol HitCheck_Ground()
    {
        // Groundと判定
        foreach (var cols in SpriteColManager.Instance.groundSprCols)
        {
            if (spr.bounds.Intersects(cols.spr.bounds))
            {
                return cols;
            }
        }

        // RidableEnemyと判定
        foreach(var cols in SpriteColManager.Instance.ridableEnemySprCols)
        {
            if (spr.bounds.Intersects(cols.spr.bounds))
            {
                return cols;
            }
        }

        return null;
    }



    /// <summary>
    /// トリガーと当たってるか
    /// </summary>
    public SpriteCol HitCheck_Trigger()
    {
        // Triggerと判定
        foreach(var cols in SpriteColManager.Instance.triggerSprCols)
        {
            if(spr.bounds.Intersects(cols.spr.bounds))
            {
                return cols;
            }
        }

        return null;
    }

    #endregion

    #region リスト管理操作
    /// <summary>
    /// マネージャーのリストに自分を追加
    /// </summary>
    public void AddColManagerList()
    {
        if (SpriteColManager.Instance == null) return;

        layer = gameObject.layer;

        switch (layer.value)
        {
            case 6:
                SpriteColManager.Instance.groundSprCols.Add(this);
                break;
            case 7:
                SpriteColManager.Instance.bulletSprCols.Add(this);
                break;
            case 8:
                SpriteColManager.Instance.playerSprCol = this;
                break;
            case 9:
                SpriteColManager.Instance.enemySprCols.Add(this);
                break;
            case 10:
                SpriteColManager.Instance.ridableEnemySprCols.Add(this);
                break;
            case 11:
                SpriteColManager.Instance.enemyBulletSprCols.Add(this);
                break;
            case 12:
                SpriteColManager.Instance.triggerSprCols.Add(this);
                break;
        }
    }



    ///<summary>
    ///マネージャーのリストから自分を削除
    ///</summary>
    public void RemoveColManagerList()
    {
        if (SpriteColManager.Instance == null) return;

        layer = gameObject.layer;

        switch (layer.value)
        {
            case 6:
                SpriteColManager.Instance.groundSprCols.Remove(this);
                break;
            case 7:
                SpriteColManager.Instance.bulletSprCols.Remove(this);
                break;
            case 9:
                SpriteColManager.Instance.enemySprCols.Remove(this);
                break;
            case 10:
                SpriteColManager.Instance.ridableEnemySprCols.Remove(this);
                break;
            case 11:
                SpriteColManager.Instance.enemyBulletSprCols.Remove(this);
                break;
            case 12:
                SpriteColManager.Instance.triggerSprCols.Remove(this);
                break;
        }
    }

    #endregion
}