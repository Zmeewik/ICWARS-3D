using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolStandart<T> where T : Component, IPoolObject
{
    //Ссылка на префаб инициализируемого объекта
    public GameObject prefab;
    //Средний размер пула, такой размер будет сохдаваться изначально
    public int initialPoolSize = 10;
    //Максимальный размер пула, значения выше будут очищаться
    public int maxPoolSize = 20;
    //Текущий номерпр пула в списке
    private int currentPoolNum = 0;

    //Пул объектов
    private ObjectPool<T> pool;

    // Список всех созданных объектов
    private List<T> allObjects = new List<T>();

    //инициализация пула объектов
    public void Setup(GameObject prefab, int initialSize, int maxSize, int currentPoolNum)
    {
        //Передача переменных
        this.prefab = prefab;
        this.initialPoolSize = initialSize;
        this.maxPoolSize = maxSize;
        this.currentPoolNum = currentPoolNum;

        //Создание пула объектов
        pool = new ObjectPool<T>(
                    CreatePooledItem,
                    OnTakeFromPool,
                    OnReturnedToPool,
                    OnDestroyPoolObject,
                    collectionCheck: false,
                    defaultCapacity: initialPoolSize,
                    maxSize: maxPoolSize
                );
    }

    //Создание объекта пула и возврат его необходимого компонента
    private T CreatePooledItem()
    {
        GameObject obj = Object.Instantiate(prefab);
        obj.SetActive(false);
        var objScr = obj.GetComponent<T>();
        objScr.poolNum = currentPoolNum;


        allObjects.Add(objScr); // Добавляем объект в список созданных

        return objScr;
    }

    //Логика при выборе объектта из пула
    private void OnTakeFromPool(T obj)
    {
        obj.gameObject.SetActive(true);
        obj.Initialize();
    }

    //Логика при возвращении объекта в пул
    private void OnReturnedToPool(T obj)
    {
        obj.Release();
        obj.gameObject.SetActive(false);
    }

    //Логика при уничтожении объекта в пуле
    private void OnDestroyPoolObject(T obj)
    {
        Object.Destroy(obj.gameObject);
    }

    // Метод отчистки всех списков объектов
    public void ClearPool()
    {
        pool.Clear();
        allObjects.Clear();
    }


    // Метод пполучения всех экземпляров объектов пула
    public List<T> GetObjects()
    {
        // Извлекаем все объекты из пула и уничтожаем их
        return allObjects;
    }


    //Получение случайного объекта пула
    public T Get()
    {
        return pool.Get();
    }

    //Освобождение некотторого объекта пула
    public void Release(T obj)
    {
        pool.Release(obj);
    }

    //Проверить содержит ли пул объект
    public bool Contains(T obj)
    {
        return allObjects.Contains(obj);
    }


    // Метод для изменения всех объектов пула
    public void ModifyAllObjects(System.Action<T> modificationAction)
    {
        foreach (var obj in allObjects)
        {
            modificationAction(obj);
        }
    }
}
