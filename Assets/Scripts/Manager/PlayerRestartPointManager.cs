using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRestartPointManager : SingletonMonoBehaviour<PlayerRestartPointManager>
{
    public Vector3 RestartPoint;
    public bool isUpdateRestartPos;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

}
