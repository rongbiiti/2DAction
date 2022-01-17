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
    [SerializeField] private int _bulletDamage = 100;

    // HP�o�[�v���n�u
    [SerializeField] private GameObject _HP_Bar_Prefab;

    Dictionary<int, string> actionInfo;
    Dictionary<int, float> actionDict;

    private float attackDelayTime = 0.5f;

    private Rigidbody2D rb;
    private Transform playerTransform;
    private Slider myHPBar;

    protected override void Start()
    {
        base.Start();

        DictInit();

        FindObjectOfType<CameraController>().CameraMoveFlag = false;
        //FindObjectOfType<CameraController>().SetCameraPosToBossBattle();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        rb = GetComponent<Rigidbody2D>();

        GameObject hpbarObj = Instantiate(_HP_Bar_Prefab) as GameObject;
        myHPBar = hpbarObj.GetComponent<Slider>();
        myHPBar.transform.SetParent(FindObjectOfType<Canvas>().transform, false);

        StartCoroutine(nameof(ActionSelect));
    }

    protected override void Update()
    {
        base.Update();

        if (!updateFlag) return;

        myHPBar.value = (float)HP / MaxHP;
    }

    private void DictInit()
    {
        actionInfo = new Dictionary<int, string>();
        actionInfo.Add(0, "JumpMove");
        actionInfo.Add(1, "Attack_JumpingFire");
        actionInfo.Add(2, "Attack_8RoundBulletFire");

        actionDict = new Dictionary<int, float>();
        actionDict.Add(0, 40f);
        actionDict.Add(1, 35f);
        actionDict.Add(2, 25f);
    }

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

        yield return new WaitForSeconds(0.75f);

        // ���u��3����1�Ő^��ɃW�����v����
        float randomPoint = Random.value * 100f;
        if (randomPoint < 33f)
        {
            rb.AddForce(new Vector2(0, 1.8f) * 4f, ForceMode2D.Impulse);
        }

        yield return new WaitForSeconds(0.25f);

        // �~�`�ɒe����
        for (int i = 0; i < fireRoundCount; i++)
        {
            GameObject obj = Instantiate(_bulletPrefab[1], _muzzlePos[0].position, Quaternion.identity) as GameObject;
            Bullet bulletSc = obj.GetComponent<Bullet>();

            bulletSc.ShotBullet(_bulletDamage, 6f, vectors[i]);

        }

        _animator.SetBool("RoundFire", true);

        yield return new WaitForSeconds(2f);

        _animator.SetBool("RoundFire", false);

        StartCoroutine(nameof(ActionSelect));

    }

    private IEnumerator Attack_JumpingFire()
    {
        int shootTimes = 5;
        float fireRate = 0.3f;

        _animator.SetTrigger("Aiming");

        LookPlayer();

        yield return new WaitForSeconds(1f);

        _animator.SetBool("Shooting", true);

        Vector3 vec = -transform.right * transform.localScale.x;

        rb.AddForce(new Vector2(vec.x * 0.5f, 1.8f) * 4f, ForceMode2D.Impulse);

        for (int i = 0; i < shootTimes; i++)
        {
            GameObject obj = Instantiate(_bulletPrefab[0], _muzzlePos[1].position, Quaternion.identity) as GameObject;
            Bullet bulletSc = obj.GetComponent<Bullet>();

            bulletSc.ShotBullet(_bulletDamage, 12f, vec);

            yield return new WaitForSeconds(fireRate);
        }

        yield return new WaitForSeconds(0.25f);

        // �v���C���[����
        LookPlayer();

        _animator.SetBool("Shooting", false);

        StartCoroutine(nameof(ActionSelect));
    }

    private IEnumerator JumpMove()
    {
        _animator.SetBool("RoundFire", true);

        LookPlayer();

        // �����Ă���ɃW�����v
        Vector3 vec = -transform.right * transform.localScale.x;
        rb.AddForce(new Vector2(vec.x, 1.6f) * 4f, ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.5f);

        bool isGrounded = false;

        while (!isGrounded)
        {
            isGrounded = Physics2D.Linecast(transform.position - transform.up * 0.1f, transform.position - transform.up * 0.75f, platformLayer);
            Debug.DrawLine(transform.position - transform.up * 0.1f, transform.position - transform.up * 0.75f, Color.red, 0, false);

            yield return null;
        }

        // ���Ό���
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

        rb.velocity = Vector3.zero;

        // �����������
        vec = -transform.right * transform.localScale.x;
        rb.AddForce(new Vector2(vec.x, 1.8f) * 3.5f, ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.5f);

        isGrounded = false;

        while (!isGrounded)
        {
            isGrounded = Physics2D.Linecast(transform.position - transform.up * 0.1f, transform.position - transform.up * 0.75f, platformLayer);
            Debug.DrawLine(transform.position - transform.up * 0.1f, transform.position - transform.up * 0.75f, Color.red, 0, false);

            yield return null;
        }

        rb.velocity = Vector3.zero;
        _animator.SetBool("RoundFire", false);

        StartCoroutine(nameof(ActionSelect));
    }
}
