using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KariEnemy : Enemy
{
    [SerializeField]
    [Tooltip("X軸方向に移動する振幅(0にすると移動しない)")]
    private float amplitudeX = 4.0f;

    [SerializeField]
    [Tooltip("Y軸方向に移動する振幅(0にすると移動しない)")]
    private float amplitudeY = 4.0f;

    [SerializeField]
    [Tooltip("Z軸方向に移動する振幅(0にすると移動しない)")]
    private float amplitudeZ = 4.0f;

    [SerializeField]
    [Tooltip("振幅する速さ")]
    private float amplitudeSpeed = 2.0f;

    /// <summary>
    /// ゲーム開始時の場所
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
