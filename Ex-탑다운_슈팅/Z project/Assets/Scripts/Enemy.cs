using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
// ���� ������Ʈ�� NavMeshAgent �� ����
public class Enemy : LivingEntity
{
    public enum State { Idle, Cashing, Attacking };
    // ���� ����, { �⺻(�ƹ��͵� ����), ����, ���� }
    State currentState; // ���� ����

    public ParticleSystem deathEffect; // ��� ����Ʈ ���۷���

    UnityEngine.AI.NavMeshAgent pathfinder; // ������̼� ���۷���
    Transform target; // ���� Ÿ��(�÷��̾�) Ʈ������
    LivingEntity targetEntity; // Ÿ��(�÷��̾�) ���۷���

    Material skinMatreial; // ���� ������Ʈ�� ���׸���
    Color originalColor; // ���� ������Ʈ�� ���� ����

    float attackDistanceThreshold = 0.5f;
    // ���� �Ÿ� �Ӱ谪(��Ÿ�). ����Ƽ���� ���� 1�� 1 meter �̴�..!!
    float timeBetweenAttacks = 1;
    // ���� �ð� ����
    float nextAttackTime;
    // ���� ���� ���� �ð�
    float damage = 1;
    // ���� ���� ������

    float myCollisionRadius; // �ڽ��� �浹 ���� ������
    float targetConllisionRadius; // Ÿ���� �浹 ���� ������

    bool hasTarget; // Ÿ���� �ִ��� Ȯ��

    protected override void Start()
    // override �� �θ� Ŭ������ �޼ҵ带 ������.
    {
        base.Start(); // base �� ���� �θ� Ŭ������ ���� �޼ҵ带 ȣ��.
        pathfinder = GetComponent<UnityEngine.AI.NavMeshAgent>();
        // NavMeshAgent ���۷��� ����

        skinMatreial = GetComponent<Renderer>().material; // ���� ������Ʈ�� ���׸��� ����.
        originalColor = skinMatreial.color; // ���� ���� ����

        if(GameObject.FindGameObjectWithTag("Player") != null)
        // �� ���� �� �÷��̾� ������Ʈ�� �ִ�(�÷��̾ ����ִ�) ���
        {
            currentState = State.Cashing; // �⺻ ���¸� ���� ���·� ����

            hasTarget = true; // Ÿ��(�÷��̾�) �������� ����

            target = GameObject.FindGameObjectWithTag("Player").transform;
            // Ÿ������ "Player" ��� �±׸� ���� ������Ʈ�� Ʈ�������� ����

            targetEntity = target.gameObject.GetComponent<LivingEntity>();
            // ������ ���� �� �÷��̾� ������Ʈ�� LivingEntity ������Ʈ�� ����

            targetEntity.OnDeath += OnTargetDeath;
            // Ÿ�� ��� �޼ҵ� �߰�.

            myCollisionRadius = GetComponent<CapsuleCollider>().radius;
            // �ڽ��� �浹ü(collider)�� �������� ����
            targetConllisionRadius = target.GetComponent<CapsuleCollider>().radius;
            // Ÿ���� �浹ü(collider)�� �������� ����

            StartCoroutine(UpdatePath());
            // ������ �ð����� ������ ������ ��ġ�� �����ϴ� �ڷ�ƾ ����
        }
    }

    // �� Ÿ�� �޼ҵ� �������̵�
    public override void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection) {
        if(damage >= health) { // �������� ���� ü�� �̻��� ���
            Destroy(Instantiate(deathEffect.gameObject, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitDirection)) as GameObject, deathEffect.startLifetime);
            // ����Ʈ(��ƼŬ)�� �ν��Ͻ�ȭ �Ͽ� ����(FromToRotation ���� ���� ����), ������ �ð���� �� �ı�
        }
        base.TakeHit(damage, hitPoint, hitDirection);
        // �θ� Ŭ������ ���� TakeHit �޼ҵ� ȣ��
    }

    void OnTargetDeath() // Ÿ��(�÷��̾�)�� �׾��� �� ȣ��Ǵ� �޼ҵ�
    {
        hasTarget = false; // Ÿ�� �������� ����
        currentState = State.Idle; // ���� ���¸� �⺻(����)���·� ����
    }

    void Update()
    {
        if (hasTarget) // Ÿ��(�÷��̾�)�� �ִ� ���
        {
            if (Time.time >= nextAttackTime) // ���� �ð��� ������
            {
                float sqrDstToTarget = (target.position - transform.position).sqrMagnitude;
                /* �ڽ�(��)�� ��ǥ(�÷��̾�) ������ �Ÿ��� ����.
                   �� ������Ʈ ������ �Ÿ��� �����ϱ� ���� Vector3 �� Distace �޼ҵ带
                   ���� ����� ������ ���Ϳ� ���� ������ ���� ���� ���� ���� ����.

                   �� ������Ʈ ������ ���� �Ÿ��� �ʿ��� ���� �ƴ� �񱳰��� �ʿ� ��
                   ���̹Ƿ� ���� ���� ������ �ٿ� �� �� �ִ�. */

                if (sqrDstToTarget <= Mathf.Pow(attackDistanceThreshold + targetConllisionRadius + myCollisionRadius, 2))
                // �Ÿ��� ������ �Ӱ� �Ÿ��� ������ ���Ͽ� ���� ���� ������ Ȯ��
                {
                    nextAttackTime = Time.time + timeBetweenAttacks;
                    // ���� ���� �ð� ����
                    StartCoroutine(Attack());
                    // ���� �ڷ�ƾ ����
                }
            }
        }
    }

    IEnumerator Attack() // ���� ���� �ڷ�ƾ
    {
        currentState = State.Attacking; // ���� ���·� ����
        pathfinder.enabled = false; // ���� �� �÷��̾� ���� ����


        Vector3 originalPosition = transform.position; // �ڽ�(��)�� ��ġ
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        // Ÿ�������� ���� ���� ���
        Vector3 attackPosition = target.position - dirToTarget * (myCollisionRadius);
        // Ÿ��(�÷��̾�)�� ��ġ

        float attackSpeed = 3; // ���� �ӵ�
        float percent = 0;

        skinMatreial.color = Color.magenta; // ���� �� ���� ����

        bool hasAppliedDamage = false; // �������� ���� ������ Ȯ��

        while(percent <= 1) // ��� �Ÿ��� 1 ������ �� ����
        {
            if(percent >= .5f && hasAppliedDamage == false)
            // ���� ���� ������ �����߰� �������� ���������� ���� ���
            {
                hasAppliedDamage = true; // ������ ���� ������ ����
                targetEntity.TakeDamage(damage); // Ÿ��(�÷��̾�)���� ������ ����
            }
            percent += Time.deltaTime * attackSpeed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            /* ��Ī �Լ��� ���
               ������->�������� �̵����� ���� ���� �����Ѵ�. */
            transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation);
            /* Lerp �޼ҵ�� �������� ��ȯ�Ѵ�.
               ����, ��������, ���� ���� ���������� �����Ѵ�
               ���� ���� 0�̸� ������, 1�̸� ���������� �ְ� �ȴ�. */

            yield return null;
            /* while ������ ó�� ���̿��� �������� ��ŵ�մϴ�.
               �� �������� �۾��� ���߰� Update �޼ҵ� �۾��� ������
               ���� ���������� �Ѿ�� �� �Ʒ��� �ڵ峪 ������ ���� */ 
        }

        skinMatreial.color = originalColor; // ������ ������ ���� �������� ����
        currentState = State.Cashing; // ������ ������ ���� ���·� ����
        pathfinder.enabled = true; // ������ ������ �ٽ� �÷��̾� ����
    }

    IEnumerator UpdatePath()
    // �ش� �ڷ�ƾ ���� �� ������ �ð����� ������ ���� �ڵ� �ݺ�
    {
        float refreshRate = .25f; // ���� �ð�
        while(hasTarget) // Ÿ���� ���� �� �ݺ�
        {
            if(currentState == State.Cashing)
            // ���°� ���� ������ ���
            {
                Vector3 dirToTarget = (target.position - transform.position).normalized;
                // Ÿ�������� ���� ���� ���
                Vector3 targetPosition = target.position - dirToTarget * (myCollisionRadius + targetConllisionRadius + attackDistanceThreshold / 2);
                /* �ڽ��� ��ġ���� �ڽŰ� Ÿ���� ������ ���̿� ���� ��Ÿ��� ������ ���ϰ�, ���� ���͸� ���� ���� ����.
                   ��, Ÿ��(�÷��̾�) ���� ������ ��ġ�� Ÿ�� ��ġ�� ���Ѵ�. */
                if (!dead) // ���� ���� ���
                {
                    pathfinder.SetDestination(targetPosition);
                    // ������̼��� �������� Ÿ��(�÷��̾�)�� ��ġ�� ����
                }
            }
            yield return new WaitForSeconds(refreshRate);
            // refreshRate �ð���ŭ ��ٸ�
        }
    }

}
