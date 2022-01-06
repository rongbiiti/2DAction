using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InputManager : SingletonMonoBehaviour<InputManager>
{
    [SerializeField] private VirtualPad _virtualPad;
    [SerializeField] private VirtualJumpButton _virtualJumpButton;
    [SerializeField] private VirtualDashButton _virtualDashButton;
    [SerializeField] private VirtualFireButton _virtualFireButton;

    /* -- Horizontal入力 --------------------------------------------------------------------------- */
    private float moveKey = 0;
    public float MoveKey {
        get { return moveKey; }
    }

    /* -- Jump入力 --------------------------------------------------------------------------------- */
    private int jumpKey = 0;
    public int JumpKey {
        get { return jumpKey; }
    }

    /* -- Dash入力 --------------------------------------------------------------------------------- */
    private int dashKey = 0;
    public int DashKey {
        get { return dashKey; }
    }

    /* -- Fire入力 --------------------------------------------------------------------------------- */
    private int fireKey = 0;
    public int FireKey {
        get { return fireKey; }
    }

    private void Start()
    {
        // nullだったら取得
        if(_virtualPad == null)
        {
            _virtualPad = FindObjectOfType<VirtualPad>();
        }

        if (_virtualJumpButton == null)
        {
            _virtualJumpButton = FindObjectOfType<VirtualJumpButton>();
        }

        if (_virtualDashButton == null)
        {
            _virtualDashButton = FindObjectOfType<VirtualDashButton>();
        }

        if (_virtualFireButton == null)
        {
            _virtualFireButton = FindObjectOfType<VirtualFireButton>();
        }
    }

    void Update()
    {
        float virtualHorizontal = _virtualPad._horizontal;

        if(0.1f < virtualHorizontal)
        {
            virtualHorizontal = 1f;
        } 
        else if (virtualHorizontal < -0.1f)
        {
            virtualHorizontal = -1f;
        }
        else
        {
            virtualHorizontal = 0;
        }

        // 移動
        moveKey = Mathf.Clamp(Input.GetAxisRaw("Horizontal") + virtualHorizontal, -1f, 1f);

        // ジャンプ
        if (Input.GetButtonDown("Jump") || _virtualJumpButton.jump == 1)
        {
            jumpKey = 1;
        }
        else if (Input.GetButton("Jump"))
        {
            jumpKey = 2;
        }
        else if (Input.GetButtonUp("Jump") || _virtualJumpButton.jump == 0)
        {
            jumpKey = 0;
        }

        // ダッシュ
        if (Input.GetButtonDown("Dash") || _virtualDashButton.dash == 1)
        {
            dashKey = 1;
        }
        else if (Input.GetButton("Dash"))
        {
            dashKey = 1;
        }
        else if (Input.GetButtonUp("Dash") || _virtualDashButton.dash == 0)
        {
            dashKey = 0;
        }

        // 発射
        if (Input.GetButtonDown("Fire") || _virtualFireButton.fire == 1)
        {
            fireKey = 1;
        }
        else if (Input.GetButton("Fire"))
        {
            fireKey = 2;
        }
        else if (Input.GetButtonUp("Fire") || _virtualFireButton.fire == 0)
        {
            fireKey = 0;
        }
    }
}