using UnityEngine.Pool;
using UnityEngine;

public interface IPoolObject
{
    //������� ����� ���� �������
    int poolNum { get; set; }
    //������� ������������� �������
    void Initialize();
    //������� ������������ ��������
    void Release();
}
