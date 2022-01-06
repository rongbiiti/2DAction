using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VirtualFireButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public int fire;

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        fire = 1;
    }

    public void OnPointerUp(PointerEventData pointerEventData)
    {
        fire = 0;
    }

    
}
