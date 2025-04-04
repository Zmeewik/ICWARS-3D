using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decoration : MonoBehaviour, IPoolObject
{

    //������ ��� ������������� � ���������
    //��������� � �������� ����� ������
    [SerializeField] Transform startPoint;
    [SerializeField] Transform endPoint;
    //������ �� �������� ���������
    [SerializeField] GameSettings gameSettings;
    [SerializeField] public string type;
    [SerializeField] public int sizePercent;

    //��������� ��������� � �������� ����� ������
    public Transform StartPosition { get { return startPoint; } }
    public Transform EndPosition { get { return endPoint; } }
    //������� ��� �������
    public int poolNum { get; set; }

    //��������� ������
    //������� �������� �����
    [SerializeField] float speed;
    public float Speed => speed;

    Vector3 standartSize;

    public void Start()
    {
        this.speed = gameSettings.runnerSpeed;
    }

    //������� ���������� � ����������� ��������� �����������
    public void FixedUpdate()
    {
        transform.position += new Vector3(0, 0, -speed * Time.deltaTime);
    }

    //��������� �������� �����
    public void ChangeSpeed(float speed)
    {
        this.speed = speed;
    }

    //������������� �����
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

    //������� ������ ��� ������������ �����
    public void Release()
    {

        //Debug.Log("Chunk Released");
        if (type == "tree" || type == "bush")
        {
            transform.localScale = standartSize;
        }
    }
}
