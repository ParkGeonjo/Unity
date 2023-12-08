using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public LayerMask collisionMask; // 어떤 오브젝트, 레이어가 발사체(총알)과 충돌할지 저장.
    public LayerMask obstacleMask; // 장애물 레이어 마스크 레퍼런스

    public Color trailColour; // 트레일 렌더러 색상 레퍼런스

    float speed = 10.0f; // 기본 속도
    float damage = 1; // 데미지

    float lifeTime = 3; // 총알 유지 시간

    float skinWidth = .1f; // 레이 거리 보정값

    void Start()
    {
        Destroy(gameObject, lifeTime); // 총알 유지 시간이 지났다면 현재 오브젝트(총알)을 제거.

        Collider[] initialCollisions = Physics.OverlapSphere(transform.position, 0.1f, collisionMask + obstacleMask);
        // 현재 오브젝트(총알, 발사체)와 충돌중인 충돌체들의 배열 생성
        if(initialCollisions.Length > 0)
        // 위 배열의 길이가 0 보다 큰 경우, 즉 충돌체가 하나라도 있는 경우
        {
            OnHitObject(initialCollisions[0], transform.position);
            // 충돌 처리 메소드를 호출하고 첫 번째 충돌체를 전달.
        }

        GetComponent<TrailRenderer>().material.SetColor("_Color", trailColour); // 트레일 렌더러 색상 설정
    }

    public void SetSpeed(float newSpeed) // 속도 설정 메소드
    {
        speed = newSpeed; // 입력받은 속도로 설정
    }
    void Update()
    {
        float moveDistance = speed * Time.deltaTime; // 총알의 이동 거리
        CheckCollision(moveDistance); // 총알 충돌 확인 메소드 호출
        transform.Translate(Vector3.forward * Time.deltaTime * speed);
        // 총알 이동
    }

    void CheckCollision(float moveDistance) // 총알 충돌 확인 메소드
    {
        Ray ray = new Ray(transform.position, transform.forward);
        // 레이 생성, 발사체(총알)위치와 정면 방향을 전달.
        RaycastHit hit;  // 충돌 오브젝트 반환 정보

        if (Physics.Raycast(ray, out hit, moveDistance + skinWidth, obstacleMask, QueryTriggerInteraction.Collide))
        /* 레이캐스트로 발사체가 장애물 오브젝트 레이어에 닿았는지 확인.
           QueryTriggerInteraction 을 Collide 로 하여 트리거 콜라이더와 충돌 하게 설정. */
        {
            GameObject.Destroy(gameObject); // 현재 오브젝트(발사체) 제거 
        }
        else if (Physics.Raycast(ray, out hit, moveDistance + skinWidth, collisionMask, QueryTriggerInteraction.Collide))
        /* 레이캐스트로 발사체가 오브젝트나 레이어에 닿았는지 확인.
           QueryTriggerInteraction 을 Collide 로 하여 트리거 콜라이더와 충돌 하게 설정. */
        {
            OnHitObject(hit.collider, hit.point); // 충돌 처리 메소드 호출
        }
    }

    void OnHitObject(Collider c, Vector3 hitPoint) // 충돌 시 처리 메소드
    {
        IDamageable damageableObject = c.GetComponent<IDamageable>();
        // 인터페이스 변수 생성, 발사체에 맞은 오브젝트의 인터페이스를 가져와 저장.
        if (damageableObject != null)
        /* 위 변수가 null 이 아닐 때,
           즉 맞은 오브젝트에 인터페이스가 있는, 데미지를 받는 오브젝트인 경우. */
        {
            damageableObject.TakeHit(damage, hitPoint, transform.forward);
            // 해당 오브젝트 인터페이스의 타격 메소드를 호출
        }
        GameObject.Destroy(gameObject); // 현재 오브젝트(발사체) 제거
    }
}
