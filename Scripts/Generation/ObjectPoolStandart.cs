using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolStandart<T> where T : Component, IPoolObject
{
    //������ �� ������ ����������������� �������
    public GameObject prefab;
    //������� ������ ����, ����� ������ ����� ����������� ����������
    public int initialPoolSize = 10;
    //������������ ������ ����, �������� ���� ����� ���������
    public int maxPoolSize = 20;
    //������� ������� ���� � ������
    private int currentPoolNum = 0;

    //��� ��������
    private ObjectPool<T> pool;

    // ������ ���� ��������� ��������
    private List<T> allObjects = new List<T>();

    //������������� ���� ��������
    public void Setup(GameObject prefab, int initialSize, int maxSize, int currentPoolNum)
    {
        //�������� ����������
        this.prefab = prefab;
        this.initialPoolSize = initialSize;
        this.maxPoolSize = maxSize;
        this.currentPoolNum = currentPoolNum;

        //�������� ���� ��������
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

    //�������� ������� ���� � ������� ��� ������������ ����������
    private T CreatePooledItem()
    {
        GameObject obj = Object.Instantiate(prefab);
        obj.SetActive(false);
        var objScr = obj.GetComponent<T>();
        objScr.poolNum = currentPoolNum;


        allObjects.Add(objScr); // ��������� ������ � ������ ���������

        return objScr;
    }

    //������ ��� ������ �������� �� ����
    private void OnTakeFromPool(T obj)
    {
        obj.gameObject.SetActive(true);
        obj.Initialize();
    }

    //������ ��� ����������� ������� � ���
    private void OnReturnedToPool(T obj)
    {
        obj.Release();
        obj.gameObject.SetActive(false);
    }

    //������ ��� ����������� ������� � ����
    private void OnDestroyPoolObject(T obj)
    {
        Object.Destroy(obj.gameObject);
    }

    // ����� �������� ���� ������� ��������
    public void ClearPool()
    {
        pool.Clear();
        allObjects.Clear();
    }


    // ����� ���������� ���� ����������� �������� ����
    public List<T> GetObjects()
    {
        // ��������� ��� ������� �� ���� � ���������� ��
        return allObjects;
    }


    //��������� ���������� ������� ����
    public T Get()
    {
        return pool.Get();
    }

    //������������ ����������� ������� ����
    public void Release(T obj)
    {
        pool.Release(obj);
    }

    //��������� �������� �� ��� ������
    public bool Contains(T obj)
    {
        return allObjects.Contains(obj);
    }


    // ����� ��� ��������� ���� �������� ����
    public void ModifyAllObjects(System.Action<T> modificationAction)
    {
        foreach (var obj in allObjects)
        {
            modificationAction(obj);
        }
    }
}
