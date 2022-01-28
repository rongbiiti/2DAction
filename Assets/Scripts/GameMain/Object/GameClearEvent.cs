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
        // �v���C���[�����ȂȂ��悤�ɂ���
        _playerHealth.isCleared = true;
        yield return new WaitForSeconds(1.5f);

        // Clear!�̃e�L�X�g�\��
        _clearText.SetActive(true);
        SoundManager.Instance.PlaySE(SE.Victory);
        yield return new WaitForSeconds(6.5f);

        _playerHealth.GameRestart();
    }
}
