using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public enum FireMode {Auto, Burst, Single}; // 총 격발 모드 (자동, 점사, 단발)
    public FireMode fireMode; // 격발 모드 변수

    public Transform[] projectileSpawn; // 총알 발사 위치 배열
    public Projectile projectile; // 총알
    public float msBetweenShots = 100; // 총알 발사 시간 간격
    public float muzzleVelocity = 35; // 총알 발사 속도

    public int burstCount; // 점사 모드 총알 발사 개수
    public int shotsRemainingInBurst; // 점사 모드 남은 총알

    public Transform shell; // 탄피 레퍼런스
    public Transform shellEjection; // 탄피 배출 위치 레퍼런스

    MuzzleFlash muzzleFlash; // 총구 화염 레퍼런스

    float nextShotTime; // 다음 총알 발사 시간

    bool triggerReleasedSinceLastShoot; // 지난 발사 후 방아쇠(마우스 좌클릭)을 놓았는지

    void Start() {
        muzzleFlash = GetComponent<MuzzleFlash>(); // 총구 화염 레퍼런스 할당
        shotsRemainingInBurst = burstCount; // 점사 모드 남은 총알 개수 설정
    }

    void Shoot() // 총알 발사 메소드
    {
        if(Time.time > nextShotTime) // 총알 발사 시간이 지났는지 확인
        {
            if(fireMode == FireMode.Burst) { // 무기 발사 모드가 점사인 경우
                if(shotsRemainingInBurst == 0) { // 발사 할 남은 총알이 없는 경우
                    return; // 아래 코드(총알 발사) 실행 건너뜀
                }
                shotsRemainingInBurst--; // 남은 총알 개수 감소
            }
            else if (fireMode == FireMode.Single) { // 무기 발사 모드가 단발인 경우
                if (!triggerReleasedSinceLastShoot) { // 이미 총알을 발사 하고 방아쇠를 당기고(마우스 좌클릭 중) 있는 경우
                    return; // 아래 코드(총알 발사) 실행 건너뜀 
                }
            }

            for (int i = 0; i < projectileSpawn.Length; i++) { // 총알 발사 개수만큼 반복
                nextShotTime = Time.time + msBetweenShots / 1000; // 다음 총알 발사 시작 시간 저장
                Projectile newProjectile = Instantiate(projectile, projectileSpawn[i].position, projectileSpawn[i].rotation) as Projectile;
                //  총알 프리팹을 통해 오브젝트를 인스턴스화 하여 생성.
                newProjectile.SetSpeed(muzzleVelocity); // 총알 발사 속도 설정
            }

            Instantiate(shell, shellEjection.position, shellEjection.rotation); // 탄피 인스턴스화 하여 생성
            muzzleFlash.Activate(); // 총구 화염 생성
        }
    }

    // ■ 방아쇠(마우스 좌클릭)를 누르고 있을 때
    public void OnTriggerHold() {
        Shoot(); // 총알 발사
        triggerReleasedSinceLastShoot = false; // 방아쇠 잡은 상태로
    }

    // ■ 방아쇠(마우스 좌클릭)를 놓았을 때
    public void OnTriggerRelease() {
        triggerReleasedSinceLastShoot = true; // 방아쇠 놓은 상태로

        shotsRemainingInBurst = burstCount; // 점사 모드 남은 총알 개수 설정
    }
}
