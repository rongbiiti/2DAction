using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VirtualDashButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public int dash;

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        dash = 1;
    }

    public void OnPointerUp(PointerEventData pointerEventData)
    {
        dash = 0;
    }

}
