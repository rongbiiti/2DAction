using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteColManager : SingletonMonoBehaviour<SpriteColManager>
{
    //@vC[ΜSpriteCol
    public SpriteCol playerSprCol;

    // n`
    public List<SpriteCol> groundSprCols = new List<SpriteCol>();

    // G
    public List<SpriteCol> enemySprCols = new List<SpriteCol>();

    // ζκιG
    public List<SpriteCol> ridableEnemySprCols = new List<SpriteCol>();

    // vC[Μe
    public List<SpriteCol> bulletSprCols = new List<SpriteCol>();

    // GΜe
    public List<SpriteCol> enemyBulletSprCols = new List<SpriteCol>();

    // gK[
    public List<SpriteCol> triggerSprCols = new List<SpriteCol>();



}
