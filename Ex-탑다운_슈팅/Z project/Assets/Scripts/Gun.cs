using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Transform muzzle; // 총알 발사 위치
    public Projectile projectile; // 총알
    public float msBetweenShots = 100; // 총알 발사 시간 간격
    public float muzzleVelocity = 35; // 총알 발사 속도

    float nextShotTime; // 다음 총알 발사 시간

    public void Shoot() // 총알 발사 메소드
    {
        if(Time.time > nextShotTime) // 총알 발사 시간이 지났는지 확인
        {
            nextShotTime = Time.time + msBetweenShots / 1000; // 다음 총알 발사 시작 시간 저장
            Projectile newProjectile = Instantiate(projectile, muzzle.position, muzzle.rotation) as Projectile;
            //  총알 프리팹을 통해 오브젝트를 인스턴스화 하여 생성.
            newProjectile.SetSpeed(muzzleVelocity); // 총알 발사 속도 설정
        }
    }
}
