using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditorInternal.VersionControl.ListControl;

public class StateMachine : MonoBehaviour
{

    //Ссылки на скрипты
    [SerializeField] PlayerMovement plrMovement;
    [SerializeField] ChunkManager chunkManager;
    [SerializeField] UIControl uIControl;
    [SerializeField] StatisticsControl statisticsControl;

    //Текущие режимы
    public enum GameState { Runner, Menu, StoreHats, StoreUpgrades, StoreSkins }
    public static GameState currentState;
    static GameState lastState;

    private void Start()
    {
        currentState = GameState.Menu;
        ChangeState(GameState.Menu);
    }


    //Сменить режим игры
    public void ChangeState(GameState newState)
    {
        DeactivateState();
        currentState = newState;
        ActivateState();
    }

    //Активировать новое состояние
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

    //Деактивировать старое состояние
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

    //Сохранить прогресс пользователя
    public void SaveProgress()
    {
        statisticsControl.SetResult();
    }

    //Воссатновить игрока
    public void RevivePlayer()
    {
        statisticsControl.IsRunning = true;
        chunkManager.RestoreSpeed();
        plrMovement.ActivateMovement();
    }

    //Уничтожить текущие объекты на чанке
    public void DestroyCurrentChunk()
    {
        chunkManager.DestroyCurrentChunk();
        plrMovement.SetPlayerHealth(1);
        plrMovement.DeactivateRagdoll();
    }

    //Снять игру с паузы 
    public void ResumePlay()
    {
        statisticsControl.IsRunning = true;
        chunkManager.RestoreSpeed();
        plrMovement.ActivateMovement();
    }

    //Остановить игру
    public void StopPlay()
    {
        statisticsControl.IsRunning = false;
        chunkManager.StopPlay();
        plrMovement.DeactivateMovement();
    }
}
