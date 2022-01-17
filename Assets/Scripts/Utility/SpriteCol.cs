using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteCol : MonoBehaviour
{
    private SpriteRenderer spr;
    private LayerMask layer;

    private void Awake()
    {
        gameObject.name = transform.parent.name + "SprCol";
        spr = GetComponent<SpriteRenderer>();
        spr.color = Color.clear;
    }

    void Start()
    {
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
        }

        
    }

    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        
    }

    private void OnDestroy()
    {
        if (SpriteColManager.Instance == null) return;

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
        }
    }

}
