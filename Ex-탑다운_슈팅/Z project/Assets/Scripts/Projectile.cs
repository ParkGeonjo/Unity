using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public LayerMask collisionMask; // � ������Ʈ, ���̾ �߻�ü(�Ѿ�)�� �浹���� ����.
    public LayerMask obstacleMask; // ��ֹ� ���̾� ����ũ ���۷���

    public Color trailColour; // Ʈ���� ������ ���� ���۷���

    float speed = 10.0f; // �⺻ �ӵ�
    float damage = 1; // ������

    float lifeTime = 3; // �Ѿ� ���� �ð�

    float skinWidth = .1f; // ���� �Ÿ� ������

    void Start()
    {
        Destroy(gameObject, lifeTime); // �Ѿ� ���� �ð��� �����ٸ� ���� ������Ʈ(�Ѿ�)�� ����.

        Collider[] initialCollisions = Physics.OverlapSphere(transform.position, 0.1f, collisionMask + obstacleMask);
        // ���� ������Ʈ(�Ѿ�, �߻�ü)�� �浹���� �浹ü���� �迭 ����
        if(initialCollisions.Length > 0)
        // �� �迭�� ���̰� 0 ���� ū ���, �� �浹ü�� �ϳ��� �ִ� ���
        {
            OnHitObject(initialCollisions[0], transform.position);
            // �浹 ó�� �޼ҵ带 ȣ���ϰ� ù ��° �浹ü�� ����.
        }

        GetComponent<TrailRenderer>().material.SetColor("_Color", trailColour); // Ʈ���� ������ ���� ����
    }

    public void SetSpeed(float newSpeed) // �ӵ� ���� �޼ҵ�
    {
        speed = newSpeed; // �Է¹��� �ӵ��� ����
    }
    void Update()
    {
        float moveDistance = speed * Time.deltaTime; // �Ѿ��� �̵� �Ÿ�
        CheckCollision(moveDistance); // �Ѿ� �浹 Ȯ�� �޼ҵ� ȣ��
        transform.Translate(Vector3.forward * Time.deltaTime * speed);
        // �Ѿ� �̵�
    }

    void CheckCollision(float moveDistance) // �Ѿ� �浹 Ȯ�� �޼ҵ�
    {
        Ray ray = new Ray(transform.position, transform.forward);
        // ���� ����, �߻�ü(�Ѿ�)��ġ�� ���� ������ ����.
        RaycastHit hit;  // �浹 ������Ʈ ��ȯ ����

        if (Physics.Raycast(ray, out hit, moveDistance + skinWidth, obstacleMask, QueryTriggerInteraction.Collide))
        /* ����ĳ��Ʈ�� �߻�ü�� ��ֹ� ������Ʈ ���̾ ��Ҵ��� Ȯ��.
           QueryTriggerInteraction �� Collide �� �Ͽ� Ʈ���� �ݶ��̴��� �浹 �ϰ� ����. */
        {
            GameObject.Destroy(gameObject); // ���� ������Ʈ(�߻�ü) ���� 
        }
        else if (Physics.Raycast(ray, out hit, moveDistance + skinWidth, collisionMask, QueryTriggerInteraction.Collide))
        /* ����ĳ��Ʈ�� �߻�ü�� ������Ʈ�� ���̾ ��Ҵ��� Ȯ��.
           QueryTriggerInteraction �� Collide �� �Ͽ� Ʈ���� �ݶ��̴��� �浹 �ϰ� ����. */
        {
            OnHitObject(hit.collider, hit.point); // �浹 ó�� �޼ҵ� ȣ��
        }
    }

    void OnHitObject(Collider c, Vector3 hitPoint) // �浹 �� ó�� �޼ҵ�
    {
        IDamageable damageableObject = c.GetComponent<IDamageable>();
        // �������̽� ���� ����, �߻�ü�� ���� ������Ʈ�� �������̽��� ������ ����.
        if (damageableObject != null)
        /* �� ������ null �� �ƴ� ��,
           �� ���� ������Ʈ�� �������̽��� �ִ�, �������� �޴� ������Ʈ�� ���. */
        {
            damageableObject.TakeHit(damage, hitPoint, transform.forward);
            // �ش� ������Ʈ �������̽��� Ÿ�� �޼ҵ带 ȣ��
        }
        GameObject.Destroy(gameObject); // ���� ������Ʈ(�߻�ü) ����
    }
}
