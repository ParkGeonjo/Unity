using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour {
    public Image fadePlane; // 페이드 이미지(UI 배경) 오브젝트 레퍼런스
    public GameObject gameOverUI; // 게임 오버 텍스트, 버튼 오브젝트 레퍼런스

    void Start() {
        FindObjectOfType<Player>().OnDeath += OnGameOver; // 플레이어 사망 이벤트 구독
    }

    // ■ 플레이어 사망(게임 오버) 시 UI 처리 메소드
    void OnGameOver() {
        StartCoroutine(Fade(Color.clear, Color.black, 1)); // 배경 이미지 페이드 인 효과 코루틴 시작
        gameOverUI.SetActive(true); // 게임 오버 텍스트, 버튼 오브젝트 활성화
    }

    // ■ 이미지 페이드 효과 코루틴
    IEnumerator Fade(Color from, Color to, float time) {
        float speed = 1 / time; // 페이드 효과 속도
        float percent = 0; // 이미지 색상(투명도) 변화 퍼센트

        while(percent < 1) { // 색상(투명도) 변화가 1(100%)미만일 때
            percent += Time.deltaTime * speed; // 색상(투명도) 변화율 계산
            fadePlane.color = Color.Lerp(from, to, percent); // 색상(투명도) 지정
            yield return null; // 다음 프레임으로 건너뜀
        }
    }

    // UI Input 부분

    // ■ 새 게임 시작 메소드
    public void startNewGame() {
        Application.LoadLevel("Game"); // 게임 씬, 게임을 다시 로드
    }
}
