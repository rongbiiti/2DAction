using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VirtualPad : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public RectTransform padBackGround;
    public RectTransform pad;
    public float _horizontal = 0;
    public float _vertical = 0;
    public float _offset = 0;
    private Vector2 pointPosition;

    public void OnBeginDrag(PointerEventData eventData)
    {

    }

    public void OnDrag(PointerEventData eventData)
    {

        pointPosition = new Vector2((eventData.position.x - padBackGround.position.x) / ((padBackGround.rect.size.x - pad.rect.size.x) / 2), (eventData.position.y - padBackGround.position.y) / ((padBackGround.rect.size.y - pad.rect.size.y) / 2));


        if(pointPosition.magnitude > 1.0f)
        {
            pointPosition = pointPosition.normalized;
        }

        pad.transform.position = new Vector2((pointPosition.x * ((padBackGround.rect.size.x - pad.rect.size.x) / 2) * _offset) + padBackGround.position.x, (pointPosition.y * ((padBackGround.rect.size.y - pad.rect.size.y) / 2) * _offset) + padBackGround.position.y);

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        pointPosition = new Vector2(0f, 0f);
        pad.transform.position = padBackGround.position;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnEndDrag(eventData);
    }

    void Update()
    {
        _horizontal = pointPosition.x;
        _vertical = pointPosition.y;
    }

    public Vector2 Coordinate()
    {
        return new Vector2(_horizontal, _vertical);
    }
}
