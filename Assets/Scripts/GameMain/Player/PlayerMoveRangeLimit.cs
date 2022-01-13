using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveRangeLimit : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;

    void Start()
    {
        // null‚¾‚Á‚½‚çŒŸõ‚µ‚Äæ“¾
        if (_mainCamera == null)
        {
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        }
    }

    private void FixedUpdate()
    {
        Vector3 myViewPortPos = _mainCamera.WorldToViewportPoint(transform.position);

        if (myViewPortPos.x <= 0.05f)
        {
            Vector3 worldPos = _mainCamera.ViewportToWorldPoint(new Vector3(0.05f, myViewPortPos.y, myViewPortPos.z));
            worldPos.y = transform.position.y;
            transform.position = worldPos;
        }

        if(myViewPortPos.y <= -0.1f)
        {
            GetComponent<PlayerHealth>().TakeDamage(100);
        }
    }

}
