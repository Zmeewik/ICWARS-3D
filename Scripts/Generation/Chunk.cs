using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour, IPoolObject
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
    //������������� ������� �����
    public List<Collectable> chunkCollectables;
    //������� �����
    public List<Barrier> chunkBarriers;
    //��������� �����
    public List<Decoration> chunkDecorations;

    //��������� ������
    //������� �������� �����
    [SerializeField] float speed;
    public float Speed => speed;

    public void Start()
    {
        this.speed = gameSettings.runnerSpeed;
    }

    //������� ���������� � ����������� ��������� �����������
    public void FixedUpdate()
    {
        transform.position += new Vector3(0,0, -speed * Time.deltaTime);
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
    }

    //������� ������ ��� ������������ �����
    public void Release()
    {
        //Debug.Log("Chunk Released");
    }

}
