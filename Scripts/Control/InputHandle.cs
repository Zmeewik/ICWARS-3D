using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//������� � ������� ����������
[System.Serializable]
public class SwipeEvent : UnityEvent<Vector2> { }

public class InputHandle : MonoBehaviour
{
    //������� ��������� ������
    public static SwipeEvent OnSwipe = new SwipeEvent();
    //������� ��������� �������� �������
    public static UnityEvent OnDoubleTouch = new UnityEvent();

    //��������� �������
    //���� �������� ��������� �� ����������
    bool isMobile = false;
    //�������� ��������� �� ����� � �������� ������
    bool isSwiping = false;


    //����������� �������� ��������� �����
    Vector2 tapPosition = Vector2.zero;
    //������� ���������� ������
    Vector2 swipeDelta = Vector2.zero;
    //����������� ���������� ��� ������
    [SerializeField] float swipeDeadZone;


    //������� ���������� ��� ������� ����������
    private void Start()
    {
        isMobile = Application.isMobilePlatform;
    }

    //������� ���������� ������ ���� ������ ����������
    private void Update()
    {
        Swipe();
    }

    //��������� ���� ������
    private void Swipe()
    {
        //���������� ���� � ����������
        if (!isMobile)
        {
            if (Input.GetMouseButtonDown(0))
            {
                isSwiping = true;
                tapPosition = Input.mousePosition;

            }
            else if (Input.GetMouseButtonUp(0))
            {
                ResetSwipe();
            }
        }
        //���������� ���� � ���������� ����������
        else
        {
            if (Input.touchCount > 0)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    isSwiping = true;
                    tapPosition = Input.GetTouch(0).position;
                }
                else if (Input.GetTouch(0).phase == TouchPhase.Canceled ||
                    Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                    ResetSwipe();
                }
            }
        }

        if (isSwiping)
        {
            CheckForSwipe();
        }
    }

    //�������� ���������� ������
    private void ResetSwipe()
    {
        isSwiping = false;

        swipeDelta = Vector2.zero;
        tapPosition = Vector2.zero;
    }

    //��������� ����� � ������������
    private void CheckForSwipe()
    {
        //�������� ������� �������� ����������� �������/������ �� ����������
        if (!isMobile && Input.GetMouseButton(0))
            swipeDelta = (Vector2)Input.mousePosition - tapPosition;
        else if (Input.touchCount > 0)
            swipeDelta = Input.GetTouch(0).position - tapPosition;

        //���� ����������� ������ ������������ ��� ��������� �������� �������
        if (swipeDelta.magnitude > swipeDeadZone)
        {
            if (OnSwipe != null)
            {
                //���� ���������� x ������ ��������� y, �� ����� ��������������
                if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
                    OnSwipe.Invoke(swipeDelta.x > 0 ? Vector2.right : Vector2.left);
                //���� ���������� y ������ ��������� x, �� ����� �������������
                else
                    OnSwipe.Invoke(swipeDelta.y > 0 ? Vector2.up : Vector2.down);

                ResetSwipe();
            }
        }
    }

    //���������� ���� �������� �������
    private void DoubleTouch()
    { 
        
    }
}
