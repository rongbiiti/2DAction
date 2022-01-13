using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerZankiManager : SingletonMonoBehaviour<PlayerZankiManager>
{
    // 残機最大数
    [SerializeField] private int _maxZanki = 99;

    // 初期残機
    [SerializeField] private int _startZanki = 3;

    private int zanki;
    public int Zanki {
        get { return zanki; }
        set { zanki = value; }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        ZankiReset();
    }

    public void ZankiReset()
    {
        zanki = _startZanki;
    }
}
