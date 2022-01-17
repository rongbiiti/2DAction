using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerZankiManager : SingletonMonoBehaviour<PlayerZankiManager>
{
    // ‰Šúc‹@
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
