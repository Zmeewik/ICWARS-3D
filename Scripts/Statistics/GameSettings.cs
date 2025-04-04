using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Settings/GameSettings")]
public class GameSettings : ScriptableObject
{
    //������� ��������� ����
    [Header("Runner")]
    //��������
    public float minSpeed;    //����������� �������� ����������� ���� � �������
    public float maxSpeed;    //������������ �������� ����������� ���� � �������
    public float changeSpeed; //�������� ��������� �������� � �������
    public float runnerSpeed; //�������� ����������� ���� � �������
    //�����
    public float lineRange;   //���������� �� �����
}
