using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class Barrier : MonoBehaviour, IPoolObject
{
    //Ссылки для инициализации и генерации
    //Начальная и конечная точки чанков
    [SerializeField] Transform startPoint;
    [SerializeField] Transform endPoint;
    //Ссылка на основные настройки
    [SerializeField] GameSettings gameSettings;

    //Получение начальной и конечной точки чанков
    public Transform StartPosition { get { return startPoint; } }
    public Transform EndPosition { get { return endPoint; } }

    //Текущий пул объекта
    public int poolNum { get; set; }
    //Инициализация чанка
    public void Initialize()
    {
        //Debug.Log("Chunk Initialized");
        this.speed = gameSettings.runnerSpeed;
    }

    //Очистка данных при освобождении чанка
    public void Release()
    {
        //Debug.Log("Chunk Released");
    }



    //Обработка физики
    //Текущая скорость чанка
    [SerializeField] float speed;
    public float Speed => speed;

    //Функция вызываемая с определённым временным промежутком
    public void FixedUpdate()
    {
        transform.position += new Vector3(0, 0, -speed * Time.deltaTime);
    }

    public void ChangeSpeed(float speed)
    {
        this.speed = speed;
    }
}
