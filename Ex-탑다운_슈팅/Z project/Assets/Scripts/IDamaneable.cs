using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamaneable // 인터페이스 생성.
{
    void TakeHit(float damage, RaycastHit hit); // 데미지를 받는 메소드
    void TakeDamage(float damage); // 데미지를 받는 메소드(간략화)
}
