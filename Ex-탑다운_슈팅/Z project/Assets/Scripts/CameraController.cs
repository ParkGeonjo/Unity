using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject Player; // 플레이어 오브젝트
    public Vector3 offset; // 플레이어와 카메라 간격

    private void Start() {
        offset = this.transform.position - Player.transform.position; // 간격 설정
    }

    private void Update() {
        if (Player != null) { // 플레이어 오브젝트가 있는 경우
            this.transform.position = Player.transform.position + offset;
            // 현재 오브젝트(카메라) 위치를 플레이어 위치 + 간격 으로 설정
        }
    }
}
