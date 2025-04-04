using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour, IPoolObject
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
    //Коллекционные объекты чанка
    public List<Collectable> chunkCollectables;
    //Барьеры чанка
    public List<Barrier> chunkBarriers;
    //Декорации чанка
    public List<Decoration> chunkDecorations;

    //Обработка физики
    //Текущая скорость чанка
    [SerializeField] float speed;
    public float Speed => speed;

    public void Start()
    {
        this.speed = gameSettings.runnerSpeed;
    }

    //Функция вызываемая с определённым временным промежутком
    public void FixedUpdate()
    {
        transform.position += new Vector3(0,0, -speed * Time.deltaTime);
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
    }

    //Очистка данных при освобождении чанка
    public void Release()
    {
        //Debug.Log("Chunk Released");
    }

}
