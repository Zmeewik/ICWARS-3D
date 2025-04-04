using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class LifeChangeEvent : UnityEvent<int> { }

public class PlayerMovement : MonoBehaviour
{
    [Header("Сслыки")]
    //Ссылки
    //Управление коллайдером
    [SerializeField] PlayerCollisionControl plrColControl;
    //Глобальные настройки игры
    [SerializeField] GameSettings gameSettings;
    //Жёское тело игрока
    Rigidbody rb;
    [SerializeField] CameraScript cam_scr;
    //Эффекты персонажа
    [SerializeField] ParticleSystem dieParticles;
    [SerializeField] ParticleSystem collectParticles;
    [SerializeField] ParticleSystem currentDieParticle;

    //Ссылка на анимационный скрипт
    [SerializeField] AnimationControl animControl;

    //Объект игрока и рэгдолл
    [SerializeField] GameObject playerStatic;
    [SerializeField] GameObject cartStatic;
    [SerializeField] GameObject playerRagdoll;
    [SerializeField] GameObject cartRagdoll;
    //Текущий коллайдер игрока
    [SerializeField] BoxCollider plrCollider;
    GameObject currentRagdoll;
    GameObject currentCartRagdoll;


    [Header("Здоровье")]
    //Текущее здоровье игрока
    [SerializeField] int currentHealth = 0;
    //Максимальное здоровье игрока
    [SerializeField] int maxHealth;
    //Событие при иизменении здоровья
    public static LifeChangeEvent OnLifeChange = new LifeChangeEvent();
    //Событие происходящее при смерти
    public static UnityEvent OnElimination = new UnityEvent();
    //Событие происходящее при возрождении
    public static UnityEvent OnRevive = new UnityEvent();
    //Устранён ли игрок
    bool isEliminated = false;



    //Смещение игрока по X
    //Текущая линия движения
    private int laneIndex = 1;
    private int lastLaneIndex = 1;
    //Расстояние между линиями
    private float laneDistance;
    //Текущая скорость перемещения игрока между линиями
    private float moveSpeed;
    [Header("Сдвиг")]
    //Событие вызываемое при начале сдвига игрока
    static public UnityEvent OnShift = new UnityEvent();
    //Скорость изменения скорости
    [SerializeField] private float moveSpeedChangeSpeed;
    //Скорость перемещения от наибольшей к наименьше
    [SerializeField] private float minSpeed, maxSpeed;
    //Расстояние прекращения движения игрока
    [SerializeField] private float stopMoveDistance;
    //Положение текущей цели линии
    private Vector3 targetPosition;
    private Vector3 lastPosition;
    //Перемещается ли игрок по X координате
    private bool isPlayerMoving;


    [Header("Прыжок")]
    //Прыжок
    //Сила прыжка игрока
    [SerializeField] private float jumpForce;
    [SerializeField] private float gravityScale; // Масштаб гравитации
    //В воздухе ли сейчас игрок
    private bool isPlayerInAir = false;


    [Header("Присед")]
    //Присед
    //Уменьшение коллайдера
    [SerializeField] float crouchScale;
    //Дополнительная сила вниз
    [SerializeField] float crouchDownForce;
    //Время приседа
    [SerializeField] float crouchTime;
    bool isCrouched = false;


    //Функция вызываемая при старте игры
    private void Start()
    {
        //Получение ссылки на кокмпонент жёского тела игрока
        rb = GetComponent<Rigidbody>();
        //Получение расстояния до линий
        laneDistance = gameSettings.lineRange;
        collectParticles.Stop();
        dieParticles.Stop();
    }

    public void StartRunner()
    {
        laneIndex = 1;
        //Обновление конечного положения игрока в пространстве
        MoveToLine();
        //Установка здоровья на максимум
        currentHealth = maxHealth;
        OnLifeChange.Invoke(currentHealth);
        dieParticles.gameObject.transform.position = transform.position + Vector3.up * 3;

        //ПОставить стандартную анимацию
        animControl.ChangeAnimation("Running");

        //Подписка методов на события
        //Подписка на свайп
        InputHandle.OnSwipe.AddListener(OnMoveAction);
        //Подписка на столкновение с землёй
        PlayerCollisionControl.OnGround.AddListener(SetPlayerToTheGround);
        PlayerCollisionControl.OnCoinTouch.AddListener(ActivateCollectEffect);
        //Подписка на столкновение с бальерами
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

        //ПОставить стандартную анимацию
        animControl.ChangeAnimation("Walk");

        //Вернуть физику и убрать рэгдоллы
        cartStatic.SetActive(true);
        playerStatic.SetActive(true);
        Destroy(currentRagdoll);
        Destroy(currentCartRagdoll);
        plrCollider.enabled = true;
        plrCollider.transform.GetComponent<Rigidbody>().useGravity = true;
        currentCartRagdoll = null;
        currentRagdoll = null;

        //Отписка методов на события
        //Подписка на свайп
        InputHandle.OnSwipe.RemoveListener(OnMoveAction);
        //Подписка на столкновение с землёй
        PlayerCollisionControl.OnGround.RemoveListener(SetPlayerToTheGround);
        //Подписка на столкновение с бальерами
        plrColControl.OnBarrierTouch.RemoveListener(HitSequence);
        plrColControl.OnBarrierHit.RemoveListener(EleminationSequence);
    }

        //Функкция вызываемая через определённые промежутки времени
        private void FixedUpdate()
    {
        rb.AddForce(Physics.gravity * (gravityScale - 1) * rb.mass);


        if (isPlayerMoving)
        {

            //Изменение скорости сдвига игрока
            moveSpeed = Mathf.MoveTowards(moveSpeed, minSpeed, moveSpeedChangeSpeed);

            //Мягкое перемещение игрока к нужной линии по X координате
            Vector3 newPosition = new Vector3(targetPosition.x, rb.position.y, rb.position.z);
            newPosition = Vector3.MoveTowards(rb.position, newPosition, moveSpeed * Time.fixedDeltaTime);
            rb.MovePosition(newPosition);

            //Если расстояние меньше заданного, то игрок останавливается
            if (Mathf.Abs(rb.position.x - targetPosition.x) < stopMoveDistance)
            { 
                isPlayerMoving = false;
                lastPosition = targetPosition;
            }
        }
    }

    //Обработка свайпа в разные направления
    private void OnMoveAction(Vector2 direction)
    {
        //если игрокк устранён он не может двигаться
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

    //Движение игрока между линиями
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

    //Задать новую цель для движения игрока, опрежделённую линию
    public void MoveToLine()
    {
        moveSpeed = maxSpeed;
        targetPosition = new Vector3((laneIndex - 1) * laneDistance, transform.position.y, transform.position.z);
    }

    //Движение вверх (прыжок)
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

    //Активировать эффект получения монеты
    private void ActivateCollectEffect(Collectable coin)
    {
        collectParticles.Play();
    }

    //Поставить игрока на землю
    private void SetPlayerToTheGround()
    {
        isPlayerInAir = false;
    }

    //Движение вниз (подкат)
    public void Crouch()
    {
        if (isCrouched)
            return;

        isCrouched = true;

        //Уменьшить коллайдер игрока
        plrColControl.ChangeColliderHeight(crouchScale);
        Invoke("RaisePlayer", crouchTime);

        //Залезть в тележку
        animControl.ChangeAnimation("GoIn");
        Invoke("StartStaying", 1f * 24/60);

        //Добавить скорость приближения к земле, если игрок в воздухе
        if (isPlayerInAir)
            rb.AddRelativeForce(Vector3.down * crouchDownForce, ForceMode.Impulse);
    }

    //Вернуть игрока к нормальному росту
    public void RaisePlayer()
    {
        plrColControl.ChangeColliderHeight(1.35846f);
        //Вылезти из тележки
        animControl.ChangeAnimation("GoOut");
        Invoke("StartRunning", 1f * 24 / 60);
        isCrouched = false;
    }

    //Начать пребыватть внутри тележки
    public void StartStaying()
    {
        animControl.ChangeAnimation("StayIn");
    }

    //Начать снова бежать
    public void StartRunning()
    {
        animControl.ChangeAnimation("Running");
    }


    //Последовательность дейтсвий при ударе о препятствие
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


    //Последовательность выполянемая при устранении игрока
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

    //Забрать управление у игрока
    public void DeactivateMovement()
    {
        isEliminated = true;
    }

    //Вернуть управление игроку
    public void ActivateMovement()
    {
        isEliminated = false;
        animControl.ChangeAnimation("Running");
    }

    //Отключить рэгдоллы и вернуть в нужное положение камеру и игрока
    public void DeactivateRagdoll()
    {
        //Вернеть камеру и игрока в мнужную позицию
        transform.rotation = Quaternion.Euler(0,0,0);
        transform.position = new Vector3(0, 0, -1.5f);
        MoveToLine();
        cam_scr.SetCameraToSrtartPosition();

        animControl.ChangeAnimation("Walk");

        if(currentDieParticle != null)
            Destroy(currentDieParticle.gameObject);
        OnRevive.Invoke();

        //Вернуть физику и убрать рэгдоллы
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

    //Восстановить здоровье игроку(
    public void SetPlayerHealth(int val)
    {
        //Установка здоровья на максимум
        currentHealth = val;
        OnLifeChange.Invoke(currentHealth);
    }

    public void RestorePlayerHealth()
    {
        //Установка здоровья на максимум
        currentHealth = maxHealth;
        OnLifeChange.Invoke(currentHealth);
    }
}
