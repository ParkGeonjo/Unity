using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    public Rigidbody myRigidbody; // 리지드바디 레퍼런스
    public float forceMin; // 가해지는 힘 최소값
    public float forceMax; // 가해지는 힘 최대값

    float lifeTime = 1; // 탄피 유지 시간
    float fadeTime = 1; // 탄피 사라지는 모션 시간

    void Start() {
        float force = Random.Range(forceMin, forceMax); // 가해지는 힘 범위내에서 랜덤으로 설정
        myRigidbody.AddForce(transform.right * force); // 리지드바디를 통해 우측면으로 힘 전달
        myRigidbody.AddTorque(Random.insideUnitSphere * force); // 단위 구체 내부에 랜덤 값을 곱해 랜덤 회전을 준다.

        StartCoroutine(Fade()); // 페이드 아웃 효과 코루틴 실행
    }

    // ■ 탄피 페이드 아웃 코루틴
    IEnumerator Fade() {
        yield return new WaitForSeconds(lifeTime); // lifeTime 후에 실행.

        float percent = 0; // 페이드 아웃 효과 퍼센트
        float fadeSpeed = 1 / fadeTime; // 페이드 아웃 효과 시간
        Material mat = GetComponent<Renderer>().material; // 머터리얼 레퍼런스
        Color initialColour = mat.color; // 기존 색상 저장

        while (percent < 1) { // 페이드 아웃 효과가 끝나지 않았다면
            percent += Time.deltaTime * fadeSpeed; // 페이드 아웃 효과 퍼센트 계산

            mat.color = Color.Lerp(initialColour, Color.clear, percent); // 탄피 색상 설정
            yield return null; // 다음 프레임 이후 반복
        }

        Destroy(gameObject); // 이 오브젝트 파괴
    }
}
