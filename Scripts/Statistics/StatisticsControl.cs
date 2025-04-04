using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CoinUpdateEvent : UnityEvent<int> { }

public class StatisticsControl : MonoBehaviour
{
    //События
    //Событие вызываемое на измениение количества монет
    public CoinUpdateEvent OnCoinUpdate = new CoinUpdateEvent();

    [Header("Ссылки")]
    [SerializeField] GameSettings gameSettings;

    [Header("Монеты")]
    //Все монеты игрока
    [SerializeField] int coins;
    public int Coins => coins;
    //Текущие монеты в раунде
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


    [Header("Расстояние")]
    //Текущий рекорд расстояния игрока
    [SerializeField] double maxDistance;
    public double MaxDistance => maxDistance;
    //Текущее расстояние
    double currentDistance;
    public double CurrentDistance => currentDistance;

    [Header("Раннер")]
    bool isRunning;
    public bool IsRunning
    {
        get
        {  return isRunning; }
        set 
        { isRunning = value; }
    }

    //Функция запускаемная вначале программы
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

    //Функция вызываемая при начале забега
    public void StartRunning()
    {
        currentCoins = 0;
        currentDistance = 0;

        isRunning = true;

        //Добавление подписки на получение монет
        PlayerCollisionControl.OnCoinTouch.AddListener(CoinCollision);
        //Добавление подписки на смерть
        PlayerMovement.OnElimination.AddListener(StopPlayer);
    }

    //Вункция вызываемая при конце забега
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

    //Действия выполянемые на столкновения с монетами разных типов
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

    //Установить текущий результат монет как финальный и добавить ко всем монетам
    public void SetResult()
    {
        coins += currentCoins;
        if (currentDistance > maxDistance)
            maxDistance = currentDistance;
    }
}
