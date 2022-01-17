using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteColManager : SingletonMonoBehaviour<SpriteColManager>
{
    public SpriteCol playerSprCol;
    public List<SpriteCol> groundSprCols = new List<SpriteCol>();
    public List<SpriteCol> enemySprCols = new List<SpriteCol>();
    public List<SpriteCol> ridableEnemySprCols = new List<SpriteCol>();
    public List<SpriteCol> bulletSprCols = new List<SpriteCol>();
    public List<SpriteCol> enemyBulletSprCols = new List<SpriteCol>();

    void Start()
    {
        
    }
    
    void Update()
    {
        
    }
}
