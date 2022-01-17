using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRoomShutter : MonoBehaviour
{
    [SerializeField] private GameObject _boss;
    private SpriteRenderer[] shutterSprites;

    private void Start()
    {
        shutterSprites = GetComponentsInChildren<SpriteRenderer>();
    }

    public void ShutterOff()
    {
        StartCoroutine(nameof(ShutterOffCoroutine));
    }

    public void ShutterOn()
    {
        StartCoroutine(nameof(ShutterOnCoroutine));
    }

    private IEnumerator ShutterOffCoroutine()
    {
        for(int i = shutterSprites.Length - 1; i >= 0; i--)
        {
            shutterSprites[i].enabled = false;
            yield return new WaitForSecondsRealtime(0.1f);
        }

    }

    private IEnumerator ShutterOnCoroutine()
    {
        for(int i = 0; i < shutterSprites.Length; i++)
        {
            shutterSprites[i].enabled = true;
            yield return new WaitForSecondsRealtime(0.1f);
        }
        _boss.SetActive(true);
        //FindObjectOfType<Enemy_Boss>().gameObject.SetActive(true);
    }
}
