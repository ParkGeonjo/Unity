using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable // 인터페이스 생성.
{
    void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection); // 타격을 받는 메소드
    void TakeDamage(float damage); // 데미지를 받는 메소드
}
