using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decoration : MonoBehaviour, IPoolObject
{

    //Ссылки для инициализации и генерации
    //Начальная и конечная точки чанков
    [SerializeField] Transform startPoint;
    [SerializeField] Transform endPoint;
    //Ссылка на основные настройки
    [SerializeField] GameSettings gameSettings;
    [SerializeField] public string type;
    [SerializeField] public int sizePercent;

    //Получение начальной и конечной точки чанков
    public Transform StartPosition { get { return startPoint; } }
    public Transform EndPosition { get { return endPoint; } }
    //Текущий пул объекта
    public int poolNum { get; set; }

    //Обработка физики
    //Текущая скорость чанка
    [SerializeField] float speed;
    public float Speed => speed;

    Vector3 standartSize;

    public void Start()
    {
        this.speed = gameSettings.runnerSpeed;
    }

    //Функция вызываемая с определённым временным промежутком
    public void FixedUpdate()
    {
        transform.position += new Vector3(0, 0, -speed * Time.deltaTime);
    }

    //Изменение скорости чанка
    public void ChangeSpeed(float speed)
    {
        this.speed = speed;
    }

    //Инициализация чанка
    public void Initialize()
    {
        //Debug.Log("Chunk Initialized");
        this.speed = gameSettings.runnerSpeed;
        if (type == "tree" || type == "bush")
        { 
            standartSize = transform.localScale;

            var randSize = standartSize * Random.Range(100 - sizePercent, 100 + sizePercent) * 0.01f;
            transform.localScale = randSize;

        }
    }

    //Очистка данных при освобождении чанка
    public void Release()
    {

        //Debug.Log("Chunk Released");
        if (type == "tree" || type == "bush")
        {
            transform.localScale = standartSize;
        }
    }
}
