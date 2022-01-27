using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class TypeWriterText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textObj = default;
    private float _feedTime = 0.1f; // �������莞��
    private float _t = 0f;
    private int _visibleLen = 0;
    private int _textLen = 0;

    private void Start()
    {
        _visibleLen = 0;
        StartCoroutine(nameof(EffectCoroutine));
        SoundManager.Instance.PlaySE(SE.StageStart);
    }

    //�e�L�X�g��ݒ�
    public void SetText(string text)
    {
        textObj.text = text;
        _textLen = text.Length;
        _visibleLen = 0;
        _t = 0;
        textObj.maxVisibleCharacters = 0; // �\�����������O��
    }

    private IEnumerator EffectCoroutine()
    {
        yield return new WaitForSeconds(3.5f);
        SetText("DOC ROBOT");

        yield return new WaitForSeconds(5.5f);
        SceneManager.LoadScene(1);
    }

    private void Update()
    {
        if (_visibleLen < _textLen)
        {
            _t += Time.deltaTime;
            if (_t >= _feedTime)
            {
                _t -= _feedTime;
                _visibleLen++;
                textObj.maxVisibleCharacters = _visibleLen; // �\����1���������₷
            }
        }
    }
}
