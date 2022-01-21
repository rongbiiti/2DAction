using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteColManager : SingletonMonoBehaviour<SpriteColManager>
{
    //　プレイヤーのSpriteCol
    public SpriteCol playerSprCol;

    // 地形
    public List<SpriteCol> groundSprCols = new List<SpriteCol>();

    // 敵
    public List<SpriteCol> enemySprCols = new List<SpriteCol>();

    // 乗れる敵
    public List<SpriteCol> ridableEnemySprCols = new List<SpriteCol>();

    // プレイヤーの弾
    public List<SpriteCol> bulletSprCols = new List<SpriteCol>();

    // 敵の弾
    public List<SpriteCol> enemyBulletSprCols = new List<SpriteCol>();

    // トリガー
    public List<SpriteCol> triggerSprCols = new List<SpriteCol>();



}
