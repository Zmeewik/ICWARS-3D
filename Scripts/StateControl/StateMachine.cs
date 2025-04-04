using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditorInternal.VersionControl.ListControl;

public class StateMachine : MonoBehaviour
{

    //������ �� �������
    [SerializeField] PlayerMovement plrMovement;
    [SerializeField] ChunkManager chunkManager;
    [SerializeField] UIControl uIControl;
    [SerializeField] StatisticsControl statisticsControl;

    //������� ������
    public enum GameState { Runner, Menu, StoreHats, StoreUpgrades, StoreSkins }
    public static GameState currentState;
    static GameState lastState;

    private void Start()
    {
        currentState = GameState.Menu;
        ChangeState(GameState.Menu);
    }


    //������� ����� ����
    public void ChangeState(GameState newState)
    {
        DeactivateState();
        currentState = newState;
        ActivateState();
    }

    //������������ ����� ���������
    public void ActivateState()
    {
        switch (currentState)
        { 
            case GameState.Runner:
                chunkManager.StartRunner();
                uIControl.StartRunnerUI();
                plrMovement.StartRunner();
                plrMovement.ActivateMovement();
                statisticsControl.StartRunning();
                break;
            case GameState.Menu:
                uIControl.StartMenuUI();
                break;
            case GameState.StoreHats:
                uIControl.StartStoreHatUI();
                break;
            case GameState.StoreSkins:
                uIControl.StartStoreSkinUI();
                break;
            case GameState.StoreUpgrades:
                uIControl.StartStoreUpgradeUI();
                break;
        }
    }

    //�������������� ������ ���������
    public void DeactivateState()
    {
        switch (currentState)
        {
            case GameState.Runner:
                uIControl.EndRunnerUI();
                chunkManager.EndRunner();
                plrMovement.EndRunner();
                plrMovement.DeactivateMovement();
                SaveProgress();
                statisticsControl.StopRunning();
                break;
            case GameState.Menu:
                uIControl.EndMenuUI();
                break;
            case GameState.StoreHats:
                uIControl.EndStoreHatUI();
                break;
            case GameState.StoreSkins:
                uIControl.EndStoreSkinUI();
                break;
            case GameState.StoreUpgrades:
                uIControl.EndStoreUpgradeUI();
                break;
            default:
                break;
        }
    }

    //��������� �������� ������������
    public void SaveProgress()
    {
        statisticsControl.SetResult();
    }

    //������������ ������
    public void RevivePlayer()
    {
        statisticsControl.IsRunning = true;
        chunkManager.RestoreSpeed();
        plrMovement.ActivateMovement();
    }

    //���������� ������� ������� �� �����
    public void DestroyCurrentChunk()
    {
        chunkManager.DestroyCurrentChunk();
        plrMovement.SetPlayerHealth(1);
        plrMovement.DeactivateRagdoll();
    }

    //����� ���� � ����� 
    public void ResumePlay()
    {
        statisticsControl.IsRunning = true;
        chunkManager.RestoreSpeed();
        plrMovement.ActivateMovement();
    }

    //���������� ����
    public void StopPlay()
    {
        statisticsControl.IsRunning = false;
        chunkManager.StopPlay();
        plrMovement.DeactivateMovement();
    }
}
