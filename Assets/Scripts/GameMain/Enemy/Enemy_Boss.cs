using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy_Boss : Enemy
{
    // ���C���[�}�X�N�i���n����p�j
    [SerializeField] private LayerMask platformLayer;

    // Animator
    [SerializeField] private Animator _animator;

    // �e�v���n�u
    [SerializeField] private GameObject[] _bulletPrefab;

    // �e���ˈʒu
    [SerializeField] private Transform[] _muzzlePos;

    // �e�̃_���[�W
    [SerializeField] private int[] _bulletDamage = { 15, 35 };

    // HP�o�[�v���n�u
    [SerializeField] private GameObject _HP_Bar_Prefab;

    // �n��SpriteCol
    [SerializeField] private SpriteCol _groundSpriteCol;

    // �Q�[���N���A���o�I�u�W�F�N�g
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

            // �Q�[���N���A���o�N��
            _gameClearEvent.EventStart();
        }
        
    }

    /// <summary>
    /// �A�N�V�����\������
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
    /// ���̃A�N�V�����I��
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
    /// �v���C���[��������Ɍ���
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
    /// �~�`8�����ɃG�l���M�[�e���o
    /// </summary>
    /// <returns></returns>
    private IEnumerator Attack_8RoundBulletFire()
    {
        int fireRoundCount = 8;         // �~�`�ɒe����������
        float angle = (float)360 / fireRoundCount;          // 1��������̊p�x
        Vector2[] vectors = new Vector2[fireRoundCount];    // ���˃x�N�g���z��

        for(int i = 0; i < fireRoundCount; i++)
        {
            // 1�����Ƃ̊p�x�����W�A���ɕϊ�
            float rad = angle * i * Mathf.Deg2Rad;
            
            // ���W�A������x�N�g���쐬
            vectors[i] = new Vector2(Mathf.Sin(rad), Mathf.Cos(rad));

        }

        _animator.SetTrigger("RoundFireReady");

        LookPlayer();

        SoundManager.Instance.PlaySE(SE.BossCharge, 1.3f);

        yield return new WaitForSeconds(0.75f);

        // HP�����ȉ��Ő^��ɃW�����v����
        float hpPercent = (float)HP / MaxHP * 100f;
        if (hpPercent < 50f)
        {
            //rb.Velocity = new Vector2(0, 1.8f) * 4f;
            rb.AddForce(new Vector2(0, 8.5f));
            SoundManager.Instance.PlaySE(SE.BossJump, 1.3f);
        }

        yield return new WaitForSeconds(0.3f);

        // �~�`�ɒe����
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
    /// �O���ɃW�����v���Ȃ���5�A��
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

        // �v���C���[����
        LookPlayer();

        _animator.SetBool("Shooting", false);

        StartCoroutine(nameof(ActionSelect));
    }

    /// <summary>
    /// 2�A���W�����v
    /// </summary>
    /// <returns></returns>
    private IEnumerator JumpMove()
    {
        _animator.SetBool("RoundFire", true);

        LookPlayer();

        // �����Ă���ɃW�����v
        Vector3 vec = -transform.right * transform.localScale.x;
        //rb.Velocity = new Vector2(vec.x, 1.6f) * 4f;
        rb.AddForce(new Vector2(vec.x * 4.5f, 8f));

        SoundManager.Instance.PlaySE(SE.BossJump, 1.3f);

        // 0.1�b�u���Ă���
        yield return new WaitForSeconds(0.1f);

        // �ڒn����
        SpriteCol hitCol = _groundSpriteCol.HitCheck_Ground();

        while (!hitCol)
        {
            hitCol = _groundSpriteCol.HitCheck_Ground();

            yield return null;
        }

        // ���Ό���
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

        // �x���V�e�B0�ɂ���
        rb.Velocity = Vector3.zero;

        yield return new WaitForSeconds(0.08f);

        // �����������
        vec = -transform.right * transform.localScale.x;
        //rb.Velocity = new Vector2(vec.x, 1.8f) * 3.5f;
        rb.AddForce(new Vector2(vec.x * 5.5f, 6f));

        SoundManager.Instance.PlaySE(SE.BossJump, 1.3f);

        // 0.1�b�u���Ă���
        yield return new WaitForSeconds(0.1f);

        // �ڒn����
        hitCol = _groundSpriteCol.HitCheck_Ground();

        while (!hitCol)
        {
            hitCol = _groundSpriteCol.HitCheck_Ground();

            yield return null;
        }

        // �x���V�e�B0�ɂ���
        rb.Velocity = Vector3.zero;
        _animator.SetBool("RoundFire", false);

        StartCoroutine(nameof(ActionSelect));
    }
}
