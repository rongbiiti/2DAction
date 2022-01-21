using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteCol : MonoBehaviour
{

    #region �v���p�e�B

    [Header("���蔲�������Ȃ����i���̂��j")]
    [SerializeField] private bool _isRigid;
    public bool IsRigid {
        get { return _isRigid; }
    }



    private SpriteRenderer spr;
    public SpriteRenderer SPR {
        get { return spr; }
    }



    /// <summary>
    /// ��
    /// </summary>
    public float MaxY {
        get { return spr.bounds.center.y + spr.bounds.extents.y; }
    }

    /// <summary>
    /// ��
    /// </summary>
    public float MinY {
        get { return spr.bounds.center.y - spr.bounds.extents.y; }
    }

    /// <summary>
    /// �E
    /// </summary>
    public float MaxX {
        get { return spr.bounds.center.x + spr.bounds.extents.x; }
    }

    /// <summary>
    /// ��
    /// </summary>
    public float MinX {
        get { return spr.bounds.center.x - spr.bounds.extents.x; }
    }

    #endregion

    #region private�ϐ�

    private LayerMask layer;
    private bool isRendered;
    private bool lastRendered;

    #endregion

    #region Unity�R�[���o�b�N�iAwake�Ƃ�Update�j
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
    /// �`�悳�ꂽ�Ƃ�
    /// </summary>
    private void OnWillRenderObject()
    {
        if (SpriteColManager.Instance == null) return;

        isRendered = true;
    }



    /// <summary>
    /// �j�󂳂ꂽ�Ƃ�
    /// </summary>
    private void OnDestroy()
    {
        RemoveColManagerList();
    }

    #endregion

    #region �����蔻��֐�
    /// <summary>
    /// �v���C���[�Ɠ������Ă邩����
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
    /// �v���C���[�̒e�Ɠ������Ă邩����
    /// </summary>
    /// <returns></returns>
    public SpriteCol HitCheck_PlayerBullet()
    {
        // Bullet�Ɣ���
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
    /// �G�̒e�Ɠ������Ă邩����
    /// </summary>
    /// <returns></returns>
    public SpriteCol HitCheck_EnemyBullet()
    {
        // EnemyBullet�Ɣ���
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
    /// �n�`�Ɠ������Ă邩����
    /// </summary>
    /// <returns></returns>
    public SpriteCol HitCheck_Ground()
    {
        // Ground�Ɣ���
        foreach (var cols in SpriteColManager.Instance.groundSprCols)
        {
            if (spr.bounds.Intersects(cols.spr.bounds))
            {
                return cols;
            }
        }

        // RidableEnemy�Ɣ���
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
    /// �g���K�[�Ɠ������Ă邩
    /// </summary>
    public SpriteCol HitCheck_Trigger()
    {
        // Trigger�Ɣ���
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

    #region ���X�g�Ǘ�����
    /// <summary>
    /// �}�l�[�W���[�̃��X�g�Ɏ�����ǉ�
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
    ///�}�l�[�W���[�̃��X�g���玩�����폜
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