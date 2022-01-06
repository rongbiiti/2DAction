using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerManager : SingletonMonoBehaviour<PlayerManager>
{
    /* -- 移動速度(地上) --------------------------------------------------------------------------- */
    [SerializeField, Range(0f, 20f)] private float moveSpeed = 10f;
    public float MoveSpeed {
        get { return moveSpeed; }
        set { moveSpeed = value; }
    }

    /* -- 移動速度(地上) --------------------------------------------------------------------------- */
    [SerializeField, Range(0f, 20f)] private float dashSpeed = 10f;
    public float DashSpeed {
        get { return dashSpeed; }
        set { dashSpeed = value; }
    }

    /* -- 移動速度(空中) --------------------------------------------------------------------------- */
    [SerializeField, Range(0f, 20f)] private float jumpMoveSpeed = 8f;
    public float JumpMoveSpeed {
        get { return jumpMoveSpeed; }
        set { jumpMoveSpeed = value; }
    }

    /* -- ジャンプ力 ------------------------------------------------------------------------------- */
    [SerializeField, Range(0f, 40f)] private float jumpPower = 20f;
    public float JumpPower {
        get { return jumpPower; }
        set { jumpPower = value; }
    }

    /* -- 重力倍率 --------------------------------------------------------------------------------- */
    [SerializeField, Range(0f, 10f)] private float gravityRate = 1.8f;
    public float GravityRate {
        get { return gravityRate; }
        set { gravityRate = value; }
    }

}