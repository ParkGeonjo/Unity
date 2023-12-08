using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{
    public GameObject flashHolder; // 화염 효과 오브젝트 레퍼런스
    public Sprite[] flashSprites; // 화염 효과 스프라이트 배열
    public SpriteRenderer[] spriteRenderer; // 화염 효과 스프라이트 렌더러 배열

    public float flashTime; // 화염 효과 지속 시간

    // ■ 화염 효과 활성화 메소드
    public void Activate() {
        flashHolder.SetActive(true); // 화염 효과 오브젝트 활성화

        int flashSpriteIndex = Random.Range(0, flashSprites.Length); // 스프라이트 인덱스 랜덤 지정
        for(int i = 0; i < spriteRenderer.Length; i++) {
            spriteRenderer[i].sprite = flashSprites[flashSpriteIndex]; // 랜덤 스프라이트 적용
        }

        Invoke("Deactivate", flashTime); // 지정한 시간 후 Deactivate 메소드 호출
    }

    // ■ 화염 효과 비활성화 메소드 
    void Deactivate() {
        flashHolder.SetActive(false); // 화염 효과 오브젝트 비활성화
    }


}
