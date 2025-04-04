using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class UIControl : MonoBehaviour
{
    //������
    [Header("������")]
    [SerializeField] StatisticsControl statisticsControl;   //�������� ���������� ����
    [SerializeField] StateMachine st_machine;               //������ ���������
    [SerializeField] TextPopupControl textPopupControl;     //�������� �������������� ������

    [Header("������")]
    [SerializeField] TextMeshProUGUI coinCounter;           //����� ������� �����
    [SerializeField] TextMeshProUGUI distanceCounter;       //����� ������� ����������
    [SerializeField] TextMeshProUGUI healthCounter;         //����� ������� ��������
    [SerializeField] TextMeshProUGUI hiscoreDistance;       //����� ������� �������
    [SerializeField] GameObject runnerUI;                   //������ UI ��������� �������

    [Header("�����")]
    [SerializeField] GameObject pauseUI;                    //������ UI ��������� �����
    [SerializeField] TextMeshProUGUI pauseStatCounter;      //����� ������� ����������, �����, ������� �����

    [Header("����������")]
    [SerializeField] GameObject statisticsUI;               //������ UI ��������� ��������� ����������
    [SerializeField] TextMeshProUGUI statsStatCounter;  //����� ������� ����������, �����, ������� ����������

    [Header("����")]
    [SerializeField] GameObject menuUI;                     //������ UI ��������� ����

    [Header("������� ����������")]
    [SerializeField] GameObject upStatsUI;                  //������ UI ������� ����������
    [SerializeField] TextMeshProUGUI upCoinCounter;         //����� ������� ���� �����

    [Header("�������")]
    [SerializeField] GameObject storeHatUI;                 //������ UI ��������� �������� ����
    [SerializeField] GameObject storeSkinUI;                //������ UI ��������� �������� �������
    [SerializeField] GameObject storeUpgradesUI;            //������ UI ��������� ���������

    [Header("���������")]
    [SerializeField] GameObject settingsUI;                 //������ UI ��������� ��������


    //������� ���������� ��� ������� ����������
    private void Awake()
    {
    }

    //������� ���������� ������ ���� ���������
    private void Update()
    {
        //���� ����� ������ ��������� ����������
        if (StateMachine.currentState == StateMachine.GameState.Runner)
        {
            distanceCounter.text = "Distance: \n" + statisticsControl.CurrentDistance.ToString("0.00") + " m";
            if (statisticsControl.CurrentDistance > statisticsControl.MaxDistance)
            {
                hiscoreDistance.text = "Highscore \n" + statisticsControl.CurrentDistance.ToString("0.00") + " m";
            }
        }
    }


    //������ ������.............................................................................................................................................................
    //�������� ���������� �����
    private void UpdateCoins(int n)
    {
        coinCounter.text = "Coins: \n" + n.ToString() + " c";
    }

    //�������� ���������� ��������
    private void UpdateHealth(int n)
    { 
        healthCounter.text = "Health: \n" + n.ToString();
    }

    //������ �������� ������� ��� ������ �����
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


    



    //������� ����� �������.......................................................................................................................................................

    //������ ����
    public void StartRunner()
    {
        pauseUI.SetActive(false);
        statisticsUI.SetActive(false);
        settingsUI.SetActive(false);
        st_machine.ChangeState(StateMachine.GameState.Runner);
    }
    //��������� � ����
    public void StartMenu()
    {
        pauseUI.SetActive(false);
        statisticsUI.SetActive(false);
        settingsUI.SetActive(false);
        st_machine.ChangeState(StateMachine.GameState.Menu);
    }
    //������� ������� ����
    public void StartHats()
    {
        st_machine.ChangeState(StateMachine.GameState.StoreHats);
    }
    //������� ������� �������
    public void StartSkins()
    {
        st_machine.ChangeState(StateMachine.GameState.StoreSkins);
    }
    //������� ������� ���������
    public void StartUpgrades()
    {
        st_machine.ChangeState(StateMachine.GameState.StoreUpgrades);
    }
    //���������� ������
    public void RevivePlayer()
    {
        statisticsUI.SetActive(false);
        st_machine.DestroyCurrentChunk();
        StartCoroutine(TimerRemainCounter("revive"));
    }




    //����� ��������� UI...........................................................................................................................................................

    //�������� ��������� ����������
    public void ShowStatistics()
    {
        statisticsUI.SetActive(true);

        //��������� ������� �������� ����������
        statsStatCounter.text = "Distance: " + statisticsControl.CurrentDistance.ToString("0.00") + '\n';
        if (statisticsControl.CurrentDistance > statisticsControl.MaxDistance)
            statsStatCounter.text += "Highscore: " + statisticsControl.CurrentDistance.ToString("0.00") + '\n';
        else
            statsStatCounter.text += "Highscore: " + statisticsControl.MaxDistance.ToString("0.00") + '\n';
        statsStatCounter.text += "Coins: " + statisticsControl.CurrentCoins.ToString() + '\n';
    }

    //������ ���������� ����������
    public void CloseStatistics()
    {
        statisticsUI.SetActive(false);
    }

    //������ ����� �������
    public void StartRunnerUI()
    {
        runnerUI.SetActive(true);
        coinCounter.text = "Coins: \n" + 0 + " c";
        hiscoreDistance.text = "Highscore:\n" + statisticsControl.MaxDistance.ToString("0.00") + " m";

        statisticsControl.OnCoinUpdate.AddListener(UpdateCoins);
        PlayerMovement.OnElimination.AddListener(ShowStatistics);
        PlayerMovement.OnLifeChange.AddListener(UpdateHealth);
    }

    //��������� ����� �������
    public void EndRunnerUI()
    {
        runnerUI.SetActive(false);
        statisticsControl.OnCoinUpdate.RemoveListener(UpdateCoins);
        PlayerMovement.OnElimination.RemoveListener(ShowStatistics);
        PlayerMovement.OnLifeChange.RemoveListener(UpdateHealth);
    }


    //������ ����� ����
    public void StartMenuUI()
    {
        menuUI.SetActive(true);
        ShowUpStat();
    }
    //��������� ����� ����
    public void EndMenuUI()
    {
        menuUI.SetActive(false);
        HideUpStat();
    }


    //������� ���� ����
    public void StartStoreHatUI()
    {
        storeHatUI.SetActive(true);
        ShowUpStat();
    }
    //������� ���� ����
    public void EndStoreHatUI()
    {
        storeHatUI.SetActive(false);
        HideUpStat();
    }

    //������� ���� ���������
    public void StartStoreSkinUI()
    {
        storeSkinUI.SetActive(true);
        ShowUpStat();
    }
    //������� ���� ������
    public void EndStoreSkinUI()
    {
        storeSkinUI.SetActive(false);
        HideUpStat();
    }


    //������� ���� ���������
    public void StartStoreUpgradeUI()
    {
        storeUpgradesUI.SetActive(true);
        ShowUpStat();
    }
    //������� ���� ���������
    public void EndStoreUpgradeUI()
    {
        storeUpgradesUI.SetActive(false);
        HideUpStat();
    }

    //������� ���� ����� � ���������� ����
    public void StartPauseUI()
    {
        st_machine.StopPlay();

        //��������� ������� �������� ����������
        pauseStatCounter.text = "Distance: " + statisticsControl.CurrentDistance.ToString("0.00") + '\n';
        if (statisticsControl.CurrentDistance > statisticsControl.MaxDistance)
            pauseStatCounter.text += "Highscore: " + statisticsControl.CurrentDistance.ToString("0.00") + '\n';
        else
            pauseStatCounter.text += "Highscore: " + statisticsControl.MaxDistance.ToString("0.00") + '\n';
        pauseStatCounter.text += "Coins: " + statisticsControl.CurrentCoins.ToString() + '\n';

        pauseUI.SetActive(true);
        ShowUpStat();
    }
    //������� ���� �����
    public void EndPauseUI()
    {
        pauseUI.SetActive(false);
        HideUpStat();
        StartCoroutine(TimerRemainCounter("resume"));
    }

    //�������� ������� ����������
    public void ShowUpStat()
    {
        upCoinCounter.text = statisticsControl.Coins.ToString();
        upStatsUI.SetActive(true);
    }
    //������ ������� ����������
    public void HideUpStat()
    {
        upStatsUI.SetActive(false);
    }

    //������ ����� ����
    public void StartSettingsUI()
    {
        settingsUI.SetActive(true);

        //��������� �������� �������� �����
    }
    //��������� ����� ����
    public void EndSettingsUI()
    {
        settingsUI.SetActive(false);
    }
}
