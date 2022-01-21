using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NGHMRigidbody : MonoBehaviour
{
    [SerializeField] private SpriteCol _mySpriteCol;
    [SerializeField] private SpriteCol _groundCheckSpriteCol;
    [SerializeField] private SpriteCol _wallCheckSpriteCol;
    [SerializeField] private SpriteCol _ceilCheckSpriteCol;

    [SerializeField] private bool _useGravity = true;

    [Space(10f)]
    [SerializeField] private float _gravityRate;

    [Header("最大Y重力（正の値を設定してくれ）")]
    [SerializeField] private float _maxGravity = 6;

    [SerializeField] private float _fliction = 0;

    private Vector3 velocity;
    public Vector3 Velocity {
        set { velocity = value; }
        get { return velocity; }
    }

    private Vector3 lastVelocity;
    private bool isGround = true;
    private bool isInitComplete = false;

    void Start()
    {
        isInitComplete = true;
    }

    private void FixedUpdate()
    {
        if (!isInitComplete) return;

        // 接地フラグ折る
        isGround = false;

        // 接地判定
        SpriteCol hitGroundCol = _groundCheckSpriteCol.HitCheck_Ground();

        // 地面と当たってたら押し戻し処理
        // 剛体じゃなかったら押し戻しはしない
        if (hitGroundCol != null && hitGroundCol.IsRigid)
        {
            float newY = transform.position.y + (hitGroundCol.MaxY - _mySpriteCol.MinY);
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            velocity.y = 0;

            isGround = true;
        }

        // 天井判定
        SpriteCol hitCeilCol = _ceilCheckSpriteCol.HitCheck_Ground();

        if (hitCeilCol != null && hitCeilCol.IsRigid)
        {
            float newY = transform.position.y - (_mySpriteCol.MaxY - hitCeilCol.MinY);
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);

            // 天井と当たったとき上昇する力が入ってたら0にする
            if (0 < velocity.y)
            {
                velocity.y = 0;
            }
        }

        float flicFlag = 0f;

        if (isGround)
        {
            flicFlag = 1f;
        }

        // X方向にかかる力減少
        if (0 < velocity.x && _useGravity)
        {
            // 右方向への力残ってたら減らす
            velocity.x -= 5f * Time.fixedDeltaTime * _gravityRate * (flicFlag * _fliction);

            // 減らした後、左方向に力が入ることになっちゃったら0に補正
            if (velocity.x <= 0)
            {
                velocity.x = 0;
            }
        }
        else if (velocity.x < 0 && _useGravity)
        {
            // 左方向への力残ってたら減らす
            velocity.x += 5f * Time.fixedDeltaTime * _gravityRate * (flicFlag * _fliction);

            // 減らした後、右方向に力が入ることになっちゃったら0に補正
            if (0 <= velocity.x)
            {
                velocity.x = 0;
            }
        }

        // Y方向の力
        // 接地していなかったら重力をかける
        if (!isGround && _useGravity)
        {
            velocity.y += Physics2D.gravity.y * _gravityRate;

            // 最大落下力より強くなってたらそれ以上強くならないようにする
            if(velocity.y < -_maxGravity)
            {
                velocity.y = -_maxGravity;
            }
        }

        
    }

    public void AddForce(Vector3 power)
    {
        velocity = power;

        transform.position += velocity * Time.deltaTime;
    }

    void Update()
    {
        if (!isInitComplete) return;

        // 壁に当たったときの押し戻し処理
        SpriteCol hitWallCol = _wallCheckSpriteCol.HitCheck_Ground();

        if (hitWallCol != null && hitWallCol.IsRigid)
        {
            float newX = transform.position.x;

            if (lastVelocity.x < 0)
            {
                // 左方向に進んでた時
                newX = transform.position.x + (hitWallCol.MaxX - _mySpriteCol.MinX);
            }
            else if (0 < lastVelocity.x)
            {
                // 右方向に進んでた時
                newX = transform.position.x - (_mySpriteCol.MaxX - hitWallCol.MinX);
            }

            transform.position = new Vector3(newX, transform.position.y, transform.position.z);
            velocity.x = 0;
        }

        transform.position += velocity * Time.deltaTime;

        // この更新時のVelocity覚えておく
        lastVelocity = velocity;
    }

}
