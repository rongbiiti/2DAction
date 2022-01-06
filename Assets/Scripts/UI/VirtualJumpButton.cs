using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VirtualJumpButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler
{
    public int jump;

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        jump = 1;
    }

    public void OnPointerUp(PointerEventData pointerEventData)
    {
        jump = 0;
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if(InputManager.Instance.DashKey == 1)
        {
            StartCoroutine(nameof(DashJump));
        }
    }

    private IEnumerator DashJump()
    {
        jump = 1;
        yield return null;
        jump = 0;
    }

}
