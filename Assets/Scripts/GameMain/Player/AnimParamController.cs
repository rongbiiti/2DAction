using UnityEngine;

public class AnimParamController : MonoBehaviour
{
    [SerializeField] private Animator _myAnimator;

    private void Awake()
    {
        // nullだったら取得
        if(_myAnimator == null)
        {
            _myAnimator = GetComponent<Animator>();
        }
        
    }

    // パラメーターの名前を指定して代入
    public void SetAnimParamBool(string paramName, bool value)
    {
        _myAnimator.SetBool(paramName, value);
    }

    // パラメーターのIDを指定して代入
    public void SetAnimParamBool(int id, bool value)
    {
        _myAnimator.SetBool(id, value);
    }

    
}
