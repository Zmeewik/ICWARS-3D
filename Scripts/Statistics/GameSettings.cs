using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Settings/GameSettings")]
public class GameSettings : ScriptableObject
{
    //Текущие настройки игры
    [Header("Runner")]
    //Скорости
    public float minSpeed;    //Минимальная скорость перемещения мира в раннере
    public float maxSpeed;    //Максимальная скорость перемещения мира в раннере
    public float changeSpeed; //Скорость изменения скорости в раннере
    public float runnerSpeed; //Скорость перемещения мира в раннере
    //Линии
    public float lineRange;   //Расстояние до линий
}
