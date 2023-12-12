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
    public int projectilesPerMag; // 탄창 총알 수
    public float reloadTime = .3f; // 재장전 속도

    [Header("Recoil")]
    public Vector2 kickMinMax = new Vector2(.05f, .5f); // 뒤쪽 반동 최소, 최대값
    public Vector2 recoilAngleMinMax = new Vector2(3, 5); // 위쪽 반동 최소, 최대값
    public float recoilMoveSettleTime = .1f; // 뒤쪽 반동 회복 시간
    public float recoilRotationSettleTime = .1f; // 뒤쪽 반동 회복 시간

    [Header("Effect")]
    public Transform shell; // 탄피 레퍼런스
    public Transform shellEjection; // 탄피 배출 위치 레퍼런스
    MuzzleFlash muzzleFlash; // 총구 화염 레퍼런스
    float nextShotTime; // 다음 총알 발사 시간

    bool triggerReleasedSinceLastShoot; // 지난 발사 후 방아쇠(마우스 좌클릭)을 놓았는지
    int shotsRemainingInBurst; // 점사 모드 남은 총알
    int projectilesRemainingInMag; // 탄창에 남아있는 총알
    bool isReloading; // 장전중인지 여부

    Vector3 recoilSmoothDampVelocity; // 반동 회복 속도
    float recoilRotSmoothDampVelocity; // 반동 회복 속도
    float recoilAngle; // 위쪽 반동 각도
    

    void Start() {
        muzzleFlash = GetComponent<MuzzleFlash>(); // 총구 화염 레퍼런스 할당
        shotsRemainingInBurst = burstCount; // 점사 모드 남은 총알 개수 설정
        projectilesRemainingInMag = projectilesPerMag; // 기본 탄창 총알 개수 설정
    }

    void LateUpdate() {
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, Vector3.zero, ref recoilSmoothDampVelocity, recoilMoveSettleTime);
        // SmoothDamp 를 통해 부드러운 움직임으로 현재 위치에서 원지점인 zero 까지 지정한 속도로 지정한 시간동안 움직인다.
        recoilAngle = Mathf.SmoothDamp(recoilAngle, 0, ref recoilRotSmoothDampVelocity, recoilRotationSettleTime);
        // 위와 같이 각도 또한 부드럽게 원지점으로 복구하도록 함
        transform.localEulerAngles = transform.localEulerAngles + Vector3.left * recoilAngle;
        // 위쪽 반동 각도 적용

        if(!isReloading && projectilesRemainingInMag == 0) { // 재장전중이 아니고 탄창에 총알이 없는 경우
            Reload(); // 재장전 메소드 호출
        }
    }

    void Shoot() // 총알 발사 메소드
    {
        if(!isReloading && Time.time > nextShotTime && projectilesRemainingInMag > 0) {
        // 장전중이지 않고, 총알 발사 시간이 지났고, 탄창에 총알이 있는지 확인
            // □ 무기 모드 설정에 따른 발사
            if (fireMode == FireMode.Burst) { // 무기 발사 모드가 점사인 경우
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

            // □ 총알 생성 및 발사
            for (int i = 0; i < projectileSpawn.Length; i++) { // 총알 발사 개수만큼 반복
                if(projectilesRemainingInMag == 0) { // 탄창에 총알이 없는 경우
                    break; // 루프 탈출(총알 생성 취소)
                }
                projectilesRemainingInMag--; // 탄창에서 총알 감소
                nextShotTime = Time.time + msBetweenShots / 1000; // 다음 총알 발사 시작 시간 저장
                Projectile newProjectile = Instantiate(projectile, projectileSpawn[i].position, projectileSpawn[i].rotation) as Projectile;
                //  총알 프리팹을 통해 오브젝트를 인스턴스화 하여 생성.
                newProjectile.SetSpeed(muzzleVelocity); // 총알 발사 속도 설정
            }

            // □ 무기 이펙트(탄피, 총구 화염)
            Instantiate(shell, shellEjection.position, shellEjection.rotation); // 탄피 인스턴스화 하여 생성
            muzzleFlash.Activate(); // 총구 화염 생성

            // □ 무기 반동
            transform.localPosition -= Vector3.forward * Random.Range(kickMinMax.x, kickMinMax.y); // 뒤쪽 반동값 설정
            recoilAngle += Random.Range(recoilAngleMinMax.x, recoilAngleMinMax.y); // 위쪽 반동값 설정
            recoilAngle = Mathf.Clamp(recoilAngle, 0, 30); // 무기 반동 각도 최소, 최대 범위 지정
        }
    }

    // ■ 재장전 메소드
    public void Reload() {
        if(!isReloading && projectilesRemainingInMag != projectilesPerMag) { // 재장전중이 아니고 탄창에 총알이 꽉 차있지 않은 경우
            StartCoroutine(AnimateReload()); // 재장전 애니메이션 코루틴 실행
        }
    }

    // ■ 재장전 애니메이션 코루틴
    IEnumerator AnimateReload() {
        isReloading = true; // 재장전중으로 변경
        yield return new WaitForSeconds(.2f); // 다음 코드 실행까지 0.2 초 대기

        float reloadSpeed = 1 / reloadTime; // 재장전 속도 설정
        float percent = 0; // 애니메이션 실행 비율
        Vector3 initialRot = transform.localEulerAngles; // 기존 각도
        float maxReloadAngle = 30; // 최대 장전 각도 설정

        while (percent < 1) {
            percent += Time.deltaTime * reloadSpeed; // 애니메이션 실행 비율 설정
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4; // 재장전 각도를 위한 보간값 설정
            float reloadAngle = Mathf.Lerp(0, maxReloadAngle, interpolation); // 재장전 각도 설정
            transform.localEulerAngles = initialRot + Vector3.left * reloadAngle; // 각도 적용

            yield return null; // 다음 프레임 스킵
        }

        isReloading = false; // 재장전중이 아님으로 변경
        projectilesRemainingInMag = projectilesPerMag; // 탄창 총알 수 설정
    }

    // ■ 에임 보정 메소드
    public void Aim(Vector3 aimPoint) {
        if(!isReloading) { // 재장전중이 아닐 때
            transform.LookAt(aimPoint); // 전달받은 위치를 보도록 방향 설정
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
