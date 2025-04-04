using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static Collectable;

public class Collectable : MonoBehaviour, IPoolObject
{
    //Логика пула......................................
    public int poolNum { get; set; }

    //Инициализация коллектабла
    public void Initialize()
    {
        speed = gameSettings.runnerSpeed;
        //Debug.Log("Chunk Initialized");
    }

    //Очистка данных при освобождении коллектабла
    public void Release()
    {
        //Debug.Log("Chunk Released");
    }
    //Общая логика....................................

    [Header("Ссылки")]
    [SerializeField]GameSettings gameSettings;

    //Список состояний коллектаблса
    public enum CollectableState { None, Jumping, Rotating }
    //текущее состояние коллектаблса
    [Header("Состояние")]
    [SerializeField] CollectableState currentState;
    //Тип собирраемого предмета
    [SerializeField] string type;
    [HideInInspector] public string Type { get { return type; } }
    //Текущая скорость игрового поля и в частности монет
    private float speed = 1;
    [HideInInspector] public float Speed => speed;

    [Header("Прыжок")]
    [SerializeField] private float jumpForce;

    [Header("Поворот")]
    [SerializeField] private float rotationSpeed;
    [SerializeField] private Vector3 rotationAxis;


    //Выполнять некоторое поведение персонажа
    private void FixedUpdate()
    {
        switch (currentState)
        { 
            case CollectableState.None:
                break;
            case CollectableState.Jumping:

                break;
            case CollectableState.Rotating:
                transform.Rotate(rotationAxis, rotationSpeed);
                break;
            default:
                break;
        }

        //Постоянное перемещение в стоорону игрока
        transform.position += new Vector3(0, 0, -speed * Time.deltaTime);
    }

    //Сменить состояние объекта
    public void ChangeState(CollectableState state)
    {
        currentState = state;
    }


    //Изменение скорости объекта
    public void ChangeSpeed(float speed)
    {
        this.speed = speed;
    }


}





//Скрипт динамического изменения атрибутов в редакторе
[CustomEditor(typeof(Collectable))]
public class CollectableEditor : Editor
{
    //Контроллируемые атрибуты
    SerializedProperty currentState;
    SerializedProperty jumpForce;
    SerializedProperty rotationSpeed;
    SerializedProperty rotationAxis;
    SerializedProperty type;
    SerializedProperty gameSettings;

    //При активации ищем данные атрибуты
    private void OnEnable()
    {
        currentState = serializedObject.FindProperty("currentState");
        jumpForce = serializedObject.FindProperty("jumpForce");
        rotationSpeed = serializedObject.FindProperty("rotationSpeed");
        rotationAxis = serializedObject.FindProperty("rotationAxis");
        type = serializedObject.FindProperty("type");
        gameSettings = serializedObject.FindProperty("gameSettings");
    }

    //При отрисовке инспектора в соответствии с состояние currentState отрисовываем необходимые атрибуты
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(gameSettings);

        EditorGUILayout.PropertyField(currentState);
        EditorGUILayout.PropertyField(type);

        switch ((CollectableState)currentState.enumValueIndex)
        {
            case CollectableState.None:
                //Нет дополнительных атрибутов
                break;

            case CollectableState.Jumping:
                EditorGUILayout.PropertyField(jumpForce);
                break;

            case CollectableState.Rotating:
                EditorGUILayout.PropertyField(rotationSpeed);
                EditorGUILayout.PropertyField(rotationAxis);
                break;
        }

        //Применить изменённые атрибуты
        serializedObject.ApplyModifiedProperties();
    }
}