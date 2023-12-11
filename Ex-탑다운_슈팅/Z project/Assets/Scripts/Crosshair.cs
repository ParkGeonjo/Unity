using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    public LayerMask targetMask; // 타겟(적)마스크 레퍼런스

    public SpriteRenderer dot; // 점 스프라이트 렌더러 레퍼런스
    public Color dotHighlightColour; // 점 하이라이트 색상
    Color originalDotColour; // 점 기존 색상

    void Start() {
        Cursor.visible = false; // 커서 안보이도록 설정
        originalDotColour = dot.color; // 점 기존 색상 저장
    }

    void Update() {
        transform.Rotate(Vector3.forward * -40 * Time.deltaTime); // 회전
    }

    // ■ 조준점 레이캐스트 메소드
    public void DetectTarget(Ray ray) {
        if (Physics.Raycast(ray, 100, targetMask)) { // 타겟 마스크에 레이가 닿은 경우
            dot.color = dotHighlightColour; // 점 색상 하이라이트 색상으로 변경
        }
        else {
            dot.color = originalDotColour; // 점 색상 기존 색상으로 변경
        }
    }
}
