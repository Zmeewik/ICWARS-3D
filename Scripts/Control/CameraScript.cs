using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{

    [SerializeField] Vector3 standartOffset;
    [SerializeField] Quaternion standartRotation;

    [SerializeField] Transform player;
    [SerializeField] float transferTime;
    [SerializeField] float connectZone;
    bool xShifting = false, yShifting = false;

    float timeNoConnect = 0.4f;
    float xTimer, yTimer;

    bool isEliminated;

    //����������� ������� ��������� ����������� ������
    private void Start()
    {
        PlayerMovement.OnRevive.AddListener(RevivePlayer);
        PlayerMovement.OnElimination.AddListener(EliminatePlayer);
        PlayerMovement.OnShift.AddListener(StartXShift);
        PlayerCollisionControl.OnGround.AddListener(StartYShift);
    }

    //���������� ������, ���� ����� �������
    private void EliminatePlayer()
    {
        isEliminated = true;
    }

    //������������ �������� �������, ���� ����� ������������
    private void RevivePlayer()
    {
        isEliminated = false;
    }

    //������ ����� �� X
    private void StartXShift()
    { 
        xShifting = true;
        xTimer = 0;
    }

    //������ ����� �� Y
    private void StartYShift()
    {
        yShifting = true;
        yTimer = timeNoConnect;
    }

    private void StartYShiftWithDeadZone()
    {
        yShifting = true;
        yTimer = 0;
    }

    public void SetCameraToSrtartPosition()
    {
        transform.position = player.position + standartOffset;
    }

    //������� ���������� ������ ���������� ������������ ��� ��������� ������
    private void FixedUpdate()
    {
        if (isEliminated)
            return;
        

        if (xShifting)
        {
            //��� �������� �� X ������� ������
            var xShift = Mathf.Lerp(transform.position.x, (player.position + standartOffset).x, transferTime);
            transform.position = new Vector3(xShift, transform.position.y, transform.position.z);
            var xDistance = Mathf.Abs(transform.position.x - (player.position + standartOffset).x);

            xTimer += Time.deltaTime;

            if (xDistance < connectZone && xTimer >= timeNoConnect)
            {
                transform.position = new Vector3((player.position + standartOffset).x, transform.position.y, transform.position.z);
                xShifting = false;
            }
        }

        if (yShifting)
        {
            //��� �������� �� Y ������� ������
            var yShift = Mathf.Lerp(transform.position.y, (player.position + standartOffset).y, transferTime);
            transform.position = new Vector3(transform.position.x, yShift, transform.position.z);
            var yDistance = Mathf.Abs(transform.position.y - (player.position + standartOffset).y);

            yTimer += Time.deltaTime;

            if (yDistance < connectZone && yTimer >= timeNoConnect)
            {
                transform.position = new Vector3(transform.position.x, (player.position + standartOffset).y, transform.position.z);
                yShifting = false;
            }
        }
    }

}
