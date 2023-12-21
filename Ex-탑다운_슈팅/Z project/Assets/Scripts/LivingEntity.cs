using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour, IDamageable // 인터페이스 상속
{
    public float startingHealth; // 시작 체력
    public float health { get; protected set; } // 체력
    /* protected 를 통해 상속 관계가 없는 클래스에서 사용할 수 없게 한다.
       인스펙터에서도 보이지 않음. */
    protected bool dead; // 캐릭터가 죽었는지

    public event System.Action OnDeath;
    /* System.Action 은 델리게이트 메소드이다.
       여기서 델리게이트란 C++ 의 함수 포인터와 유사하게
       메소드의 위치를 가르키고 불러올 수 있는 타입이다.
       void 를 리턴하고 입력을 받지 않는다. */

    protected virtual void Start()
    // 상속 받는 클래스에서 같은 메소드를 재정의 할 수 있도록 virtual 로 선언.
    {
        health = startingHealth; // 시작 체력 설정
    }

    // ■ 타격 메소드
    public virtual void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection) {
        TakeDamage(damage); // 데미지 받는 메소드 호출
    }

    // ■ 데미지 받는 메소드
    public virtual void TakeDamage(float damage) {
        health -= damage; // 체력에서 데미지만큼 감소
        if (health <= 0 && !dead) // 체력이 0 이하고 죽은 경우
        {
            Die(); // 죽음 메소드 호출
        }
    }

    // 인스펙터에서 스크립트 컴포넌트 우클릭 시 자체 파괴 버튼 추가
    [ContextMenu("Self-Destruct")]
    public virtual void Die() // 죽음 메소드
    {
        dead = true; // 죽은 판정으로 변경.
        if(OnDeath != null) // 이벤트가 있는 경우
        {
            OnDeath(); // 메소드를 호출
        }
        GameObject.Destroy(gameObject); // 현재 오브젝트 파괴
    }
}
