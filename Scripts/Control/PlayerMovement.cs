using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class LifeChangeEvent : UnityEvent<int> { }

public class PlayerMovement : MonoBehaviour
{
    [Header("������")]
    //������
    //���������� �����������
    [SerializeField] PlayerCollisionControl plrColControl;
    //���������� ��������� ����
    [SerializeField] GameSettings gameSettings;
    //Ƹ���� ���� ������
    Rigidbody rb;
    [SerializeField] CameraScript cam_scr;
    //������� ���������
    [SerializeField] ParticleSystem dieParticles;
    [SerializeField] ParticleSystem collectParticles;
    [SerializeField] ParticleSystem currentDieParticle;

    //������ �� ������������ ������
    [SerializeField] AnimationControl animControl;

    //������ ������ � �������
    [SerializeField] GameObject playerStatic;
    [SerializeField] GameObject cartStatic;
    [SerializeField] GameObject playerRagdoll;
    [SerializeField] GameObject cartRagdoll;
    //������� ��������� ������
    [SerializeField] BoxCollider plrCollider;
    GameObject currentRagdoll;
    GameObject currentCartRagdoll;


    [Header("��������")]
    //������� �������� ������
    [SerializeField] int currentHealth = 0;
    //������������ �������� ������
    [SerializeField] int maxHealth;
    //������� ��� ���������� ��������
    public static LifeChangeEvent OnLifeChange = new LifeChangeEvent();
    //������� ������������ ��� ������
    public static UnityEvent OnElimination = new UnityEvent();
    //������� ������������ ��� �����������
    public static UnityEvent OnRevive = new UnityEvent();
    //������� �� �����
    bool isEliminated = false;



    //�������� ������ �� X
    //������� ����� ��������
    private int laneIndex = 1;
    private int lastLaneIndex = 1;
    //���������� ����� �������
    private float laneDistance;
    //������� �������� ����������� ������ ����� �������
    private float moveSpeed;
    [Header("�����")]
    //������� ���������� ��� ������ ������ ������
    static public UnityEvent OnShift = new UnityEvent();
    //�������� ��������� ��������
    [SerializeField] private float moveSpeedChangeSpeed;
    //�������� ����������� �� ���������� � ���������
    [SerializeField] private float minSpeed, maxSpeed;
    //���������� ����������� �������� ������
    [SerializeField] private float stopMoveDistance;
    //��������� ������� ���� �����
    private Vector3 targetPosition;
    private Vector3 lastPosition;
    //������������ �� ����� �� X ����������
    private bool isPlayerMoving;


    [Header("������")]
    //������
    //���� ������ ������
    [SerializeField] private float jumpForce;
    [SerializeField] private float gravityScale; // ������� ����������
    //� ������� �� ������ �����
    private bool isPlayerInAir = false;


    [Header("������")]
    //������
    //���������� ����������
    [SerializeField] float crouchScale;
    //�������������� ���� ����
    [SerializeField] float crouchDownForce;
    //����� �������
    [SerializeField] float crouchTime;
    bool isCrouched = false;


    //������� ���������� ��� ������ ����
    private void Start()
    {
        //��������� ������ �� ���������� ������ ���� ������
        rb = GetComponent<Rigidbody>();
        //��������� ���������� �� �����
        laneDistance = gameSettings.lineRange;
        collectParticles.Stop();
        dieParticles.Stop();
    }

    public void StartRunner()
    {
        laneIndex = 1;
        //���������� ��������� ��������� ������ � ������������
        MoveToLine();
        //��������� �������� �� ��������
        currentHealth = maxHealth;
        OnLifeChange.Invoke(currentHealth);
        dieParticles.gameObject.transform.position = transform.position + Vector3.up * 3;

        //��������� ����������� ��������
        animControl.ChangeAnimation("Running");

        //�������� ������� �� �������
        //�������� �� �����
        InputHandle.OnSwipe.AddListener(OnMoveAction);
        //�������� �� ������������ � �����
        PlayerCollisionControl.OnGround.AddListener(SetPlayerToTheGround);
        PlayerCollisionControl.OnCoinTouch.AddListener(ActivateCollectEffect);
        //�������� �� ������������ � ���������
        plrColControl.OnBarrierTouch.AddListener(HitSequence);
        plrColControl.OnBarrierHit.AddListener(EleminationSequence);
    }

    public void EndRunner()
    {
        laneIndex = 1;
        transform.rotation = Quaternion.Euler(0, 0, 0);
        transform.position = new Vector3(0,0,-1.5f);
        MoveToLine();
        cam_scr.SetCameraToSrtartPosition();

        if (currentDieParticle != null)
            Destroy(currentDieParticle.gameObject);
        OnRevive.Invoke();

        //��������� ����������� ��������
        animControl.ChangeAnimation("Walk");

        //������� ������ � ������ ��������
        cartStatic.SetActive(true);
        playerStatic.SetActive(true);
        Destroy(currentRagdoll);
        Destroy(currentCartRagdoll);
        plrCollider.enabled = true;
        plrCollider.transform.GetComponent<Rigidbody>().useGravity = true;
        currentCartRagdoll = null;
        currentRagdoll = null;

        //������� ������� �� �������
        //�������� �� �����
        InputHandle.OnSwipe.RemoveListener(OnMoveAction);
        //�������� �� ������������ � �����
        PlayerCollisionControl.OnGround.RemoveListener(SetPlayerToTheGround);
        //�������� �� ������������ � ���������
        plrColControl.OnBarrierTouch.RemoveListener(HitSequence);
        plrColControl.OnBarrierHit.RemoveListener(EleminationSequence);
    }

        //�������� ���������� ����� ����������� ���������� �������
        private void FixedUpdate()
    {
        rb.AddForce(Physics.gravity * (gravityScale - 1) * rb.mass);


        if (isPlayerMoving)
        {

            //��������� �������� ������ ������
            moveSpeed = Mathf.MoveTowards(moveSpeed, minSpeed, moveSpeedChangeSpeed);

            //������ ����������� ������ � ������ ����� �� X ����������
            Vector3 newPosition = new Vector3(targetPosition.x, rb.position.y, rb.position.z);
            newPosition = Vector3.MoveTowards(rb.position, newPosition, moveSpeed * Time.fixedDeltaTime);
            rb.MovePosition(newPosition);

            //���� ���������� ������ ���������, �� ����� ���������������
            if (Mathf.Abs(rb.position.x - targetPosition.x) < stopMoveDistance)
            { 
                isPlayerMoving = false;
                lastPosition = targetPosition;
            }
        }
    }

    //��������� ������ � ������ �����������
    private void OnMoveAction(Vector2 direction)
    {
        //���� ������ ������� �� �� ����� ���������
        if (isEliminated)
            return;

        if (direction == Vector2.up)
            Jump();
        else if (direction == Vector2.down)
            Crouch();
        else if (direction == Vector2.left)
            Move(Vector3.left);
        else if (direction == Vector2.right)
            Move(Vector3.right);
    }

    //�������� ������ ����� �������
    public void Move(Vector3 direction)
    {
        isPlayerMoving = true;
        OnShift.Invoke();

        if (direction == Vector3.left)
        {
            if (laneIndex > 0)
            {
                lastLaneIndex = laneIndex;
                laneIndex--;
                MoveToLine();
            }
        }
        else if (direction == Vector3.right)
        {
            if (laneIndex < 2)
            {
                lastLaneIndex = laneIndex;
                laneIndex++;
                MoveToLine();
            }
        }
        
    }

    //������ ����� ���� ��� �������� ������, ������������ �����
    public void MoveToLine()
    {
        moveSpeed = maxSpeed;
        targetPosition = new Vector3((laneIndex - 1) * laneDistance, transform.position.y, transform.position.z);
    }

    //�������� ����� (������)
    public void Jump()
    {
        if (!isPlayerInAir)
        {
            plrColControl.isFalling = false;
            Invoke("SetFalling", 0.1f);
            Debug.Log("Jumped!");
            rb.AddRelativeForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isPlayerInAir = true;
        }
    }

    private void SetFalling()
    {
        plrColControl.isFalling = true;
    }

    //������������ ������ ��������� ������
    private void ActivateCollectEffect(Collectable coin)
    {
        collectParticles.Play();
    }

    //��������� ������ �� �����
    private void SetPlayerToTheGround()
    {
        isPlayerInAir = false;
    }

    //�������� ���� (������)
    public void Crouch()
    {
        if (isCrouched)
            return;

        isCrouched = true;

        //��������� ��������� ������
        plrColControl.ChangeColliderHeight(crouchScale);
        Invoke("RaisePlayer", crouchTime);

        //������� � �������
        animControl.ChangeAnimation("GoIn");
        Invoke("StartStaying", 1f * 24/60);

        //�������� �������� ����������� � �����, ���� ����� � �������
        if (isPlayerInAir)
            rb.AddRelativeForce(Vector3.down * crouchDownForce, ForceMode.Impulse);
    }

    //������� ������ � ����������� �����
    public void RaisePlayer()
    {
        plrColControl.ChangeColliderHeight(1.35846f);
        //������� �� �������
        animControl.ChangeAnimation("GoOut");
        Invoke("StartRunning", 1f * 24 / 60);
        isCrouched = false;
    }

    //������ ���������� ������ �������
    public void StartStaying()
    {
        animControl.ChangeAnimation("StayIn");
    }

    //������ ����� ������
    public void StartRunning()
    {
        animControl.ChangeAnimation("Running");
    }


    //������������������ �������� ��� ����� � �����������
    private void HitSequence()
    {
        Debug.Log("Hit");
        currentHealth -= 1;
        OnLifeChange.Invoke(currentHealth);

        if (isPlayerMoving)
        {
            laneIndex = lastLaneIndex;
            targetPosition = lastPosition;
        }

        if (currentHealth <= 0)
        {
            EleminationSequence();
        }
    }


    //������������������ ����������� ��� ���������� ������
    private void EleminationSequence()
    {
        playerStatic.SetActive(false);
        cartStatic.SetActive(false);

        plrCollider.enabled = false;
        plrCollider.transform.GetComponent<Rigidbody>().useGravity = false;
        //cartCollider.enabled = true;
        currentRagdoll = Instantiate(playerRagdoll, playerStatic.transform.position, Quaternion.identity);
        currentRagdoll.transform.rotation = Quaternion.Euler(0,90,0);
        currentRagdoll.transform.GetChild(0).GetChild(0).GetComponent<Rigidbody>().AddForce(Vector3.forward * 3, ForceMode.Impulse);

        currentCartRagdoll = Instantiate(cartRagdoll, cartStatic.transform.position, Quaternion.identity);
        currentCartRagdoll.GetComponent<Rigidbody>().AddForce(Vector3.forward * 3, ForceMode.Impulse);


        currentDieParticle = Instantiate(dieParticles.gameObject, null).GetComponent<ParticleSystem>();
        currentDieParticle.transform.position = currentRagdoll.transform.position + Vector3.up * 2;
        currentDieParticle.Play();

        animControl.ChangeAnimation("Walk");
        Debug.Log("Eliminated");
        isEliminated = true;
        OnElimination.Invoke();
    }

    //������� ���������� � ������
    public void DeactivateMovement()
    {
        isEliminated = true;
    }

    //������� ���������� ������
    public void ActivateMovement()
    {
        isEliminated = false;
        animControl.ChangeAnimation("Running");
    }

    //��������� �������� � ������� � ������ ��������� ������ � ������
    public void DeactivateRagdoll()
    {
        //������� ������ � ������ � ������� �������
        transform.rotation = Quaternion.Euler(0,0,0);
        transform.position = new Vector3(0, 0, -1.5f);
        MoveToLine();
        cam_scr.SetCameraToSrtartPosition();

        animControl.ChangeAnimation("Walk");

        if(currentDieParticle != null)
            Destroy(currentDieParticle.gameObject);
        OnRevive.Invoke();

        //������� ������ � ������ ��������
        cartStatic.SetActive(true);
        playerStatic.SetActive(true);
        Destroy(currentRagdoll);
        Destroy(currentCartRagdoll);
        plrCollider.enabled = true;
        plrCollider.transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
        plrCollider.transform.GetComponent<Rigidbody>().useGravity = true;
        currentCartRagdoll = null;
        currentRagdoll = null;
    }

    //������������ �������� ������(
    public void SetPlayerHealth(int val)
    {
        //��������� �������� �� ��������
        currentHealth = val;
        OnLifeChange.Invoke(currentHealth);
    }

    public void RestorePlayerHealth()
    {
        //��������� �������� �� ��������
        currentHealth = maxHealth;
        OnLifeChange.Invoke(currentHealth);
    }
}
