using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class Barrier : MonoBehaviour, IPoolObject
{
    //������ ��� ������������� � ���������
    //��������� � �������� ����� ������
    [SerializeField] Transform startPoint;
    [SerializeField] Transform endPoint;
    //������ �� �������� ���������
    [SerializeField] GameSettings gameSettings;

    //��������� ��������� � �������� ����� ������
    public Transform StartPosition { get { return startPoint; } }
    public Transform EndPosition { get { return endPoint; } }

    //������� ��� �������
    public int poolNum { get; set; }
    //������������� �����
    public void Initialize()
    {
        //Debug.Log("Chunk Initialized");
        this.speed = gameSettings.runnerSpeed;
    }

    //������� ������ ��� ������������ �����
    public void Release()
    {
        //Debug.Log("Chunk Released");
    }



    //��������� ������
    //������� �������� �����
    [SerializeField] float speed;
    public float Speed => speed;

    //������� ���������� � ����������� ��������� �����������
    public void FixedUpdate()
    {
        transform.position += new Vector3(0, 0, -speed * Time.deltaTime);
    }

    public void ChangeSpeed(float speed)
    {
        this.speed = speed;
    }
}
