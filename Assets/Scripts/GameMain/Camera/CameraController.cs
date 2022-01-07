using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform _playerTransform;

    private Camera myCamera;
    private Vector3 playerScreenPos;
    private Vector3 velocity;

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

        myCamera = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        playerScreenPos = myCamera.WorldToViewportPoint(_playerTransform.position);
        bool followFlag = false;
        float followX = playerScreenPos.x;
        float followY = playerScreenPos.y;

        if(0.499f <= playerScreenPos.x)
        {
            followFlag = true;
            followX = 0.499f;
        }

        if(0.67f <= playerScreenPos.y)
        {
            followFlag = true;
            followY = 0.67f;
        }
        else if(playerScreenPos.y <= 0.23f)
        {
            followFlag = true;
            followY = 0.23f;
        }

        if (followFlag)
        {
            Vector3 delta = _playerTransform.position - myCamera.ViewportToWorldPoint(new Vector3(followX, followY, playerScreenPos.z));
            Vector3 destination = transform.position + delta;

            transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, 0.05f);
        }
    }
}
