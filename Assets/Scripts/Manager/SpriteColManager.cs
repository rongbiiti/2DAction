using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteColManager : SingletonMonoBehaviour<SpriteColManager>
{
    //�@�v���C���[��SpriteCol
    public SpriteCol playerSprCol;

    // �n�`
    public List<SpriteCol> groundSprCols = new List<SpriteCol>();

    // �G
    public List<SpriteCol> enemySprCols = new List<SpriteCol>();

    // ����G
    public List<SpriteCol> ridableEnemySprCols = new List<SpriteCol>();

    // �v���C���[�̒e
    public List<SpriteCol> bulletSprCols = new List<SpriteCol>();

    // �G�̒e
    public List<SpriteCol> enemyBulletSprCols = new List<SpriteCol>();

    // �g���K�[
    public List<SpriteCol> triggerSprCols = new List<SpriteCol>();



}
