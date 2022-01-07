using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KariEnemy : Enemy
{
    [SerializeField]
    [Tooltip("X�������Ɉړ�����U��(0�ɂ���ƈړ����Ȃ�)")]
    private float amplitudeX = 4.0f;

    [SerializeField]
    [Tooltip("Y�������Ɉړ�����U��(0�ɂ���ƈړ����Ȃ�)")]
    private float amplitudeY = 4.0f;

    [SerializeField]
    [Tooltip("Z�������Ɉړ�����U��(0�ɂ���ƈړ����Ȃ�)")]
    private float amplitudeZ = 4.0f;

    [SerializeField]
    [Tooltip("�U�����鑬��")]
    private float amplitudeSpeed = 2.0f;

    /// <summary>
    /// �Q�[���J�n���̏ꏊ
    /// </summary>
    private Vector3 startPosition;
    protected override void Start()
    {
        base.Start();
        startPosition = transform.position;

    }

    protected override void Update()
    {
        base.Update();
        transform.position = new Vector3((Mathf.Sin((Time.time) * amplitudeSpeed) * amplitudeX + startPosition.x), (Mathf.Sin((Time.time) * amplitudeSpeed) * amplitudeY + startPosition.y), (Mathf.Sin((Time.time) * amplitudeSpeed) * amplitudeZ + startPosition.z));
    }
}
