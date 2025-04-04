using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class UIControl : MonoBehaviour
{
    //Ссылки
    [Header("Ссылки")]
    [SerializeField] StatisticsControl statisticsControl;   //Контроль статистики игры
    [SerializeField] StateMachine st_machine;               //Машина состояний
    [SerializeField] TextPopupControl textPopupControl;     //Контроль выскакивающего текста

    [Header("Раннер")]
    [SerializeField] TextMeshProUGUI coinCounter;           //Текст счётчик монет
    [SerializeField] TextMeshProUGUI distanceCounter;       //Текст счётчик расстояния
    [SerializeField] TextMeshProUGUI healthCounter;         //Текст счётчик здоровья
    [SerializeField] TextMeshProUGUI hiscoreDistance;       //Текст счётчик рекорда
    [SerializeField] GameObject runnerUI;                   //Объект UI элементов раннера

    [Header("Пауза")]
    [SerializeField] GameObject pauseUI;                    //Объект UI элементов паузы
    [SerializeField] TextMeshProUGUI pauseStatCounter;      //Текст счётчик расстояния, монет, рекорда паузы

    [Header("Статистика")]
    [SerializeField] GameObject statisticsUI;               //Объект UI элементов финальной статистики
    [SerializeField] TextMeshProUGUI statsStatCounter;  //Текст счётчик расстояния, монет, рекорда статистики

    [Header("Меню")]
    [SerializeField] GameObject menuUI;                     //Объект UI элементов меню

    [Header("Верхняя статистика")]
    [SerializeField] GameObject upStatsUI;                  //Объект UI верхней статистики
    [SerializeField] TextMeshProUGUI upCoinCounter;         //Текст счётчик всех монет

    [Header("Магазин")]
    [SerializeField] GameObject storeHatUI;                 //Объект UI элементов магазина шляп
    [SerializeField] GameObject storeSkinUI;                //Объект UI элементов магазина образов
    [SerializeField] GameObject storeUpgradesUI;            //Объект UI элементов улучшений

    [Header("Настройки")]
    [SerializeField] GameObject settingsUI;                 //Объект UI элементов настроек


    //Функция вызываемая при запуске приложения
    private void Awake()
    {
    }

    //Функция вызываемая каждый кадр программы
    private void Update()
    {
        //Если режим раннер обновляем расстояние
        if (StateMachine.currentState == StateMachine.GameState.Runner)
        {
            distanceCounter.text = "Distance: \n" + statisticsControl.CurrentDistance.ToString("0.00") + " m";
            if (statisticsControl.CurrentDistance > statisticsControl.MaxDistance)
            {
                hiscoreDistance.text = "Highscore \n" + statisticsControl.CurrentDistance.ToString("0.00") + " m";
            }
        }
    }


    //Раннер логика.............................................................................................................................................................
    //Обновить количесвто монет
    private void UpdateCoins(int n)
    {
        coinCounter.text = "Coins: \n" + n.ToString() + " c";
    }

    //Обновить количество здоровья
    private void UpdateHealth(int n)
    { 
        healthCounter.text = "Health: \n" + n.ToString();
    }

    //Таймер обатного отсчёта при отмене паузы
    private IEnumerator TimerRemainCounter(string regime)
    {
        textPopupControl.PopUpText("3", Color.white, new Vector2(0, 0));
        yield return new WaitForSeconds(1);
        textPopupControl.PopUpText("2", Color.white, new Vector2(0, 0));
        yield return new WaitForSeconds(1);
        textPopupControl.PopUpText("1", Color.white, new Vector2(0, 0));
        yield return new WaitForSeconds(1);
        if(regime == "resume")
            st_machine.ResumePlay();
        else if(regime == "revive")
            st_machine.RevivePlayer();
    }


    



    //События смены режимов.......................................................................................................................................................

    //Начать игру
    public void StartRunner()
    {
        pauseUI.SetActive(false);
        statisticsUI.SetActive(false);
        settingsUI.SetActive(false);
        st_machine.ChangeState(StateMachine.GameState.Runner);
    }
    //Вернуться в меню
    public void StartMenu()
    {
        pauseUI.SetActive(false);
        statisticsUI.SetActive(false);
        settingsUI.SetActive(false);
        st_machine.ChangeState(StateMachine.GameState.Menu);
    }
    //Открыть магазин шляп
    public void StartHats()
    {
        st_machine.ChangeState(StateMachine.GameState.StoreHats);
    }
    //Открыть магазин образов
    public void StartSkins()
    {
        st_machine.ChangeState(StateMachine.GameState.StoreSkins);
    }
    //Открыть магазин улучшений
    public void StartUpgrades()
    {
        st_machine.ChangeState(StateMachine.GameState.StoreUpgrades);
    }
    //Воскресить игрока
    public void RevivePlayer()
    {
        statisticsUI.SetActive(false);
        st_machine.DestroyCurrentChunk();
        StartCoroutine(TimerRemainCounter("revive"));
    }




    //Смена состояний UI...........................................................................................................................................................

    //Показать финальную статистику
    public void ShowStatistics()
    {
        statisticsUI.SetActive(true);

        //Установка текущих значений статистики
        statsStatCounter.text = "Distance: " + statisticsControl.CurrentDistance.ToString("0.00") + '\n';
        if (statisticsControl.CurrentDistance > statisticsControl.MaxDistance)
            statsStatCounter.text += "Highscore: " + statisticsControl.CurrentDistance.ToString("0.00") + '\n';
        else
            statsStatCounter.text += "Highscore: " + statisticsControl.MaxDistance.ToString("0.00") + '\n';
        statsStatCounter.text += "Coins: " + statisticsControl.CurrentCoins.ToString() + '\n';
    }

    //Скрыть финуальную статистику
    public void CloseStatistics()
    {
        statisticsUI.SetActive(false);
    }

    //Начать режим раннера
    public void StartRunnerUI()
    {
        runnerUI.SetActive(true);
        coinCounter.text = "Coins: \n" + 0 + " c";
        hiscoreDistance.text = "Highscore:\n" + statisticsControl.MaxDistance.ToString("0.00") + " m";

        statisticsControl.OnCoinUpdate.AddListener(UpdateCoins);
        PlayerMovement.OnElimination.AddListener(ShowStatistics);
        PlayerMovement.OnLifeChange.AddListener(UpdateHealth);
    }

    //Завершить режим раннера
    public void EndRunnerUI()
    {
        runnerUI.SetActive(false);
        statisticsControl.OnCoinUpdate.RemoveListener(UpdateCoins);
        PlayerMovement.OnElimination.RemoveListener(ShowStatistics);
        PlayerMovement.OnLifeChange.RemoveListener(UpdateHealth);
    }


    //Начать режим меню
    public void StartMenuUI()
    {
        menuUI.SetActive(true);
        ShowUpStat();
    }
    //Закончить режим меню
    public void EndMenuUI()
    {
        menuUI.SetActive(false);
        HideUpStat();
    }


    //Открыть меню шляп
    public void StartStoreHatUI()
    {
        storeHatUI.SetActive(true);
        ShowUpStat();
    }
    //Закрыть меню шляп
    public void EndStoreHatUI()
    {
        storeHatUI.SetActive(false);
        HideUpStat();
    }

    //Открыть меню улучшений
    public void StartStoreSkinUI()
    {
        storeSkinUI.SetActive(true);
        ShowUpStat();
    }
    //Закрыть меню скинов
    public void EndStoreSkinUI()
    {
        storeSkinUI.SetActive(false);
        HideUpStat();
    }


    //Открыть меню улучшений
    public void StartStoreUpgradeUI()
    {
        storeUpgradesUI.SetActive(true);
        ShowUpStat();
    }
    //Закрыть меню улучшений
    public void EndStoreUpgradeUI()
    {
        storeUpgradesUI.SetActive(false);
        HideUpStat();
    }

    //Открыть меню паузы и остановить игру
    public void StartPauseUI()
    {
        st_machine.StopPlay();

        //Установка текущих значений статистики
        pauseStatCounter.text = "Distance: " + statisticsControl.CurrentDistance.ToString("0.00") + '\n';
        if (statisticsControl.CurrentDistance > statisticsControl.MaxDistance)
            pauseStatCounter.text += "Highscore: " + statisticsControl.CurrentDistance.ToString("0.00") + '\n';
        else
            pauseStatCounter.text += "Highscore: " + statisticsControl.MaxDistance.ToString("0.00") + '\n';
        pauseStatCounter.text += "Coins: " + statisticsControl.CurrentCoins.ToString() + '\n';

        pauseUI.SetActive(true);
        ShowUpStat();
    }
    //Закрыть меню паузы
    public void EndPauseUI()
    {
        pauseUI.SetActive(false);
        HideUpStat();
        StartCoroutine(TimerRemainCounter("resume"));
    }

    //Показать верхнюю статистику
    public void ShowUpStat()
    {
        upCoinCounter.text = statisticsControl.Coins.ToString();
        upStatsUI.SetActive(true);
    }
    //Скрыть верхнюю статистику
    public void HideUpStat()
    {
        upStatsUI.SetActive(false);
    }

    //Начать режим меню
    public void StartSettingsUI()
    {
        settingsUI.SetActive(true);

        //Установка текущего значения аудио
    }
    //Закончить режим меню
    public void EndSettingsUI()
    {
        settingsUI.SetActive(false);
    }
}
