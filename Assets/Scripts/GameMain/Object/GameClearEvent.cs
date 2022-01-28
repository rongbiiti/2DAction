using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameClearEvent : MonoBehaviour
{
    [SerializeField] private PlayerHealth _playerHealth;
    [SerializeField] private GameObject _clearText;

    public void EventStart()
    {
        StartCoroutine(nameof(ClearEvent));
    }

    private IEnumerator ClearEvent()
    {
        // プレイヤーが死なないようにする
        _playerHealth.isCleared = true;
        yield return new WaitForSeconds(1.5f);

        // Clear!のテキスト表示
        _clearText.SetActive(true);
        SoundManager.Instance.PlaySE(SE.Victory);
        yield return new WaitForSeconds(6.5f);

        _playerHealth.GameRestart();
    }
}
