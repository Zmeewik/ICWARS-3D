using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//Событие с входным аргументом
[System.Serializable]
public class SwipeEvent : UnityEvent<Vector2> { }

public class InputHandle : MonoBehaviour
{
    //Событие обработки свайпа
    public static SwipeEvent OnSwipe = new SwipeEvent();
    //Событие обработки двойного нажатия
    public static UnityEvent OnDoubleTouch = new UnityEvent();

    //Обработка касаний
    //Флаг проверки мобильное ли устройство
    bool isMobile = false;
    //Проверка находится ли игрок в процессе свайпа
    bool isSwiping = false;


    //Определение текущего положения клика
    Vector2 tapPosition = Vector2.zero;
    //Текущее расстояние свайпа
    Vector2 swipeDelta = Vector2.zero;
    //Минимальное расстояние для свайпа
    [SerializeField] float swipeDeadZone;


    //Функция вызываемая при запуске приложения
    private void Start()
    {
        isMobile = Application.isMobilePlatform;
    }

    //Функция вызываемая каждый кадр работы приложения
    private void Update()
    {
        Swipe();
    }

    //Проверить ввод свайпа
    private void Swipe()
    {
        //Обработать ввод с компьютера
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
        //Обработать ввод с мобильного устройства
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

    //Обнулить переменные свайпа
    private void ResetSwipe()
    {
        isSwiping = false;

        swipeDelta = Vector2.zero;
        tapPosition = Vector2.zero;
    }

    //Проверить свайп и активировать
    private void CheckForSwipe()
    {
        //Получаем текущее значение перемещения курсора/пальца от стартового
        if (!isMobile && Input.GetMouseButton(0))
            swipeDelta = (Vector2)Input.mousePosition - tapPosition;
        else if (Input.touchCount > 0)
            swipeDelta = Input.GetTouch(0).position - tapPosition;

        //Если перемещение больше минимального для обработки вызываем событие
        if (swipeDelta.magnitude > swipeDeadZone)
        {
            if (OnSwipe != null)
            {
                //Если ищзменение x больше изменения y, то свайп горизонтальный
                if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
                    OnSwipe.Invoke(swipeDelta.x > 0 ? Vector2.right : Vector2.left);
                //Если ищзменение y больше изменения x, то свайп вертиккальный
                else
                    OnSwipe.Invoke(swipeDelta.y > 0 ? Vector2.up : Vector2.down);

                ResetSwipe();
            }
        }
    }

    //Провериить ввод двойного нажатия
    private void DoubleTouch()
    { 
        
    }
}
