using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LockonCursor : MonoBehaviour
{
    private Camera mainCamera;
    private Image myImage;

    private Transform lockonEnemy;

    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        }

        myImage = GetComponent<Image>();
    }

    public void SetImageEnabled(bool flag, Transform targetEnemy)
    {
        if (flag && !myImage.enabled)
        {
            myImage.enabled = true;
            
        }
        else if(!flag && myImage.enabled)
        {
            myImage.enabled = false;
        }

        if(targetEnemy != null)
        {
            lockonEnemy = targetEnemy;
            transform.position = mainCamera.WorldToScreenPoint(lockonEnemy.transform.position);
        }
    }

    void Update()
    {
        if(lockonEnemy != null)
        {
            transform.position = mainCamera.WorldToScreenPoint(lockonEnemy.transform.position);
        }
    }
}
