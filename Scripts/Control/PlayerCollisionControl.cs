using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CollectableEvent : UnityEvent<Collectable> { }

public class PlayerCollisionControl : MonoBehaviour
{

    [SerializeField] public bool isFalling;

    //������� ������ ������������ � ���������
    static public UnityEvent OnGround = new UnityEvent();
    public UnityEvent OnBarrierTouch = new UnityEvent();
    public UnityEvent OnBarrierHit = new UnityEvent();

    //������� ������������� � ������������� ���������
    public static CollectableEvent OnCoinTouch = new CollectableEvent();
    public static CollectableEvent OnCollectableTouch = new CollectableEvent();
    public static CollectableEvent OnUpgradeTouch = new CollectableEvent();

    //������� �������� ������
    GameObject lastCollision = null;
    

    //������ �� �����������
    BoxCollider boxCollider;

    //������� ��������� ��� ������� �����������
    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    //������� ��������� ������ ����������
    public void ChangeColliderHeight(float newSize)
    {
        //������� ������ ���������� � ������������ � ����� �������
        bool isIncrease = boxCollider.size.y < newSize;
        float difference = Mathf.Abs(newSize - boxCollider.size.y);
        float offset = (isIncrease ? +1 : -1) * difference / 2;

        //������� ������ � �����, ����� ��������� ����� �������� � ��������� ������
        boxCollider.size = new Vector3(boxCollider.size.x, newSize, boxCollider.size.z);
        boxCollider.center = new Vector3(boxCollider.center.x, boxCollider.center.y + offset, boxCollider.center.z);
    }

    //������� �������� ������������
    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.tag == "Ground")
        {
            OnGround.Invoke();
        }
        else if (collision.gameObject.tag == "Barrier")
        {
            if (lastCollision == collision.gameObject)
                return;

            Vector3 collisionNormal = collision.contacts[0].normal;

            Debug.Log(collisionNormal);

            //������������ ������, �����
            if (Mathf.Abs(collisionNormal.x) > 0.1)
                OnBarrierTouch.Invoke();
            //������������ �������, �����
            else if (Mathf.Abs(collisionNormal.z) > 0.1)
                OnBarrierHit.Invoke();
            //������������ ������, �����
            else
                OnBarrierHit.Invoke();

            lastCollision = collision.gameObject;
        }
        else if (collision.gameObject.tag == "Walkable")
        {
            if (lastCollision == collision.gameObject)
                return;

            Vector3 collisionNormal = collision.contacts[0].normal;
            Debug.Log(collisionNormal);

            //������������ ������, �����
            if (collisionNormal.x != 0)
                OnBarrierTouch.Invoke();
            //������������ �� ������
            else if (!(collisionNormal.y > 0))
                OnBarrierHit.Invoke();
            //������������ �������, �����
            else if (collisionNormal.z != 0)
                OnBarrierHit.Invoke();
            else if (collisionNormal.y > 0)
            {

                Debug.Log("Landed");
                OnGround.Invoke();
            }


            lastCollision = collision.gameObject;
        }
        
    }

    //������������ � ���������� (�� ������ �� ������ ��������� �����)
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Coin")
        {
            OnCoinTouch.Invoke(collision.gameObject.GetComponent<Collectable>());
        }
        else if (collision.gameObject.tag == "Upgrade")
        {
            OnUpgradeTouch.Invoke(collision.gameObject.GetComponent<Collectable>());
        }
        else if (collision.gameObject.tag == "Collectable")
        {
            OnCollectableTouch.Invoke(collision.gameObject.GetComponent<Collectable>());
        }
    }

}
