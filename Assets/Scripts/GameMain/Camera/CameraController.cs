using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform _playerTransform;

    private Camera myCamera;
    public Camera Camera {
        get { return myCamera; }
    }

    private Vector3 playerViewPortPos;
    public Vector3 PlayerViewPortPos {
        get { return playerViewPortPos; }
    }

    private Vector3 velocity;
    private PlayerController _playerController;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
    }

    void Start()
    {
        // nullÇæÇ¡ÇΩÇÁåüçıÇµÇƒéÊìæ
        if(_playerTransform == null)
        {
            _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }

        _playerController = _playerTransform.GetComponent<PlayerController>();
        myCamera = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        playerViewPortPos = myCamera.WorldToViewportPoint(_playerTransform.position);
        bool followFlag = false;
        float followX = playerViewPortPos.x;
        float followY = playerViewPortPos.y;

        if(0.499f <= playerViewPortPos.x)
        {
            followFlag = true;
            followX = 0.499f;
        }

        if(0.67f <= playerViewPortPos.y)
        {
            followFlag = true;
            followY = 0.67f;
        }
        else if(playerViewPortPos.y <= 0.23f && !_playerController.IsFalling)
        {
            followFlag = true;
            followY = 0.23f;
        }

        if (followFlag)
        {
            Vector3 delta = _playerTransform.position - myCamera.ViewportToWorldPoint(new Vector3(followX, followY, playerViewPortPos.z));
            Vector3 destination = transform.position + delta;

            transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, 0.05f);
        }
    }
}
