using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CoinUpdateEvent : UnityEvent<int> { }

public class StatisticsControl : MonoBehaviour
{
    //�������
    //������� ���������� �� ���������� ���������� �����
    public CoinUpdateEvent OnCoinUpdate = new CoinUpdateEvent();

    [Header("������")]
    [SerializeField] GameSettings gameSettings;

    [Header("������")]
    //��� ������ ������
    [SerializeField] int coins;
    public int Coins => coins;
    //������� ������ � ������
    int currentCoins;
    public int CurrentCoins
    {
        get
        {
            return currentCoins;
        }
        set
        {
            currentCoins = value;
            Debug.Log("Coin added");
            OnCoinUpdate.Invoke(currentCoins);
        }
    }


    [Header("����������")]
    //������� ������ ���������� ������
    [SerializeField] double maxDistance;
    public double MaxDistance => maxDistance;
    //������� ����������
    double currentDistance;
    public double CurrentDistance => currentDistance;

    [Header("������")]
    bool isRunning;
    public bool IsRunning
    {
        get
        {  return isRunning; }
        set 
        { isRunning = value; }
    }

    //������� ������������ ������� ���������
    private void Start()
    {
        
    }

    private void FixedUpdate()
    {
        if (isRunning)
        {
            currentDistance += gameSettings.runnerSpeed * Time.deltaTime / 10;
        }
    }

    //������� ���������� ��� ������ ������
    public void StartRunning()
    {
        currentCoins = 0;
        currentDistance = 0;

        isRunning = true;

        //���������� �������� �� ��������� �����
        PlayerCollisionControl.OnCoinTouch.AddListener(CoinCollision);
        //���������� �������� �� ������
        PlayerMovement.OnElimination.AddListener(StopPlayer);
    }

    //������� ���������� ��� ����� ������
    public void StopRunning()
    {
        isRunning = false;
        PlayerMovement.OnElimination.RemoveListener(StopPlayer);
        PlayerCollisionControl.OnCoinTouch.RemoveListener(CoinCollision);
    }

    public void StopPlayer()
    {
        isRunning = false;
    }

    //�������� ����������� �� ������������ � �������� ������ �����
    private void CoinCollision(Collectable coin)
    {
        switch (coin.Type)
        {
            case "V":
                CurrentCoins++;
                break;
            case "C":
                CurrentCoins += 2;
                break;
            case "S":
                CurrentCoins += 3;
                break;
            case "G":
                CurrentCoins += 5;
                break;
        }
    }

    //���������� ������� ��������� ����� ��� ��������� � �������� �� ���� �������
    public void SetResult()
    {
        coins += currentCoins;
        if (currentDistance > maxDistance)
            maxDistance = currentDistance;
    }
}
