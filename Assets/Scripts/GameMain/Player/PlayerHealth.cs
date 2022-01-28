using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    // ���S�G�t�F�N�g
    [SerializeField] private GameObject _diedEffect;

    // HP�o�[�p��Slider
    [SerializeField] private Slider _HP_Bar;

    // �c�@UI��Text
    [SerializeField] private Text _zanki_UI;

    // ���G����
    [SerializeField] private float _invincibleTime = 3f;

    // ���G����
    private float invincibleWaitTime;
    public float InvincibleWaitTime {
        set { invincibleWaitTime = value; }
        get { return invincibleWaitTime; }
    }

    // �ő�HP
    [SerializeField] private int maxHP = 100;
    public int MaxHP {
        get { return maxHP; }
        set { maxHP = value; }
    }

    // ����HP
    private int hp = 100;
    public int HP {
        get { return hp; }
        set { hp = value; }
    }

    // �Q�[���N���A������
    public bool isCleared;

    private SpriteRenderer spriteRenderer;
    private AnimParamController animParamController;

    private void Awake()
    {
        // HP������
        hp = maxHP;
    }

    private void Start()
    {
        // �c�@��0�����Ȃ�X�e�[�W�̍ŏ����烊�X�^�[�g
        if(PlayerZankiManager.Instance.Zanki < 0)
        {
            PlayerZankiManager.Instance.ZankiReset();
            PlayerRestartPointManager.Instance.isUpdateRestartPos = false;
        }

        // �c�@UI�Ɏc�@�����f
        _zanki_UI.text = PlayerZankiManager.Instance.Zanki.ToString("D2");

        // ��x�����X�^�[�g�|�C���g�ʂ��ĂȂ������珉���ʒu�㏑��
        if (!PlayerRestartPointManager.Instance.isUpdateRestartPos)
        {
            PlayerRestartPointManager.Instance.RestartPoint = transform.position;
        }
        else
        {
            // ���X�^�[�g�|�C���g�ʂ��Ă��炻�����畜�A
            transform.position = PlayerRestartPointManager.Instance.RestartPoint;

            // �J�����ʒu������
            CameraController cameraController = FindObjectOfType<CameraController>();
            Vector3 cameraPos = cameraController.Camera.WorldToViewportPoint(transform.position);
            cameraPos.y += 0.3f;
            cameraPos.z = -10f;
            cameraController.transform.position = cameraController.Camera.ViewportToWorldPoint(cameraPos);
        }
        
        spriteRenderer = GetComponent<SpriteRenderer>();
        animParamController = GetComponent<AnimParamController>();

        SoundManager.Instance.PlayBGM(BGM.Main, 6.5f);
        SoundManager.Instance.BGMVolume(0.4f);
    }

    private void FixedUpdate()
    {
        // ���G���Ԓ��̓v���C���[��_�ł�����
        if(0 < invincibleWaitTime)
        {
            float level = Mathf.Abs(Mathf.Sin(Time.time * 10));
            spriteRenderer.color = new Color(1f, 1f, 1f, level);
            invincibleWaitTime -= Time.deltaTime;

            // ���G���ԏI������猳�ɖ߂�
            if(invincibleWaitTime <= 0)
            {
                spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
            }
        }
    }

    // �_���[�W���󂯂�
    public void TakeDamage(int damage, bool ignoreInvincible = false)
    {
        if (isCleared) return;  // �N���A���Ă���_���[�W����

        if (HP <= 0) return;    // HP0�ȉ��ɂ����Ȃ�

        if (0 < invincibleWaitTime && !ignoreInvincible) return;    // ���G���Ԓ������G���Ԗ����U���łȂ���Α������^�[��

        HP -= damage;

        if (HP <= 0)
        {
            HP = 0;
            animParamController.SetAnimParamBool("Died", true);
            StartCoroutine(nameof(DiedEffect));
        }
        else
        {
            invincibleWaitTime = _invincibleTime;
            animParamController.SetAnimParamTrigger("Damaged");
        }

        _HP_Bar.value = (float)HP / MaxHP;

    }

    // �v���C���[���S������
    private IEnumerator DiedEffect()
    {
        GetComponent<PlayerController>().enabled = false;   // �����~
        GetComponent<PlayerShoot>().enabled = false;
        GetComponent<NGHMRigidbody>().enabled = false;      // �d�͒�~
        //Rigidbody2D rb = GetComponent<Rigidbody2D>();
        //rb.velocity = Vector3.zero;
        //rb.isKinematic = true;
        //Collider2D[] colliders = GetComponents<Collider2D>();
        //foreach(var cols in colliders)
        //{
        //cols.enabled = false;
        //}
        SoundManager.Instance.StopBGM();

        yield return new WaitForSeconds(0.75f);

        spriteRenderer.enabled = false;     // �X�v���C�g��\��
        Instantiate(_diedEffect, transform.position, Quaternion.identity);  // ���S���G�t�F�N�g�o��
        SoundManager.Instance.PlaySE(SE.Miss);

        yield return new WaitForSeconds(5f);

        // �c�@���炵�ă��X�^�[�g�|�C���g���畜��
        PlayerZankiManager.Instance.Zanki--;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // �Q�[���N���A��A�ŏ��̃V�[�����烊�X�^�[�g
    public void GameRestart()
    {
        PlayerZankiManager.Instance.Zanki = -1;
        SceneManager.LoadScene(0);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Needle"))
        {
            TakeDamage(10);
        }
        else if (collision.gameObject.CompareTag("Lava"))
        {
            TakeDamage(maxHP, true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyBullet"))
        {
            Bullet bullet = collision.GetComponent<Bullet>();

            TakeDamage(bullet.Damage);
        }
        else if (collision.CompareTag("RestartPoint"))
        {
            PlayerRestartPointManager.Instance.RestartPoint = collision.transform.position;
            PlayerRestartPointManager.Instance.isUpdateRestartPos = true;
        }
    }

}
