using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
// 현재 오브젝트에 NavMeshAgent 를 포함
public class Enemy : LivingEntity
{
    public enum State { Idle, Cashing, Attacking };
    // 적의 상태, { 기본(아무것도 안함), 추적, 공격 }
    State currentState; // 현재 상태

    public ParticleSystem deathEffect; // 사망 이펙트 레퍼런스

    UnityEngine.AI.NavMeshAgent pathfinder; // 내비게이션 레퍼런스
    Transform target; // 적의 타겟(플레이어) 트랜스폼
    LivingEntity targetEntity; // 타겟(플레이어) 레퍼런스

    Material skinMatreial; // 현재 오브젝트의 마테리얼
    Color originalColor; // 현재 오브젝트의 기존 색상

    float attackDistanceThreshold = 0.5f;
    // 공격 거리 임계값(사거리). 유니티에서 단위 1은 1 meter 이다..!!
    float timeBetweenAttacks = 1;
    // 공격 시간 간격
    float nextAttackTime;
    // 실제 다음 공격 시간
    float damage = 1;
    // 적의 공격 데미지

    float myCollisionRadius; // 자신의 충돌 범위 반지름
    float targetConllisionRadius; // 타겟의 충돌 범위 반지름

    bool hasTarget; // 타겟이 있는지 확인

    protected override void Start()
    // override 로 부모 클래스의 메소드를 재정의.
    {
        base.Start(); // base 를 통해 부모 클래스의 기존 메소드를 호출.
        pathfinder = GetComponent<UnityEngine.AI.NavMeshAgent>();
        // NavMeshAgent 레퍼런스 생성

        skinMatreial = GetComponent<Renderer>().material; // 현재 오브젝트의 마테리얼 저장.
        originalColor = skinMatreial.color; // 현재 색상 저장

        if(GameObject.FindGameObjectWithTag("Player") != null)
        // 적 스폰 후 플레이어 오브젝트가 있는(플레이어가 살아있는) 경우
        {
            currentState = State.Cashing; // 기본 상태를 추적 상태로 설정

            hasTarget = true; // 타겟(플레이어) 있음으로 설정

            target = GameObject.FindGameObjectWithTag("Player").transform;
            // 타겟으로 "Player" 라는 태그를 가진 오브젝트의 트랜스폼을 저장

            targetEntity = target.gameObject.GetComponent<LivingEntity>();
            // 위에서 저장 한 플레이어 오브젝트의 LivingEntity 컴포넌트를 저장

            targetEntity.OnDeath += OnTargetDeath;
            // 타겟 사망 메소드 추가.

            myCollisionRadius = GetComponent<CapsuleCollider>().radius;
            // 자신의 충돌체(collider)의 반지름을 저장
            targetConllisionRadius = target.GetComponent<CapsuleCollider>().radius;
            // 타겟의 충돌체(collider)의 반지름을 저장

            StartCoroutine(UpdatePath());
            // 지정된 시간마다 목적지 쪽으로 위치를 갱신하는 코루틴 실행
        }
    }

    // ■ 타격 메소드 오버라이딩
    public override void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection) {
        if(damage >= health) { // 데미지가 현재 체력 이상인 경우
            Destroy(Instantiate(deathEffect.gameObject, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitDirection)) as GameObject, deathEffect.startLifetime);
            // 이펙트(파티클)을 인스턴스화 하여 생성(FromToRotation 으로 방향 설정), 설정한 시간경과 후 파괴
        }
        base.TakeHit(damage, hitPoint, hitDirection);
        // 부모 클래스의 기존 TakeHit 메소드 호출
    }

    void OnTargetDeath() // 타겟(플레이어)이 죽었을 때 호출되는 메소드
    {
        hasTarget = false; // 타겟 없음으로 설정
        currentState = State.Idle; // 현재 상태를 기본(정지)상태로 변경
    }

    void Update()
    {
        if (hasTarget) // 타겟(플레이어)가 있는 경우
        {
            if (Time.time >= nextAttackTime) // 공격 시간이 지나면
            {
                float sqrDstToTarget = (target.position - transform.position).sqrMagnitude;
                /* 자신(적)과 목표(플레이어) 사이의 거리를 저장.
                   두 오브젝트 사이의 거리를 측정하기 위해 Vector3 의 Distace 메소드를
                   쓰는 방법도 있지만 벡터에 제곱 연산이 들어가다 보니 연산 수가 많다.

                   두 오브젝트 사이의 실제 거리가 필요한 것이 아닌 비교값이 필요 한
                   것이므로 위와 같이 연산을 줄여 볼 수 있다. */

                if (sqrDstToTarget <= Mathf.Pow(attackDistanceThreshold + targetConllisionRadius + myCollisionRadius, 2))
                // 거리의 제곱과 임계 거리의 제곱을 비교하여 공격 범위 내인지 확인
                {
                    nextAttackTime = Time.time + timeBetweenAttacks;
                    // 다음 공격 시간 지정
                    StartCoroutine(Attack());
                    // 공격 코루틴 실행
                }
            }
        }
    }

    IEnumerator Attack() // 적의 공격 코루틴
    {
        currentState = State.Attacking; // 공격 상태로 변경
        pathfinder.enabled = false; // 공격 중 플레이어 추적 중지


        Vector3 originalPosition = transform.position; // 자신(적)의 위치
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        // 타겟으로의 방향 벡터 계산
        Vector3 attackPosition = target.position - dirToTarget * (myCollisionRadius);
        // 타겟(플레이어)의 위치

        float attackSpeed = 3; // 공격 속도
        float percent = 0;

        skinMatreial.color = Color.magenta; // 공격 시 색상 지정

        bool hasAppliedDamage = false; // 데미지를 적용 중인지 확인

        while(percent <= 1) // 찌르는 거리가 1 이하일 때 루프
        {
            if(percent >= .5f && hasAppliedDamage == false)
            // 적이 공격 지점에 도달했고 데미지를 적용중이지 않은 경우
            {
                hasAppliedDamage = true; // 데미지 적용 중으로 설정
                targetEntity.TakeDamage(damage); // 타겟(플레이어)에게 데미지 적용
            }
            percent += Time.deltaTime * attackSpeed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            /* 대칭 함수를 사용
               원지점->공격지점 이동에서 보간 값을 참조한다. */
            transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation);
            /* Lerp 메소드는 내분점을 반환한다.
               원점, 공격지점, 보간 값을 참조값으로 전달한다
               보간 값이 0이면 원점에, 1이면 공격지점에 있게 된다. */

            yield return null;
            /* while 루프의 처리 사이에서 프레임을 스킵합니다.
               이 지점에서 작업이 멈추고 Update 메소드 작업이 끝나고
               다음 프레임으로 넘어가면 이 아래의 코드나 루프가 실행 */ 
        }

        skinMatreial.color = originalColor; // 공격이 끝나면 기존 색상으로 변경
        currentState = State.Cashing; // 공격이 끝나면 추적 상태로 변경
        pathfinder.enabled = true; // 공격이 끝나면 다시 플레이어 추적
    }

    IEnumerator UpdatePath()
    // 해당 코루틴 실행 시 지정한 시간마다 루프문 내부 코드 반복
    {
        float refreshRate = .25f; // 루프 시간
        while(hasTarget) // 타겟이 있을 때 반복
        {
            if(currentState == State.Cashing)
            // 상태가 추적 상태인 경우
            {
                Vector3 dirToTarget = (target.position - transform.position).normalized;
                // 타겟으로의 방향 벡터 계산
                Vector3 targetPosition = target.position - dirToTarget * (myCollisionRadius + targetConllisionRadius + attackDistanceThreshold / 2);
                /* 자신의 위치에서 자신과 타겟의 반지름 길이에 공격 사거리의 절반을 더하고, 방향 벡터를 곱한 값을 뺀다.
                   즉, 타겟(플레이어) 공격 가능한 위치를 타겟 위치로 정한다. */
                if (!dead) // 죽지 않은 경우
                {
                    pathfinder.SetDestination(targetPosition);
                    // 내비게이션의 목적지를 타겟(플레이어)의 위치로 설정
                }
            }
            yield return new WaitForSeconds(refreshRate);
            // refreshRate 시간만큼 기다림
        }
    }

}
