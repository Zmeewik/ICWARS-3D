using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static Collectable;

public class Collectable : MonoBehaviour, IPoolObject
{
    //������ ����......................................
    public int poolNum { get; set; }

    //������������� �����������
    public void Initialize()
    {
        speed = gameSettings.runnerSpeed;
        //Debug.Log("Chunk Initialized");
    }

    //������� ������ ��� ������������ �����������
    public void Release()
    {
        //Debug.Log("Chunk Released");
    }
    //����� ������....................................

    [Header("������")]
    [SerializeField]GameSettings gameSettings;

    //������ ��������� ������������
    public enum CollectableState { None, Jumping, Rotating }
    //������� ��������� ������������
    [Header("���������")]
    [SerializeField] CollectableState currentState;
    //��� ������������ ��������
    [SerializeField] string type;
    [HideInInspector] public string Type { get { return type; } }
    //������� �������� �������� ���� � � ��������� �����
    private float speed = 1;
    [HideInInspector] public float Speed => speed;

    [Header("������")]
    [SerializeField] private float jumpForce;

    [Header("�������")]
    [SerializeField] private float rotationSpeed;
    [SerializeField] private Vector3 rotationAxis;


    //��������� ��������� ��������� ���������
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

        //���������� ����������� � �������� ������
        transform.position += new Vector3(0, 0, -speed * Time.deltaTime);
    }

    //������� ��������� �������
    public void ChangeState(CollectableState state)
    {
        currentState = state;
    }


    //��������� �������� �������
    public void ChangeSpeed(float speed)
    {
        this.speed = speed;
    }


}





//������ ������������� ��������� ��������� � ���������
[CustomEditor(typeof(Collectable))]
public class CollectableEditor : Editor
{
    //��������������� ��������
    SerializedProperty currentState;
    SerializedProperty jumpForce;
    SerializedProperty rotationSpeed;
    SerializedProperty rotationAxis;
    SerializedProperty type;
    SerializedProperty gameSettings;

    //��� ��������� ���� ������ ��������
    private void OnEnable()
    {
        currentState = serializedObject.FindProperty("currentState");
        jumpForce = serializedObject.FindProperty("jumpForce");
        rotationSpeed = serializedObject.FindProperty("rotationSpeed");
        rotationAxis = serializedObject.FindProperty("rotationAxis");
        type = serializedObject.FindProperty("type");
        gameSettings = serializedObject.FindProperty("gameSettings");
    }

    //��� ��������� ���������� � ������������ � ��������� currentState ������������ ����������� ��������
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(gameSettings);

        EditorGUILayout.PropertyField(currentState);
        EditorGUILayout.PropertyField(type);

        switch ((CollectableState)currentState.enumValueIndex)
        {
            case CollectableState.None:
                //��� �������������� ���������
                break;

            case CollectableState.Jumping:
                EditorGUILayout.PropertyField(jumpForce);
                break;

            case CollectableState.Rotating:
                EditorGUILayout.PropertyField(rotationSpeed);
                EditorGUILayout.PropertyField(rotationAxis);
                break;
        }

        //��������� ��������� ��������
        serializedObject.ApplyModifiedProperties();
    }
}