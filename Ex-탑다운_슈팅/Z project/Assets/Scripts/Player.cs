using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GunController))] // GunController 스크립트를 포함
[RequireComponent(typeof(PlayerController))] // PlayerController 스크립트를 포함
public class Player : LivingEntity
{
    public float moveSpeed = 5.0f; // 플레이어 이동속도

    public Crosshair crosshairs; // 조준점 레퍼런스

    Camera viewCamera; // 카메라
    PlayerController controller; // 플레이어 컨트롤러
    GunController gunController; // 총 컨트롤러

    protected override void Start() { // override 로 부모 클래스의 메소드를 재정의.
        base.Start(); // base 를 통해 부모 클래스의 기존 메소드를 호출.
    }

    void Awake() {
        controller = GetComponent<PlayerController>();
        // 현재 오브젝트의 플레이어 컨트롤러를 가져옴
        gunController = GetComponent<GunController>();
        // 현재 오브젝트의 총 컨트롤러를 가져옴
        viewCamera = Camera.main;
        // 카메라
        FindObjectOfType<Spawner>().OnNewWave += OnNewWave;
        // Spawner 의 새 웨이브 시작 이벤트 구독
    }

    // ■ 새 웨이브 시작 시 메소드
    void OnNewWave(int waveNumber) {
        health = startingHealth; // 체력 초기화
        gunController.EquipGun(waveNumber - 1); // 장착할 총 설정
    }

    void Update() {
        // 이동 입력
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxis("Vertical"));
        // GetAxisRaw 로 스무딩 X
        Vector3 moveVelocity = moveInput.normalized * moveSpeed;
        // normalized 로 단위벡터로 변환, 이동 속도를 계산
        controller.Move(moveVelocity);
        // 이동 속도를 이동 메소드로 전달

        // 방향 입력
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        // 카메라 -> 마우스 커서 위치로 레이 발사.
        Plane groundPlane = new Plane(Vector3.up, Vector3.up * gunController.GunHeight);
        // 평면의 법선벡터 생성.
        float rayDistance;
        // 위에서 쏜 레이와 평면의 교차지점까지 거리

        if (groundPlane.Raycast(ray, out rayDistance)) {
            // 레이가 평면과 교차하는 경우 교차지점까지의 거리를 rayDistance 에 저장.
            Vector3 point = ray.GetPoint(rayDistance);
            // GetPoint 와 교차지점까지의 거리로 교차지점의 위치를 저장.
            // 카메라부터 교차지점까지 선으로 표시 : Debug.DrawLine(ray.origin, point, Color.red);

            controller.LookAt(point);
            // 플레이어 컨트롤러의 방향 전환 메소드에 교차 지점(방향) 전달.
            crosshairs.transform.position = point;
            // 조준점 위치 설정
            crosshairs.DetectTarget(ray);
            // 조준점 레이캐스트

            if((new Vector2(point.x, point.z) - new Vector2(transform.position.x, transform.position.z)).sqrMagnitude > 1) {
            // 조준점(커서)와 플레이어 사이 거리가 1 이상일 때
                gunController.Aim(point);
                // 총 에임 보정 위치 전달
            }
        }

        // 무기 조작 입력
        if (Input.GetMouseButton(0)) // 마우스 왼쪽 버튼 클릭 시
        {
            gunController.OnTriggerHold(); // 방아쇠 당김 메소드 호출
        }

        if (Input.GetMouseButtonUp(0)) // 마우스 왼쪽 버튼 클릭 후 손을 뗄 시
        {
            gunController.OnTriggerRelease(); // 방아쇠 놓음 메소드 호출
        }

        if (Input.GetKeyDown(KeyCode.R)) // 키보드 R 버튼 누를 경우
        {
            gunController.Reload(); // 재장전 메소드 호출
        }
    }
}
