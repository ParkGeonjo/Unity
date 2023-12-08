using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))] // 현재 오브젝트에 Rigidbody 포함

public class PlayerController : MonoBehaviour
{
    Vector3 velocity; // 플레이어 이동속도
    Rigidbody myRigidbody; // 리지드바디
    void Start() {
        myRigidbody = GetComponent<Rigidbody>();
        // 현재 오브젝트의 리지드바디를 가져옴
    }

    public void Move(Vector3 _velocity) {
        // 플레이어 이동 메소드 (사실상 이동속도 설정)
        velocity = _velocity; // 이동속도에 전달받은 이동속도 저장
    }

    public void LookAt(Vector3 lookPoint){ // 플레이어 방향 전환 메소드
        Vector3 heightCorrectedPoint = new Vector3(lookPoint.x, transform.position.y, lookPoint.z);
        // 새로운 벡터(방향) 생성, 전달받은 위치에서 y 값만 현재 플레이어 y 값으로 설정.
        transform.LookAt(heightCorrectedPoint);
        // LookAt 메소드에 위치(방향)전달하여 방향 전환.
    }

    private void FixedUpdate() {
        myRigidbody.MovePosition(myRigidbody.position + velocity * Time.fixedDeltaTime);
        // MovePosition 에 위치 + 속도 + fixedDeltaTime 전달하여 플레이어 이동
        // FixedUpdate 로 물리 업데이트에 따라 이동, 프레임 차이에도 동일한 이동 가능.
    }
}
