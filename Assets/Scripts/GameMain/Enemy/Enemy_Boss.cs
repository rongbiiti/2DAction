using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy_Boss : Enemy
{
    // レイヤーマスク（着地判定用）
    [SerializeField] private LayerMask platformLayer;

    // Animator
    [SerializeField] private Animator _animator;

    // 弾プレハブ
    [SerializeField] private GameObject[] _bulletPrefab;

    // 弾発射位置
    [SerializeField] private Transform[] _muzzlePos;

    // 弾のダメージ
    [SerializeField] private int[] _bulletDamage = { 15, 35 };

    // HPバープレハブ
    [SerializeField] private GameObject _HP_Bar_Prefab;

    // 地面SpriteCol
    [SerializeField] private SpriteCol _groundSpriteCol;

    // ゲームクリア演出オブジェクト
    [SerializeField] private GameClearEvent _gameClearEvent;

    Dictionary<int, string> actionInfo;
    Dictionary<int, float> actionDict;

    private float attackDelayTime = 0.5f;

    private NGHMRigidbody rb;
    private Transform playerTransform;
    private Slider myHPBar;

    protected override void Start()
    {
        base.Start();

        DictInit();

        FindObjectOfType<CameraController>().CameraMoveFlag = false;
        //FindObjectOfType<CameraController>().SetCameraPosToBossBattle();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        rb = GetComponent<NGHMRigidbody>();

        GameObject hpbarObj = Instantiate(_HP_Bar_Prefab) as GameObject;
        myHPBar = hpbarObj.GetComponent<Slider>();
        myHPBar.transform.SetParent(FindObjectOfType<Canvas>().transform, false);

        StartCoroutine(nameof(ActionSelect));

        SoundManager.Instance.PlayBGM(BGM.BossBattle);
        SoundManager.Instance.BGMVolume(0.2f);
    }

    protected override void Update()
    {
        base.Update();

        if (!updateFlag) return;

        myHPBar.value = (float)HP / MaxHP;
    }

    private void OnDestroy()
    {
        if(HP <= 0)
        {
            SoundManager.Instance.StopBGM();
            SoundManager.Instance.PlaySE(SE.Miss);

            // ゲームクリア演出起動
            _gameClearEvent.EventStart();
        }
        
    }

    /// <summary>
    /// アクション表初期化
    /// </summary>
    private void DictInit()
    {
        actionInfo = new Dictionary<int, string>();
        actionInfo.Add(0, "JumpMove");
        actionInfo.Add(1, "Attack_JumpingFire");
        actionInfo.Add(2, "Attack_8RoundBulletFire");

        actionDict = new Dictionary<int, float>();
        actionDict.Add(0, 35f);
        actionDict.Add(1, 35f);
        actionDict.Add(2, 30f);
    }

    /// <summary>
    /// 次のアクション選択
    /// </summary>
    /// <returns></returns>
    private IEnumerator ActionSelect()
    {
        yield return new WaitForSeconds(attackDelayTime);

        float total = 0;

        foreach(KeyValuePair<int, float> elem in actionDict)
        {
            total += elem.Value;
        }

        float randomPoint = Random.value * total;

        int actionKey = 0;

        foreach(KeyValuePair<int, float> elem in actionDict)
        {
            if(randomPoint < elem.Value)
            {
                actionKey = elem.Key;
                break;
            }
            else
            {
                randomPoint -= elem.Value;
            }

        }

        StartCoroutine(actionInfo[actionKey]);
        
    }

    /// <summary>
    /// プレイヤーがいる方に向く
    /// </summary>
    private void LookPlayer()
    {
        if(playerTransform.position.x < transform.position.x)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(-1 * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    /// <summary>
    /// 円形8方向にエネルギー弾放出
    /// </summary>
    /// <returns></returns>
    private IEnumerator Attack_8RoundBulletFire()
    {
        int fireRoundCount = 8;         // 円形に弾を何発撃つか
        float angle = (float)360 / fireRoundCount;          // 1発あたりの角度
        Vector2[] vectors = new Vector2[fireRoundCount];    // 発射ベクトル配列

        for(int i = 0; i < fireRoundCount; i++)
        {
            // 1発ごとの角度をラジアンに変換
            float rad = angle * i * Mathf.Deg2Rad;
            
            // ラジアンからベクトル作成
            vectors[i] = new Vector2(Mathf.Sin(rad), Mathf.Cos(rad));

        }

        _animator.SetTrigger("RoundFireReady");

        LookPlayer();

        SoundManager.Instance.PlaySE(SE.BossCharge, 1.3f);

        yield return new WaitForSeconds(0.75f);

        // HP半分以下で真上にジャンプする
        float hpPercent = (float)HP / MaxHP * 100f;
        if (hpPercent < 50f)
        {
            //rb.Velocity = new Vector2(0, 1.8f) * 4f;
            rb.AddForce(new Vector2(0, 8.5f));
            SoundManager.Instance.PlaySE(SE.BossJump, 1.3f);
        }

        yield return new WaitForSeconds(0.3f);

        // 円形に弾発射
        for (int i = 0; i < fireRoundCount; i++)
        {
            GameObject obj = Instantiate(_bulletPrefab[1], _muzzlePos[0].position, Quaternion.identity) as GameObject;
            Bullet bulletSc = obj.GetComponent<Bullet>();

            bulletSc.ShotBullet(_bulletDamage[1], 6f, vectors[i]);

        }

        _animator.SetBool("RoundFire", true);

        SoundManager.Instance.PlaySE(SE.BossShot2, 1.3f);

        yield return new WaitForSeconds(2f);

        _animator.SetBool("RoundFire", false);

        StartCoroutine(nameof(ActionSelect));

    }

    /// <summary>
    /// 前方にジャンプしながら5連射
    /// </summary>
    /// <returns></returns>
    private IEnumerator Attack_JumpingFire()
    {
        int shootTimes = 5;
        float fireRate = 0.3f;

        _animator.SetTrigger("Aiming");

        LookPlayer();

        SoundManager.Instance.PlaySE(SE.BossAiming, 1.3f);

        yield return new WaitForSeconds(1f);

        _animator.SetBool("Shooting", true);

        Vector3 vec = -transform.right * transform.localScale.x;

        //rb.Velocity = new Vector2(vec.x * 0.5f, 1.8f) * 4f;
        rb.AddForce(new Vector2(vec.x * 4f, 7f));

        for (int i = 0; i < shootTimes; i++)
        {
            GameObject obj = Instantiate(_bulletPrefab[0], _muzzlePos[1].position, Quaternion.identity) as GameObject;
            Bullet bulletSc = obj.GetComponent<Bullet>();

            bulletSc.ShotBullet(_bulletDamage[0], 12f, vec);

            SoundManager.Instance.PlaySE(SE.BossShot1, 1.3f);

            yield return new WaitForSeconds(fireRate);
        }

        // yield return new WaitForSeconds(0.25f);

        // プレイヤー向く
        LookPlayer();

        _animator.SetBool("Shooting", false);

        StartCoroutine(nameof(ActionSelect));
    }

    /// <summary>
    /// 2連続ジャンプ
    /// </summary>
    /// <returns></returns>
    private IEnumerator JumpMove()
    {
        _animator.SetBool("RoundFire", true);

        LookPlayer();

        // 向いてる方にジャンプ
        Vector3 vec = -transform.right * transform.localScale.x;
        //rb.Velocity = new Vector2(vec.x, 1.6f) * 4f;
        rb.AddForce(new Vector2(vec.x * 4.5f, 8f));

        SoundManager.Instance.PlaySE(SE.BossJump, 1.3f);

        // 0.1秒置いてから
        yield return new WaitForSeconds(0.1f);

        // 接地判定
        SpriteCol hitCol = _groundSpriteCol.HitCheck_Ground();

        while (!hitCol)
        {
            hitCol = _groundSpriteCol.HitCheck_Ground();

            yield return null;
        }

        // 反対向く
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

        // ベロシティ0にする
        rb.Velocity = Vector3.zero;

        yield return new WaitForSeconds(0.08f);

        // もっかい飛ぶ
        vec = -transform.right * transform.localScale.x;
        //rb.Velocity = new Vector2(vec.x, 1.8f) * 3.5f;
        rb.AddForce(new Vector2(vec.x * 5.5f, 6f));

        SoundManager.Instance.PlaySE(SE.BossJump, 1.3f);

        // 0.1秒置いてから
        yield return new WaitForSeconds(0.1f);

        // 接地判定
        hitCol = _groundSpriteCol.HitCheck_Ground();

        while (!hitCol)
        {
            hitCol = _groundSpriteCol.HitCheck_Ground();

            yield return null;
        }

        // ベロシティ0にする
        rb.Velocity = Vector3.zero;
        _animator.SetBool("RoundFire", false);

        StartCoroutine(nameof(ActionSelect));
    }
}
