using UnityEngine;

public class AnimParamController : MonoBehaviour
{
    [SerializeField] private Animator _myAnimator;

    private void Awake()
    {
        // null��������擾
        if(_myAnimator == null)
        {
            _myAnimator = GetComponent<Animator>();
        }
        
    }

    // �p�����[�^�[�̖��O���w�肵�đ��
    public void SetAnimParamBool(string paramName, bool value)
    {
        _myAnimator.SetBool(paramName, value);
    }

    // �p�����[�^�[��ID���w�肵�đ��
    public void SetAnimParamBool(int id, bool value)
    {
        _myAnimator.SetBool(id, value);
    }

    
}
