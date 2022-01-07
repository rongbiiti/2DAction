using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KariEnemy : Enemy
{
    [SerializeField]
    [Tooltip("X²•ûŒü‚ÉˆÚ“®‚·‚éU•(0‚É‚·‚é‚ÆˆÚ“®‚µ‚È‚¢)")]
    private float amplitudeX = 4.0f;

    [SerializeField]
    [Tooltip("Y²•ûŒü‚ÉˆÚ“®‚·‚éU•(0‚É‚·‚é‚ÆˆÚ“®‚µ‚È‚¢)")]
    private float amplitudeY = 4.0f;

    [SerializeField]
    [Tooltip("Z²•ûŒü‚ÉˆÚ“®‚·‚éU•(0‚É‚·‚é‚ÆˆÚ“®‚µ‚È‚¢)")]
    private float amplitudeZ = 4.0f;

    [SerializeField]
    [Tooltip("U•‚·‚é‘¬‚³")]
    private float amplitudeSpeed = 2.0f;

    /// <summary>
    /// ƒQ[ƒ€ŠJn‚ÌêŠ
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
